using Config;
using Koakuma.Game.ECS;
using TGame.ECS;
//using Koakuma.Game.Weapons;
//using Koakuma.Proto;
//using System.Collections.Generic;
//using TGame.ECS;

namespace Koakuma.Game.Characters
{
    public static class CharacterHelper
    {
        public static bool IsPlayerEntity(long entityID)
        {
            PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();
            if (playerComponent == null)
                return false;

            return playerComponent.playerEntityID == entityID;
        }

        public static CharacterEntity GetPlayerEntity()
        {
            PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();
            if (playerComponent == null)
                return default;

            return GameManager.ECS.FindEntity(playerComponent.playerEntityID) as CharacterEntity;
        }
        public static CharacterRelationType GetRelation(long entity1, long entity2)
        {
            return GetRelation(GameManager.ECS.FindEntity(entity1), GameManager.ECS.FindEntity(entity2));
        }

        public static CharacterRelationType GetRelation(ECSEntity entity1, ECSEntity entity2)
        {
            if (entity1 == null || entity2 == null)
                return CharacterRelationType.None;

            CharacterComponent characterComponent1 = entity1.GetComponent<CharacterComponent>();
            CharacterComponent characterComponent2 = entity2.GetComponent<CharacterComponent>();
            if (characterComponent1 == null || characterComponent2 == null)
                return CharacterRelationType.None;

            if (entity1.InstanceID == entity2.InstanceID)
                return CharacterRelationType.Self;

            if (characterComponent1.team == CharacterTeam.Monster && characterComponent2.team == CharacterTeam.Neutral ||
                characterComponent1.team == CharacterTeam.Neutral && characterComponent2.team == CharacterTeam.Monster)
                return CharacterRelationType.Ally;

            return characterComponent1.team == characterComponent2.team ? CharacterRelationType.Ally : CharacterRelationType.Hostile;
        }

        //public static InterruptStateType GetInterruptState(long entityID)
        //{
        //    ECSEntity entity = GameManager.ECS.FindEntity(entityID);
        //    CharacterComponent characterComponent = entity?.GetComponent<CharacterComponent>();
        //    if (characterComponent == null)
        //        return InterruptStateType.None;

        //    return characterComponent.interruptState;
        //}

        //public static bool IsCharacterDead(long characterID)
        //{
        //    ECSEntity entity = GameManager.ECS.FindEntity(characterID);
        //    return IsCharacterDead(entity?.GetComponent<CharacterComponent>());
        //}

        //public static bool IsCharacterDead(CharacterComponent characterComponent)
        //{
        //    if (characterComponent == null)
        //        return false;

        //    return characterComponent.state == CharacterState.Dead;
        //}

        public static CharacterState GetCharacterState(long characterID)
        {
            ECSEntity entity = GameManager.ECS.FindEntity(characterID);
            return entity?.GetComponent<CharacterComponent>().state ?? CharacterState.Unvitalized;
        }

        //public static void FindAllHositleEntites(long selfEntityID, List<long> characterList)
        //{
        //    GameManager.ECS.World.GameScene.GetAllEntities(characterList);
        //    for (int i = characterList.Count - 1; i >= 0; i--)
        //    {
        //        CharacterRelationType relation = GetRelation(selfEntityID, characterList[i]);
        //        if (relation != CharacterRelationType.Hostile)
        //        {
        //            characterList.RemoveAt(i);
        //            continue;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 穿上装备属性
        ///// </summary>
        ///// <param name="characterEntity"></param>
        ///// <param name="attributeComponent"></param>
        ///// <param name="itemInfo"></param>
        //public static bool WearEquipmentAttribute(CharacterEntity characterEntity, CharacterAttributeComponent attributeComponent, Proto.ItemInfo itemInfo)
        //{
        //    if (characterEntity == null)
        //        return false;

        //    if (attributeComponent == null)
        //    {
        //        UnityLog.Error($"Wear equipment fail, not found attribute component, character entity id:{characterEntity.InstanceID}");
        //        return false;
        //    }

        //    EquipmentBasePropertyConfig equipmentConfig = EquipmentBasePropertyConfig.ByID(itemInfo.equipmentInfo.configID);
        //    if (Parts.TwoHanded.HasFlag(equipmentConfig.Parts))
        //    {
        //        // weapon
        //        WeaponData weaponData = new WeaponData()
        //        {
        //            itemInfo = itemInfo,
        //        };
        //        WeaponEntity weapon = WeaponFactory.CreateWeapon(weaponData);

