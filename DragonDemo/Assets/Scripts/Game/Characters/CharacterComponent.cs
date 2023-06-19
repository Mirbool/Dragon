using Config;
using Koakuma.Game.ECS;
//using Koakuma.Game.ECS;
//using Koakuma.Game.Effects;
//using Koakuma.Game.FX;
//using Koakuma.Game.Items.Drop;
using Koakuma.Proto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;
using TGame.Message;

namespace Koakuma.Game.Characters
{
    public class CharacterComponent : ECSComponent
    {
        public int configID;
        public CharacterState state;
        public CharacterTeam team;
        public List<SkillInfo> skills;
        public long interruptEntityID;
        public InterruptStateType interruptState;
        public float interruptDuration;
    }

    public class CharacterComponentAwakeSystem : AwakeSystem<CharacterComponent, CharacterData>
    {
        public override void Awake(CharacterComponent c, CharacterData p1)
        {
            c.configID = p1.characterInfo.configID;
            c.skills = ListPool<SkillInfo>.Obtain();
            c.skills.AddRangeNonAlloc(p1.characterInfo.skills);
            c.team = p1.characterInfo.team;
            c.state = CharacterState.Unvitalized;
            c.interruptEntityID = 0;
            c.interruptState = InterruptStateType.None;
            c.interruptDuration = 0;
        }
    }

    public class CharacterComponentDestroySystem : DestroySystem<CharacterComponent>
    {
        public override void Destroy(CharacterComponent c)
        {
            c.configID = default;

            ListPool<SkillInfo>.Release(c.skills);
            c.skills = null;
            c.state = CharacterState.Unvitalized;
            c.interruptEntityID = 0;
            c.interruptState = InterruptStateType.None;
            c.interruptDuration = 0;
        }
    }

    // public class CharacterHPChangedMessageHandler : MessageHandler<MessageType.CharacterAttributeChanged>
    // {
    //public override async Task HandleMessage(MessageType.CharacterAttributeChanged arg)
    //{
    //    if (arg.type != CharacterAttributeType.CurrentHP)
    //        return;

    //    ECSEntity entity = GameManager.ECS.FindEntity(arg.entityID);
    //    if (entity == null)
    //        return;

    //    CharacterComponent characterComponent = entity.GetComponent<CharacterComponent>();
    //    if (characterComponent == null)
    //        return;

    //    UnityLog.Info($"character hp changed, id:{arg.entityID}, {arg.oldValue / 1000d}===>{arg.newValue / 1000d}");

    //    if (arg.newValue <= 0)
    //    {
    //        // dead
    //        OnDead(entity, characterComponent);
    //    }
    //}

    //private void OnDead(ECSEntity entity, CharacterComponent characterComponent)
    //{
    //    UnityLog.Info($"character dead, id:{entity.InstanceID}");
    //    characterComponent.state = CharacterState.Dead;

    //    AnimatorComponent animatorComponent = entity.GetComponent<AnimatorComponent>();
    //    if (animatorComponent != null)
    //    {
    //        animatorComponent.animator.SetBool(CharacterAnimationHash.PARAMETER_DEAD, true);
    //    }

    //    List<GameEffectEntity> effectList = ListPool<GameEffectEntity>.Obtain();
    //    entity.FindChildren(effectList);
    //    foreach (var effect in effectList)
    //    {
    //        effect.Dispose();
    //    }
    //    ListPool<GameEffectEntity>.Release(effectList);

    //    // stop casting
    //    GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.EndSkillCasting()
    //    {
    //        isCancel = true,
    //    }).Coroutine();

    //    // remove all buff
    //    GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.RemoveAllBuffs()).Coroutine();

    //    // stop AI
    //    GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.StopAI()).Coroutine();

    //    GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.UpdateMoveableState()).Coroutine();

    //    // drop
    //    CharacterConfig characterConfig = CharacterConfig.ByID(characterComponent.configID);
    //    foreach (var itemGroup in characterConfig.ItemRewardGroup)
    //    {
    //        DropFromPool(itemGroup);
    //    }

    //    if (characterComponent.team == CharacterTeam.Player)
    //    {
    //        GameManager.Message.Post(new MessageType.PlayerDead()).Coroutine();
    //    }
    //    else
    //    {
    //        long exp = characterConfig.MonsterExperience * 1000;
    //        CharacterEntity playerEntity = CharacterHelper.GetPlayerEntity();
    //        if (playerEntity != null && exp > 0)
    //        {
    //            GameManager.ECS.SendMessageToEntity(playerEntity.InstanceID, new EntityMessageType.AddExp()
    //            {
    //                valueChange = exp,
    //            }).Coroutine();
    //        }
    //    }
    //}

