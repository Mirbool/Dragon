using Config;
using Koakuma.Game.Cameras;
//using Koakuma.Game.Cameras;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using Koakuma.Game.Skills;
//using Koakuma.Game.Skills;
using Rewired;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Inputs
{
    public class InputComponent : ECSComponent
    {
        public bool enabled;
        public Player inputPlayer;
        public GameObject rewiredInputManager;
    }

    public class InputComponentAwakeSystem : AwakeSystem<InputComponent>
    {
        public override void Awake(InputComponent c)
        {
            InputManager inputManager = Resources.Load<InputManager>("Rewired Input Manager");
            c.rewiredInputManager = UnityEngine.Object.Instantiate(inputManager.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(c.rewiredInputManager);
            c.inputPlayer = ReInput.players.GetPlayer(0);
            c.enabled = true;

            GameManager.ECS.SendMessageToEntity(c.EntityID, new EntityMessageType.BindSkillInput()).Coroutine();
        }
    }

    public class InputComponentDestroySystem : DestroySystem<InputComponent>
    {
        public override void Destroy(InputComponent c)
        {
            if (c.rewiredInputManager != null)
            {
                UnityEngine.Object.Destroy(c.rewiredInputManager);
                c.rewiredInputManager = null;
            }
            c.inputPlayer?.ClearInputEventDelegates();
            c.inputPlayer = null;
        }
    }

    public class InputComponentUpdateSystem : UpdateSystem<InputComponent>
    {
        public override void Update(ECSEntity entity)
        {
            InputComponent inputComponent = entity.GetComponent<InputComponent>();
            if (!inputComponent.enabled)
                return;

            Move(entity, inputComponent);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Message.Post(new MessageType.ExitDungeon()).Coroutine();
            }
        }

        private void Move(ECSEntity entity, InputComponent inputComponent)
        {
            CharacterMoveComponent moveComponent = entity.GetComponent<CharacterMoveComponent>();
            if (moveComponent == null)
                return;

            float horizontal = inputComponent.inputPlayer.GetAxisRaw((int)InputConst.ActionID.GamePlay_MoveHorizontal);
            float vertical = inputComponent.inputPlayer.GetAxisRaw((int)InputConst.ActionID.GamePlay_MoveVertical);
            Vector3 inputDirection = new Vector3(horizontal, 0, vertical);

            if (inputDirection == Vector3.zero || !moveComponent.movable)
            {
                moveComponent.destSpeed = 0;
            }
            else
            {
                //player.MoveForward(JoystickDirection);
                // update destSpeed
                //float? moveSpeed = null;
                //if (State == CharacterState.CastSkill)
                //{
                //    SkillData skill = SkillController.GetSkill(SkillController.CastingSkillID);
                //    if (skill != null && skill.Config.MoveSpeed > 0)
                //    {
                //        moveSpeed = skill.Config.MoveSpeed;
                //    }
                //}
                //if (!moveSpeed.HasValue)
                //{
                //    moveSpeed = AttrController.MoveSpeed;
                //}
                //destSpeed = moveSpeed.Value;
                float destSpeed = moveComponent.maxSpeed;

                CameraComponent cameraComponent = GameManager.ECS.World.GameScene.GetComponent<CameraComponent>();
                if (cameraComponent == null)
                    return;

                CharacterViewComponent characterViewComponent = entity.GetComponent<CharacterViewComponent>();
                if (characterViewComponent == null)
                    return;

                Vector3 cameraForward = cameraComponent.brain.ActiveVirtualCamera.VirtualCameraGameObject.transform.forward;
                Vector3 normal = Vector3.up;
                Vector3.OrthoNormalize(ref normal, ref cameraForward);
                Quaternion rotation = Quaternion.LookRotation(inputDirection.ToXZPlane());
                Vector3 movement = rotation * cameraForward;

                moveComponent.destSpeed = destSpeed;
                moveComponent.direction = movement;  
                moveComponent.lockRotation = false;
            }
        }
    }

    public class InputComponentBindSkillInputHandler : EntityMessageHandler<EntityMessageType.BindSkillInput>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.BindSkillInput message)
        {
           
            InputComponent inputComponent = entity.GetComponent<InputComponent>();
            if (inputComponent == null)
                return;

            SkillComponent skillComponent = entity.GetComponent<SkillComponent>();
            if (skillComponent == null)
                return;

            List<InputConst.ActionID> actionIDList = new List<InputConst.ActionID>() {
                InputConst.ActionID.GamePlay_Attack,
                InputConst.ActionID.GamePlay_Skill1,
                InputConst.ActionID.GamePlay_Skill2,
                InputConst.ActionID.GamePlay_Skill3,
                InputConst.ActionID.GamePlay_Skill4,
                InputConst.ActionID.GamePlay_Dash,
            };
            for (int i = 0; i < skillComponent.mainSkillIDList.Count; i++)
            {
                int skillID = skillComponent.mainSkillIDList[i];
                SkillData skillData = skillComponent.skillIDMap[skillID];
                inputComponent.inputPlayer.AddInputEventDelegate(OnClick(inputComponent, skillData), UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, (int)actionIDList[i]);
            }

            await Task.Yield();
        }

        private Action<InputActionEventData> OnClick(InputComponent inputComponent, SkillData skillData)
        {
            return async (e) =>
            {
                if (!inputComponent.enabled)
                    return;
                ///选择技能目标
                SkillTarget? skillTarget = await SelectInputTarget(inputComponent, skillData);
                EntityMessageType.CastSkillResponse response = await GameManager.ECS.SendRpcToEntity<EntityMessageType.CastSkillRequest, EntityMessageType.CastSkillResponse>(inputComponent.EntityID, new EntityMessageType.CastSkillRequest()
                {
                    configID = skillData.configID,
                    target = skillTarget,
                });
                if (response.Error)
                {
                    UnityLog.Warn($"Cast skill failed: {response.ErrorMessage}");
                }
                if (response.comboImmediately)
                {
                    GameManager.ECS.SendMessageToEntity(inputComponent.EntityID, new EntityMessageType.SkillReachComboTime()).Coroutine();
                }
            };
        }

        private async Task<SkillTarget?> SelectInputTarget(InputComponent inputComponent, SkillData skillData)
        {
            SkillConfig skillConfig = SkillConfig.ByID(skillData.configID);
            if ((skillConfig.InputTarget & SkillTargetType.Unit) != 0)
            {
                // 尝试吸附目标
                long? raycastCharacter = CameraHelper.RaycastCharacterEntity(Input.mousePosition);
                if (raycastCharacter.HasValue)
                {
                    EntityMessageType.SkillInputTargetSatisfiedResponse skillTargetSatisfied = await GameManager.ECS.SendRpcToEntity<EntityMessageType.SkillInputTargetSatisfiedRequest, EntityMessageType.SkillInputTargetSatisfiedResponse>(inputComponent.EntityID, new EntityMessageType.SkillInputTargetSatisfiedRequest()
                    {
                        targetID = raycastCharacter.Value,
                        skillID = skillData.configID,
                    });

                    if (!skillTargetSatisfied.Error)
                    {
                        return new SkillTarget()
                        {
                            type = SkillTargetType.Unit,
                            entity = raycastCharacter.Value,
                        };
                    }
                }

                // 若没有命中，则根据一系列选取公式进行选择
                long? targetEntityID = null;
                List<long> entityList = ListPool<long>.Obtain();
                GameManager.ECS.World.GameScene.FindEntities<CharacterEntity>(entityList);

                Vector3? raycastPosition = CameraHelper.RaycastToXZPlane(Input.mousePosition);
                if (raycastPosition.HasValue)
                {
                    float maxPoint = float.MinValue;
                    foreach (var entityID in entityList)
                    {
                        EntityMessageType.SkillInputTargetSatisfiedResponse skillTargetSatisfied = await GameManager.ECS.SendRpcToEntity<EntityMessageType.SkillInputTargetSatisfiedRequest, EntityMessageType.SkillInputTargetSatisfiedResponse>(inputComponent.EntityID, new EntityMessageType.SkillInputTargetSatisfiedRequest()
                        {
                            targetID = entityID,
                            skillID = skillData.configID,
                        });

                        if (skillTargetSatisfied.Error)
                            continue;

                        // 计算分数
                        ECSEntity entity = GameManager.ECS.FindEntity(entityID);
                        if (entity == null)
                            continue;

                        GameObjectComponent gameObjectComponent = entity.GetComponent<GameObjectComponent>();
                        if (gameObjectComponent == null)
                            continue;

                        float dist = Vector3.Distance(raycastPosition.Value, gameObjectComponent.position);
                        //float angle = Vector3.Angle(this.character.Rotation * Vector3.forward, (character.Position - this.character.Position).ToXZPlane());
                        // 面向越小，分数越大，10度以内分数相同。 距离越小，分数越大，1米以内分数相同。优先面向
                        //float point = (Mathf.FloorToInt(180 - angle) / 10 * 100) + (1f / dist);
                        float point = 100 / dist;
                        if (point > maxPoint)
                        {
                            maxPoint = point;
                            targetEntityID = entityID;
                        }
                    }
                }
                ListPool<long>.Release(entityList);

                if (targetEntityID != null)
                {
                    return new SkillTarget()
                    {
                        type = SkillTargetType.Unit,
                        entity = targetEntityID.Value,
                    };
                }
            }

            if ((skillConfig.InputTarget & SkillTargetType.Position) != 0)
            {
                Vector3? position = CameraHelper.RaycastToXZPlane(Input.mousePosition);
                if (position.HasValue)
                {
                    return new SkillTarget()
                    {
                        type = SkillTargetType.Position,
                        position = position.Value,
                    };
                }
            }

            return null;
        }
    }
}
