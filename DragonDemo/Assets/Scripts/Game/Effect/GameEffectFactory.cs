using Config;
using Koakuma.Game.Characters;
using TGame.ECS;

namespace Koakuma.Game.Effects
{
    public static class GameEffectFactory
    {
        public static void CreateEffect(GameEffectData gameEffectData)
        {
            ECSEntity sourceEntity = GameManager.ECS.FindEntity(gameEffectData.sourceEntityID);
            if (sourceEntity == null)
                return;

            ECSEntity parentEntity = null;
            if (gameEffectData.effectRevert)
            {
                // 撤销的效果追加在效果来源上
                parentEntity = sourceEntity;
            }
            else
            {
                // 不撤销的效果追加在Unit或者场景上
                if (gameEffectData.target.type == SkillTargetType.Unit ||
                    gameEffectData.target.type == SkillTargetType.Object)
                {
                    parentEntity = GameManager.ECS.FindEntity(gameEffectData.target.entity);
                }
                else if (gameEffectData.target.type == SkillTargetType.Position)
                {
                    parentEntity = GameManager.ECS.World.GameScene;
                }
            }
            if (parentEntity == null)
                return;

            GameEffectEntity gameEffectEntity = new GameEffectEntity();
            parentEntity.AddChild(gameEffectEntity);

            GameEffectComponent gameEffectComponent = null;
            switch (gameEffectData.effectName)
            {
                //case "damage":
                //    gameEffectComponent = gameEffectEntity.AddComponent<DamageGameEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "knock_back":
                //    gameEffectComponent = gameEffectEntity.AddComponent<KnockBackEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "knock_back_forward":
                //    gameEffectComponent = gameEffectEntity.AddComponent<KnockBackForwardEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "shift":
                //    gameEffectComponent = gameEffectEntity.AddComponent<ShiftEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "shift_to_target":
                //    gameEffectComponent = gameEffectEntity.AddComponent<ShiftToTargetEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "add_buff":
                //    gameEffectComponent = gameEffectEntity.AddComponent<AddBuffEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "fire_bullet":
                //    gameEffectComponent = gameEffectEntity.AddComponent<FireBulletEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "time_freeze":
                //    gameEffectComponent = gameEffectEntity.AddComponent<TimeFreezeEffectComponent, GameEffectData>(gameEffectData);
                //    break;
                //case "stiff":
                //    gameEffectComponent = AddInterrupt(InterruptStateType.Stiff, gameEffectEntity, gameEffectData);
                //    break;
                //case "stun":
                //    gameEffectComponent = AddInterrupt(InterruptStateType.Stun, gameEffectEntity, gameEffectData);
                //    break;
                default:
                    UnityLog.Error($"Create Effect failed, unsupported effect:{gameEffectData.effectName}");
                    break;
            }

            if (gameEffectComponent != null)
            {
                if (gameEffectComponent.InstantEffect && !gameEffectComponent.Disposed)
                {
                    gameEffectComponent.Entity?.Dispose();
                }
            }
        }

        private static GameEffectComponent AddInterrupt(InterruptStateType newState, GameEffectEntity gameEffectEntity, GameEffectData gameEffectData)
        {
            if (gameEffectData.target.type != SkillTargetType.Unit)
                return null;

            ECSEntity targetEntity = GameManager.ECS.FindEntity(gameEffectData.target.entity);
            CharacterComponent characterComponent = targetEntity.GetComponent<CharacterComponent>();
            if (characterComponent == null)
                return null;

            InterruptStackActionConfig stackActionConfig = InterruptStackActionConfig.ByID((int)newState);
            InterruptStackActionType stackAction;
            switch (characterComponent.interruptState)
            {
                case InterruptStateType.Stiff:
                    stackAction = CalcInterrupt(characterComponent, stackActionConfig.Stiff);
                    break;
                case InterruptStateType.Stun:
                    stackAction = CalcInterrupt(characterComponent, stackActionConfig.Stun);
                    break;
                case InterruptStateType.KnockFloat:
                case InterruptStateType.KnockDown:
                case InterruptStateType.GetUp:
                case InterruptStateType.Hold:
                default:
                    stackAction = InterruptStackActionType.RemoveTarget;
                    break;
            }

            GameEffectComponent gameEffectComponent = null;
            if (stackAction != InterruptStackActionType.IgnoreSelf)
            {
                switch (newState)
                {
                    //case InterruptStateType.Stiff:
                    //    gameEffectComponent = gameEffectEntity.AddComponent<StiffGameEffectComponent, GameEffectData>(gameEffectData);
                    //    break;
                    //case InterruptStateType.Stun:
                    //    gameEffectComponent = gameEffectEntity.AddComponent<StunGameEffectComponent, GameEffectData>(gameEffectData);
                    //    break;
                    case InterruptStateType.KnockFloat:
                        break;
                    case InterruptStateType.KnockDown:
                        break;
                    case InterruptStateType.GetUp:
                        break;
                    case InterruptStateType.Hold:
                        break;
                    default:
                        break;
                }
            }
            return gameEffectComponent;
        }

        private static InterruptStackActionType CalcInterrupt(CharacterComponent characterComponent, InterruptStackActionType stackAction)
        {
            if (stackAction == InterruptStackActionType.RemoveTarget)
            {
                ECSEntity interruptEntity = GameManager.ECS.FindEntity(characterComponent.interruptEntityID);
                interruptEntity?.Dispose();
            }
            return stackAction;
        }
    }
}
