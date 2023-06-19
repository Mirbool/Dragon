using Beebyte.Obfuscator;
//using Koakuma.Game.GameCamera;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game
{
    public class CharacterViewMono : MonoBehaviour
    {
        public long entityID;

        private Animator animator;

        private ECSEntity entity;
        public ECSEntity Entity
        {
            get
            {
                if (entity == null)
                {
                    entity = GameManager.ECS.FindEntity(entityID);
                }
                return entity;
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorMove()
        {
            if (animator == null)
                return;

            if (animator.velocity != Vector3.zero)
            {
                transform.position = (transform.position + animator.deltaPosition).OnMovablePlane(false, true);
            }
        }

        [SkipRename]
        private void OnSkillAnimationEvent(AnimationEvent animationEvent)
        {
            //GameManager.Event.Fire<EventType.CharacterSkillAnimation>((args) =>
            //{
            //    args.CharacterID = Data.ID;
            //    args.AnimationEvent = animationEvent;
            //});
        }

        [SkipRename]
        private void OnAnimationFootStepL()
        {
            //GameManager.FX.PlayAtPosition(Data.StepFX, transform.position, Quaternion.identity);
            //GameManager.Audio.PlaySFX(Data.StepSFX, Rigidbody);
        }

        [SkipRename]
        private void OnAnimationFootStepR()
        {
            //GameManager.FX.PlayAtPosition(Data.StepFX, transform.position, Quaternion.identity);
            //GameManager.Audio.PlaySFX(Data.StepSFX, Rigidbody);
        }

        [SkipRename]
        private void OnAnimationEvent(AnimationEvent animationEvent)
        {
            //GameManager.Event.Fire<EventType.CharacterAnimation>((args) =>
            //{
            //    args.CharacterID = Data.ID;
            //    args.AnimationEvent = animationEvent;
            //});
            //switch (animationEvent.stringParameter)
            //{
            //    case "invisible":
            //        OnInvisible();
            //        break;
            //    default:
            //        break;
            //}
        }

        [SkipRename]
        private void OnAnimationFX(AnimationEvent animationEvent)
        {
            //UnityLog.Info($"PlayCharacterFX, ID:{ID}, FX:{animationEvent.intParameter}");
            //GameManager.FX.PlayCharacterEffect(animationEvent.intParameter, Data);
        }

        [SkipRename]
        private void OnAnimationSFX(AnimationEvent animationEvent)
        {
            //GameManager.Audio.PlaySFX(animationEvent.intParameter, Rigidbody);
        }

        [SkipRename]
        private void OnAnimationVibrate(AnimationEvent animationEvent)
        {
            //GameManager.ECS.SendMessageToEntity(entityID, new EntityMessageType.GenerateVibration()
            //{
            //    strength = (CameraVibrationStrength)animationEvent.intParameter
            //}).Coroutine();
        }

        [SkipRename]
        private void OnAnimationTimeScale(AnimationEvent animationEvent)
        {
            GameManager.Time.StartFreeze((Times.TimeModule.FreezeType)animationEvent.intParameter);
        }
    }
}
