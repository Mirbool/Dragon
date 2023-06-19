using Config;
using Koakuma.Game.Characters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame;
using TGame.ECS;
using TGame.Message;

namespace Koakuma.Game.ECS
{
    public class CharacterAttributeComponent : ECSComponent
    {
        public Dictionary<CharacterAttributeType, long> valueMap;
    }

    public class CharacterAttributeComponentAwakeSystem : AwakeSystem<CharacterAttributeComponent>
    {
        public override void Awake(CharacterAttributeComponent c)
        {
            c.valueMap = DictionaryPool<CharacterAttributeType, long>.Obtain();
        }
    }

    public class CharacterAttributeComponentDestroySystem : DestroySystem<CharacterAttributeComponent>
    {
        public override void Destroy(CharacterAttributeComponent c)
        {
            if (c.valueMap != null)
            {
                DictionaryPool<CharacterAttributeType, long>.Release(c.valueMap);
                c.valueMap = null;
            }
        }
    }

    public class GetCharacterAttributeValueRpcHandler : EntityRpcHandler<EntityMessageType.GetCharacterAttributeValueRequest, EntityMessageType.GetCharacterAttributeValueResponse>
    {
        public override async Task<EntityMessageType.GetCharacterAttributeValueResponse> HandleRpc(ECSEntity entity, EntityMessageType.GetCharacterAttributeValueRequest request)
        {
            CharacterAttributeComponent attributeComponent = entity.GetComponent<CharacterAttributeComponent>();
            if (attributeComponent == null)
                return new EntityMessageType.GetCharacterAttributeValueResponse() { Error = true, ErrorMessage = "Attribute component not found" };

            if (!attributeComponent.valueMap.TryGetValue(request.key, out long value))
                return new EntityMessageType.GetCharacterAttributeValueResponse() { Error = true, ErrorMessage = $"Attribute not found key:{request.key}" };

            await Task.Yield();
            return new EntityMessageType.GetCharacterAttributeValueResponse() { value = value };
        }
    }

    //public class CharacterAddExpMessageHandler : EntityMessageHandler<EntityMessageType.AddExp>
    //{
    //    public override async Task HandleMessage(ECSEntity entity, EntityMessageType.AddExp message)
    //    {
    //        CharacterAttributeComponent attributeComponent = entity.GetComponent<CharacterAttributeComponent>();
    //        if (attributeComponent == null)
    //            return;

    //        long oldValue, newValue;
    //        long oldLevel = CharacterHelper.GetAttribute(attributeComponent, CharacterAttributeType.Level);
    //        ChangeExp(attributeComponent, message, out oldValue, out newValue);
    //        long newLevel = CharacterHelper.GetAttribute(attributeComponent, CharacterAttributeType.Level);

          //PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();
    //        if (playerComponent.playerEntityID == entity.InstanceID)
    //        {
    //            playerComponent.gameInfo.playerInfo.characterAttributes[CharacterAttributeType.CurrentExp] = newValue;
    //            playerComponent.gameInfo.playerInfo.characterAttributes[CharacterAttributeType.Level] = newLevel;
    //        }

    //        if (oldLevel != newLevel)
    //        {
    //            await GameManager.Message.Post(new MessageType.CharacterAttributeChanged()
    //            {
    //                entityID = attributeComponent.EntityID,
    //                type = CharacterAttributeType.Level,
    //                oldValue = oldLevel,
    //                newValue = newLevel,
    //            });
    //        }

    //        if (oldValue != newValue)
    //        {
    //            await GameManager.Message.Post(new MessageType.CharacterAttributeChanged()
    //            {
    //                entityID = attributeComponent.EntityID,
    //                type = CharacterAttributeType.CurrentExp,
    //                oldValue = oldValue,
    //                newValue = newValue,
    //            });
    //        }
    //    }

    //    /// <summary>
    //    /// 调整经验值
    //    /// </summary>
    //    /// <param name="attributeComponent"></param>
    //    /// <param name="message"></param>
    //    /// <param name="oldValue"></param>
    //    /// <param name="newValue"></param>
    //    private void ChangeExp(CharacterAttributeComponent attributeComponent, EntityMessageType.AddExp message, out long oldValue, out long newValue)
    //    {
    //        if (message.valueChange < 0)
    //        {
    //            UnityLog.Error($"Can't reduce Experience");
    //            oldValue = 0;
    //            newValue = 0;
    //            return;
    //        }

    //        attributeComponent.valueMap.TryGetValue(CharacterAttributeType.CurrentExp, out oldValue);
    //        newValue = oldValue + message.valueChange;

