using Koakuma.Game;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
//using Koakuma.Game.Items.Drop;
using Koakuma.Game.Save;
using Koakuma.Proto;
using QFSW.QC;
using System.Collections.Generic;
using TGame.ECS;
using UnityEngine;

public static class ConsoleUtility
{
    [Command("Drop.FromPool")]
    public static void DropFromPool(int pool)
    {
        //ECSEntity playerEntity = CharacterHelper.GetPlayerEntity();
        //GameObjectComponent gameObjectComponent = playerEntity.GetComponent<GameObjectComponent>();

        //Vector3 position = gameObjectComponent.position + Quaternion.Euler(0, Random.Range(0f, 360f), 0) * Vector3.forward * 2;

        //List<ItemInfo> items = new List<ItemInfo>();
        //DropHelper.DropRewardFromPool(pool, items);
        //foreach (var item in items)
        //{
        //    DropFactory.CreateDropItem(new DropItemInfo()
        //    {
        //        position = position,
        //        itemInfo = item,
        //    });
        //}
    }

    [Command("Drop.Equipment")]
    public static void DropEquipment(int equipmentConfigID)
    {
     //   DropHelper.DropEquipment(equipmentConfigID);
    }

    [Command("Suicide")]
    public static void Suicide()
    {
        GameManager.ECS.SendMessageToEntity(CharacterHelper.GetPlayerEntity().InstanceID, new EntityMessageType.ChangeAttribute()
        {
            type = CharacterAttributeType.CurrentHP,
            valueChange = -999999,
        }).Coroutine();
    }

    [Command("Save")]
    public static void Save()
    {
       // SaveHelper.Save();
    }

    [Command("Add.KKMA")]
    public static void AddKKMA(decimal addValue)
    {
        GameManager.Message.Post(new MessageType.AddResource()
        {
            type = Koakuma.Game.Items.ResourceType.KKMA,
            value = (long)(addValue * 1000L),
        }).Coroutine();
    }

    [Command("Add.BOD")]
    public static void AddBOD(decimal addValue)
    {
        GameManager.Message.Post(new MessageType.AddResource()
        {
            type = Koakuma.Game.Items.ResourceType.BOD,
            value = (long)(addValue * 1000L),
        }).Coroutine();
    }

    [Command("Add.Exp")]
    public static void AddExp(decimal addValue)
    {
        CharacterEntity playerEntity = CharacterHelper.GetPlayerEntity();
        GameManager.ECS.SendMessageToEntity(playerEntity.InstanceID, new EntityMessageType.AddExp()
        {
            valueChange = (long)(addValue * 1000L),
        }).Coroutine();
    }


 
}