    //private void DropFromPool(int poolID)
    //{
    //    GameObjectComponent gameObjectComponent = CharacterHelper.GetPlayerEntity().GetComponent<GameObjectComponent>();
    //    List<ItemInfo> items = new List<ItemInfo>();
    //    DropHelper.DropRewardFromPool(poolID, items);
    //    foreach (var item in items)
    //    {
    //        DropFactory.CreateDropItem(new DropItemInfo()
    //        {
    //            position = gameObjectComponent.position,
    //            itemInfo = item,
    //        });
    //    }
    //}
    //  }

    //public class CharacterGetHitMessageHandler : EntityMessageHandler<EntityMessageType.GetHit>
    //{
    //    public override async Task HandleMessage(ECSEntity entity, EntityMessageType.GetHit message)
    //    {
    //        CharacterComponent targetCharacterComponent = entity.GetComponent<CharacterComponent>();
    //        if (targetCharacterComponent == null)
    //            return;

    //        if (targetCharacterComponent.state == CharacterState.Dead)
    //            return;

    //        ECSEntity attackerEntity = GameManager.ECS.FindEntity(message.attackerEntityID);
    //        CharacterAttributeComponent attackerAttribute = attackerEntity?.GetComponent<CharacterAttributeComponent>();
    //        if (attackerAttribute == null)
    //            return;

    //        CharacterAttributeComponent targetAttribute = entity.GetComponent<CharacterAttributeComponent>();
    //        if (targetAttribute == null)
    //            return;

    //        DamageResultData damageResultData = CombatFormularHelper.CalculateDamage(new CombatFormularHelper.DamageCalculateData()
    //        {
    //            attackerAttribute = attackerAttribute,
    //            targetAttribute = targetAttribute,
    //            damageElementType = CombatFormularHelper.DamageElementType.Physical,
    //            pureElementDamage = false,
    //            damagePerct = message.attackPerct,
    //            mainAttr = CharacterAttributeType.Strength,
    //        });

    //        await GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.ChangeAttribute()
    //        {
    //            type = CharacterAttributeType.CurrentHP,
    //            valueChange = -(long)Math.Floor(damageResultData.restrictedDamage * 1000.0),
    //        });

    //        if (message.fx > 0)
    //        {
    //            FXHelper.PlayFX(new PlayFXData()
    //            {
    //                configID = message.fx,
    //                targetEntityID = entity.InstanceID,
    //                parentEntityID = entity.InstanceID,
    //            });
    //        }

    //        GameObjectComponent targetGameObjectComponent = entity.GetComponent<GameObjectComponent>();
    //        if (targetGameObjectComponent != null)
    //        {
    //            if (message.sfx > 0)
    //            {
    //                GameManager.Audio.PlaySFX(message.sfx, targetGameObjectComponent.rigidbody);
    //            }
    //        }

    //        CharacterViewComponent targetCharacterViewComponent = entity.GetComponent<CharacterViewComponent>();
    //        if (targetCharacterViewComponent != null)
    //        {
    //            if (targetCharacterComponent.state == CharacterState.Dead)
    //            {
    //                // ragdoll
    //                //bool useRagdoll = attackerEntity != null && damageSource != null && (damageSource.SourceType == EffectSourceType.Skill || damageSource.SourceType == EffectSourceType.SkillRune);

    //                RagdollConfig ragdollConfig = targetCharacterViewComponent.ragDollConfig;
    //                bool useRagdoll = ragdollConfig != null;
    //                if (useRagdoll)
    //                {
    //                    GameObjectComponent attackerGameObjectComponent = attackerEntity.GetComponent<GameObjectComponent>();
    //                    ragdollConfig.StartRagdoll(attackerGameObjectComponent.position).Coroutine();

    //                    if (targetCharacterViewComponent != null)
    //                    {
    //                        targetCharacterViewComponent.collider.enabled = false;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public class CharacterVitalizeMessageHandler : EntityMessageHandler<EntityMessageType.CharacterVitalize>
    {
        public override async Task HandleMessage(ECSEntity entity, EntityMessageType.CharacterVitalize message)
        {
            CharacterComponent characterComponent = entity.GetComponent<CharacterComponent>();
            if (characterComponent == null)
                return;

            if (characterComponent.state != CharacterState.Unvitalized)
                return;

            CharacterAttributeComponent attributeComponent = entity.GetComponent<CharacterAttributeComponent>();
            if (attributeComponent == null)
                return;

            characterComponent.state = CharacterState.Idle;

            //attributeComponent.valueMap[CharacterAttributeType.MaxHP] = CombatFormularHelper.CalculateMaxHP(attributeComponent);
            //attributeComponent.valueMap[CharacterAttributeType.CurrentHP] = CharacterHelper.GetAttribute(attributeComponent, CharacterAttributeType.MaxHP);

            GameManager.ECS.SendMessageToEntity(entity.InstanceID, new EntityMessageType.UpdateMoveableState()).Coroutine();

            await GameManager.Message.Post(new MessageType.CharacterVitalized()
            {
                entityID = entity.InstanceID,
            });

            await Task.Yield();
        }
    }
}