        //        // attach weapon
        //        characterEntity.AddChild(weapon);
        //        GameManager.ECS.SendMessageToEntity(characterEntity.InstanceID, new EntityMessageType.AttachWeapon() { weaponEntity = weapon }).Coroutine();
        //    }

        //    // equipment base attribute
        //    if (itemInfo.equipmentInfo.baseAttributes != null)
        //    {
        //        for (int i = 0; i < equipmentConfig.RandomAttributeDatas.Length; i++)
        //        {
        //            RandomAttributeData baseAttr = equipmentConfig.RandomAttributeDatas[i];
        //            if (baseAttr.AttributeType == 0)
        //                continue;

        //            if (i >= itemInfo.equipmentInfo.baseAttributes.Count)
        //                continue;

        //            double rate = itemInfo.equipmentInfo.baseAttributes[i] / 100000D;
        //            double attr = baseAttr.AttributeInterval[0] + (baseAttr.AttributeInterval[1] - baseAttr.AttributeInterval[0]) * rate;

        //            // quality addition
        //            EquipmentQualityConfig qualityConfig = EquipmentQualityConfig.ByID((int)itemInfo.equipmentInfo.quality);
        //            attr *= (1 + qualityConfig.AdditionPercentage);

        //            attributeComponent.valueMap.TryGetValue((CharacterAttributeType)baseAttr.AttributeType, out long value);
        //            value += (long)(attr * 1000L);
        //            attributeComponent.valueMap[(CharacterAttributeType)baseAttr.AttributeType] = value;
        //        }
        //    }

        //    // equipment special attribute
        //    if (itemInfo.equipmentInfo.specialAttributes != null)
        //    {
        //        foreach (var specialAttr in itemInfo.equipmentInfo.specialAttributes)
        //        {
        //            EquipmentRandomAttributeConfig equipmentRandomAttributeConfig = EquipmentRandomAttributeConfig.ByID(specialAttr.Key);
        //            for (int i = 0; i < equipmentRandomAttributeConfig.RandomAttributeIds.Length; i++)
        //            {
        //                RandomAttributeId randomAttrData = equipmentRandomAttributeConfig.RandomAttributeIds[i];
        //                if (randomAttrData.AttributeId == 0)
        //                    continue;

        //                double rate = specialAttr.Value[i] / 100000D;
        //                double attr = randomAttrData.GrowInterval[0] + (randomAttrData.GrowInterval[1] - randomAttrData.GrowInterval[0]) * rate;

        //                attributeComponent.valueMap.TryGetValue((CharacterAttributeType)randomAttrData.AttributeId, out long value);
        //                value += (long)(attr * 1000L);
        //                attributeComponent.valueMap[(CharacterAttributeType)randomAttrData.AttributeId] = value;
        //            }
        //        }
        //    }

        //    UnityLog.Info($"Wear equipment:{itemInfo}");
        //    return true;
        //}

        ///// <summary>
        ///// 根据部件获取穿在身上的装备列表
        ///// </summary>
        ///// <param name="parts"></param>
        ///// <param name="wearingEquipmentList"></param>
        //public static void GetWearingEquipmentsByPart(PlayerComponent playerComponent, Parts parts, List<ItemInfo> wearingEquipmentList)
        //{
        //    if (playerComponent.gameInfo.playerInfo.equipments == null)
        //        return;

        //    foreach (var itemInfo in playerComponent.gameInfo.playerInfo.equipments)
        //    {
        //        EquipmentBasePropertyConfig equipmentConfig = EquipmentBasePropertyConfig.ByID(itemInfo.equipmentInfo.configID);
        //        if (parts.HasFlag(equipmentConfig.Parts) || equipmentConfig.Parts.HasFlag(parts))
        //        {
        //            wearingEquipmentList.Add(itemInfo);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 脱下装备属性
        ///// </summary>
        ///// <param name="playerEntity"></param>
        ///// <param name="attributeComponent"></param>
        ///// <param name="parts"></param>
        //public static bool TakeOffEquipmentAttribute(CharacterEntity characterEntity, CharacterAttributeComponent attributeComponent, ItemInfo itemInfo)
        //{
        //    if (characterEntity == null)
        //        return false;

