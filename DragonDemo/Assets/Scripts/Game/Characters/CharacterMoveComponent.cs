using Config;
using Koakuma.Game.ECS;
//using Koakuma.Game.Skills;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Characters
{
    public class CharacterMoveComponent : ECSComponent
    {
        public float maxSpeed;
        public float currentSpeed;
        public float destSpeed;
        public Vector3 direction;
        public Vector3 localDirection;
        public bool movable;
        public bool lockRotation;
        public MoveState moveState;
    }

    public enum MoveState
    {
        Idle,
        StartMove,
        Moving,
    }

    public class CharacterMoveComponentAwakeSystem : AwakeSystem<CharacterMoveComponent>
    {
        public override void Awake(CharacterMoveComponent c)
        {
            c.maxSpeed = 5;
        }
    }

    public class CharacterMoveComponentDestroySystem : DestroySystem<CharacterMoveComponent>
    {
        public override void Destroy(CharacterMoveComponent c)
        {
            c.maxSpeed = default;
            c.currentSpeed = default;
            c.destSpeed = default;
            c.direction = default;
            c.localDirection = default;
            c.movable = default;
            c.lockRotation = default;
            c.moveState = default;
        }
    }

    public class CharacterMoveComponentUpdateSystem : FixedUpdateSystem<CharacterMoveComponent>
    {
        public override void FixedUpdate(ECSEntity entity)
        {
            CharacterMoveComponent moveComponent = entity.GetComponent<CharacterMoveComponent>();
            if (moveComponent.currentSpeed == 0 && moveComponent.destSpeed == 0)
                return;

            AnimatorComponent animatorComponent = entity.GetComponent<AnimatorComponent>();
            GameObjectComponent gameObjectComponent = entity.GetComponent<GameObjectComponent>();

            float deltaTime = Time.fixedDeltaTime;
            float lastCurrentSpeed = moveComponent.currentSpeed;
            moveComponent.currentSpeed = Mathf.MoveTowards(moveComponent.currentSpeed, moveComponent.destSpeed, 100 * deltaTime);

            MoveState lastMoveState = moveComponent.moveState;
            if (lastCurrentSpeed == 0 && moveComponent.currentSpeed != 0)
            {
                moveComponent.moveState = MoveState.StartMove;
            }
            else if (moveComponent.currentSpeed == 0)
            {
                moveComponent.moveState = MoveState.Idle;

              // SkillComponent skillComponent = entity.GetComponent<SkillComponent>();
                //if (skillComponent == null || !skillComponent.isCastingSkill)
                //{
                    animatorComponent.animator.Play(CharacterAnimationHash.FREE);
                //}
            }
            else
            {
                moveComponent.moveState = MoveState.Moving;
            }
            if (lastMoveState != moveComponent.moveState)
            {
                GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.MoveStateChanged() { lastMoveState = lastMoveState, currentMoveState = moveComponent.moveState }).Coroutine();
            }

            animatorComponent.animator.SetFloat(CharacterAnimationHash.PARAMETER_SPEED, moveComponent.destSpeed == 0 ? 0 : moveComponent.currentSpeed / moveComponent.destSpeed);
            // 旋转
            moveComponent.localDirection = Quaternion.Inverse(gameObjectComponent.rotation) * moveComponent.direction;
            if (!moveComponent.lockRotation)
            {
                Vector3 forward = moveComponent.direction.ToXZPlane();
                if (forward != Vector3.zero)
                {
                    if (moveComponent.destSpeed != 0)
                    {
                        gameObjectComponent.rotation = Quaternion.LookRotation(forward, Vector3.up);
                    }
                }
            }
            else
            {
                animatorComponent.animator.SetFloat(CharacterAnimationHash.PARAMETER_LOCK_ON_SPEED_X, moveComponent.localDirection.x);
                animatorComponent.animator.SetFloat(CharacterAnimationHash.PARAMETER_LOCK_ON_SPEED_Y, moveComponent.localDirection.z);
            }

            // 移动
            CharacterViewComponent characterViewComponent = entity.GetComponent<CharacterViewComponent>();
            if (characterViewComponent != null)
            {
                characterViewComponent.advancedWalkerController.movementSpeed = moveComponent.currentSpeed;
            }
        }
    }

    public class StopMoveHandler : EntityMessageHandler<EntityMessageType.StopMove>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.StopMove message)
        {
            CharacterMoveComponent moveComponent = entity.GetComponent<CharacterMoveComponent>();
            if (moveComponent == null)
                return;

            moveComponent.currentSpeed = 0;
            moveComponent.destSpeed = 0;
            moveComponent.direction = default;
            moveComponent.localDirection = default;

            // 更新动画
            AnimatorComponent animatorComponent = entity.GetComponent<AnimatorComponent>();
            if (animatorComponent != null)
            {
                animatorComponent.animator.Play(CharacterAnimationHash.FREE);
                animatorComponent.animator.SetFloat(CharacterAnimationHash.PARAMETER_SPEED, 0);
            }

            await Task.Yield();
        }
    }

    public class UpdateMovableStateHandler : EntityMessageHandler<EntityMessageType.UpdateMoveableState>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.UpdateMoveableState message)
        {
            CharacterMoveComponent moveComponent = entity.GetComponent<CharacterMoveComponent>();
            if (moveComponent == null)
                return;

            bool isForbidMoveByCasting = false;
            //SkillComponent skillComponent = entity.GetComponent<SkillComponent>();
            //if (skillComponent != null && skillComponent.isCastingSkill)
            //{
            //    SkillConfig skillConfig = SkillConfig.ByID(skillComponent.castingSkillID);
            //    if (!skillConfig.MoveCast)
            //    {
            //        isForbidMoveByCasting = true;
            //    }
            //}

            bool isForbidMoveByState = false;
            bool isForbidMoveByInterrupt = false;
            CharacterComponent characterComponent = entity.GetComponent<CharacterComponent>();
            if (characterComponent != null)
            {
                if (characterComponent.state == CharacterState.Dead ||
                    characterComponent.state == CharacterState.Unvitalized)
                {
                    isForbidMoveByState = true;
                }

                if (characterComponent.interruptState != InterruptStateType.None)
                {
                    isForbidMoveByInterrupt = true;
                }
            }

            moveComponent.movable = !isForbidMoveByCasting && !isForbidMoveByState && !isForbidMoveByInterrupt;
          

            if (!moveComponent.movable && moveComponent.moveState != MoveState.Idle)
            {
                moveComponent.currentSpeed = 0;
                moveComponent.destSpeed = 0;
                moveComponent.direction = default;
                moveComponent.localDirection = default;

                // 更新动画
                AnimatorComponent animatorComponent = entity.GetComponent<AnimatorComponent>();
                if (animatorComponent != null)
                {
                    animatorComponent.animator.Play(CharacterAnimationHash.FREE);
                    animatorComponent.animator.SetFloat(CharacterAnimationHash.PARAMETER_SPEED, 0);
                }
            }

            await Task.Yield();
        }
    }
}