    //        int level = (int)CharacterHelper.GetAttributeD(attributeComponent, CharacterAttributeType.Level);
    //        while (true)
    //        {
    //            //TODO: 超出配表上限的处理

    //            ExperienceGrowthConfig expGrowthConfig = ExperienceGrowthConfig.ByID(level);
    //            long levelExp = expGrowthConfig.UpgradeExp * 1000;
    //            if (newValue >= levelExp)
    //            {
    //                newValue -= levelExp;
    //                level++;
    //                continue;
    //            }

    //            break;
    //        }

    //        attributeComponent.valueMap[CharacterAttributeType.CurrentExp] = newValue;
    //        attributeComponent.valueMap[CharacterAttributeType.Level] = level * 1000;
    //    }
    //}

    //public class CharacterChangeAttributeMessageHandler : EntityMessageHandler<EntityMessageType.ChangeAttribute>
    //{
    //    public override async Task HandleMessage(ECSEntity entity, EntityMessageType.ChangeAttribute message)
    //    {
    //        CharacterAttributeComponent attributeComponent = entity.GetComponent<CharacterAttributeComponent>();
    //        if (attributeComponent == null)
    //            return;

    //        long oldValue, newValue;

    //        switch (message.type)
    //        {
    //            case CharacterAttributeType.Vitality:
    //                ChangeCommonAttribute(attributeComponent, message, out oldValue, out newValue);
    //                await UpdateMaxHP(attributeComponent);
    //                break;
    //            case CharacterAttributeType.CurrentHP:
    //                ChangeCurrentHP(attributeComponent, message, out oldValue, out newValue);
    //                break;
    //            case CharacterAttributeType.CurrentExp:
    //                oldValue = 0;
    //                newValue = 0;
    //                UnityLog.Error($"Use AddExp instead");
    //                break;
    //            default:
    //                ChangeCommonAttribute(attributeComponent, message, out oldValue, out newValue);
    //                break;
    //        }

    //        if (oldValue != newValue)
    //        {
    //            await GameManager.Message.Post(new MessageType.CharacterAttributeChanged()
    //            {
    //                entityID = entity.InstanceID,
    //                type = message.type,
    //                oldValue = oldValue,
    //                newValue = newValue,
    //            });
    //        }
    //    }

        //private async Task UpdateMaxHP(CharacterAttributeComponent attributeComponent)
        //{
        //    long oldValue = CharacterHelper.GetAttribute(attributeComponent, CharacterAttributeType.MaxHP);
        //    long newValue = CombatFormularHelper.CalculateMaxHP(attributeComponent);
        //    if (oldValue == newValue)
        //        return;

        //    attributeComponent.valueMap[CharacterAttributeType.MaxHP] = newValue;
        //    await GameManager.Message.Post(new MessageType.CharacterAttributeChanged()
        //    {
        //        entityID = attributeComponent.EntityID,
        //        type = CharacterAttributeType.MaxHP,
        //        oldValue = oldValue,
        //        newValue = newValue,
        //    });
        //}

    //    private void ChangeCurrentHP(CharacterAttributeComponent attributeComponent, EntityMessageType.ChangeAttribute message, out long oldValue, out long newValue)
    //    {
    //        attributeComponent.valueMap.TryGetValue(CharacterAttributeType.CurrentHP, out long currentHP);
    //        attributeComponent.valueMap.TryGetValue(CharacterAttributeType.MaxHP, out long maxHP);

    //        oldValue = currentHP;
    //        newValue = Math.Min(currentHP + message.valueChange, maxHP);
    //        attributeComponent.valueMap[CharacterAttributeType.CurrentHP] = newValue;
    //    }

    //    private static void ChangeCommonAttribute(CharacterAttributeComponent attributeComponent, EntityMessageType.ChangeAttribute message, out long oldValue, out long newValue)
    //    {
    //        attributeComponent.valueMap.TryGetValue(message.type, out oldValue);
    //        newValue = oldValue + message.valueChange;
    //        attributeComponent.valueMap[message.type] = newValue;
    //    }
    //}

    public class CharacterAttributeChangedMessageHandler : MessageHandler<MessageType.CharacterAttributeChanged>
    {
        public override async Task HandleMessage(MessageType.CharacterAttributeChanged arg)
        {
            ECSEntity entity = GameManager.ECS.FindEntity(arg.entityID);
            if (entity == null)
                return;

            UnityLog.Info($"character value changed, id:{arg.entityID}, type:{arg.type}, {arg.oldValue / 1000d}===>{arg.newValue / 1000d}");

            await Task.Yield();
        }
    }
}