        //    if (attributeComponent == null)
        //    {
        //        UnityLog.Error($"Take Off equipment fail, not found attribute component, character entity id:{characterEntity.InstanceID}");
        //        return false;
        //    }

        //    EquipmentBasePropertyConfig equipmentConfig = EquipmentBasePropertyConfig.ByID(itemInfo.equipmentInfo.configID);

        //    // equipment base attribute
        //    if (itemInfo.equipmentInfo.baseAttributes != null)
        //    {
        //        for (int i = 0; i < equipmentConfig.RandomAttributeDatas.Length; i++)
        //        {
        //            RandomAttributeData baseAttr = equipmentConfig.RandomAttributeDatas[i];
        //            if (baseAttr.AttributeType == 0)
        //                continue;

        //            if (i >= itemInfo.equipmentInfo.baseAttributes.Count)
        //                continue;

        //            double rate = itemInfo.equipmentInfo.baseAttributes[i] / 100000D;
        //            double attr = baseAttr.AttributeInterval[0] + (baseAttr.AttributeInterval[1] - baseAttr.AttributeInterval[0]) * rate;

        //            // quality addition
        //            EquipmentQualityConfig qualityConfig = EquipmentQualityConfig.ByID((int)itemInfo.equipmentInfo.quality);
        //            attr *= (1 + qualityConfig.AdditionPercentage);

        //            attributeComponent.valueMap.TryGetValue((CharacterAttributeType)baseAttr.AttributeType, out long value);
        //            value -= (long)(attr * 1000L);
        //            attributeComponent.valueMap[(CharacterAttributeType)baseAttr.AttributeType] = value;
        //        }
        //    }

        //    // equipment special attribute
        //    if (itemInfo.equipmentInfo.specialAttributes != null)
        //    {
        //        foreach (var specialAttr in itemInfo.equipmentInfo.specialAttributes)
        //        {
        //            EquipmentRandomAttributeConfig equipmentRandomAttributeConfig = EquipmentRandomAttributeConfig.ByID(specialAttr.Key);
        //            for (int i = 0; i < equipmentRandomAttributeConfig.RandomAttributeIds.Length; i++)
        //            {
        //                RandomAttributeId randomAttrData = equipmentRandomAttributeConfig.RandomAttributeIds[i];
        //                if (randomAttrData.AttributeId == 0)
        //                    continue;

        //                double rate = specialAttr.Value[i] / 100000D;
        //                double attr = randomAttrData.GrowInterval[0] + (randomAttrData.GrowInterval[1] - randomAttrData.GrowInterval[0]) * rate;

        //                attributeComponent.valueMap.TryGetValue((CharacterAttributeType)randomAttrData.AttributeId, out long value);
        //                value -= (long)(attr * 1000L);
        //                attributeComponent.valueMap[(CharacterAttributeType)randomAttrData.AttributeId] = value;
        //            }
        //        }
        //    }

        //    if (Parts.TwoHanded.HasFlag(equipmentConfig.Parts))
        //    {
        //        // detach weapon
        //        WeaponEntity weaponEntity = characterEntity.FindChild<WeaponEntity>((weaponEntity) =>
        //        {
        //            WeaponComponent weaponComponent = weaponEntity.GetComponent<WeaponComponent>();
        //            if (weaponComponent == null)
        //                return false;

        //            if (weaponComponent.itemInfo.id != itemInfo.id)
        //                return false;

        //            return true;
        //        });

        //        weaponEntity?.Dispose();
        //        GameManager.ECS.SendMessageToEntity(characterEntity.InstanceID, new EntityMessageType.DetachWeapon()).Coroutine();
        //    }

        //    UnityLog.Info($"Take off equipment:{itemInfo}");
        //    return true;
        //}

        //public static long GetAttribute(CharacterAttributeComponent attributeComponent, CharacterAttributeType type)
        //{
        //    attributeComponent.valueMap.TryGetValue(type, out long value);
        //    return value;
        //}

        public static double GetAttributeD(CharacterAttributeComponent attributeComponent, CharacterAttributeType type)
        {
            attributeComponent.valueMap.TryGetValue(type, out long value);
            return value / 1000d;
        }
    }
}
