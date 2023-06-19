using Config;
using Koakuma.Game.Characters;
using Koakuma.Proto;
using System;
using System.IO;
using TGame;
using TGame.Save;

namespace Koakuma.Game.Save
{
    public static class SaveHelper
    {
        public const string SAVE_FILE_NAME = "game.json";

        public static bool HasProfile()
        {
            string path = Path.Combine(UnityEngine.Application.persistentDataPath, GameManager.Save.directory, SAVE_FILE_NAME);
            return File.Exists(path);
        }

        public static GameInfo CreateNewProfile()
        {
            CharacterInfo playerInfo = new CharacterInfo()
            {
                configID = 1001,
                team = CharacterTeam.Player,
                skills = ListPool<SkillInfo>.Obtain(),
                characterAttributes = DictionaryPool<CharacterAttributeType, long>.Obtain(),
                equipments = ListPool<ItemInfo>.Obtain(),
            };

            CharacterConfig characterConfig = CharacterConfig.ByID(playerInfo.configID);
            foreach (var skillID in characterConfig.Skills)
            {
                playerInfo.skills.Add(new SkillInfo()
                {
                    configID = skillID,
                    level = 1,
                });
            }

            playerInfo.characterAttributes[CharacterAttributeType.Strength] = (long)(characterConfig.Strength * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Intelligence] = (long)(characterConfig.Intelligence * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Dexterity] = (long)(characterConfig.Dexterity * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Vitality] = (long)(characterConfig.Vitality * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.BaseHP] = (long)(characterConfig.Life * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.MaxFury] = (long)(characterConfig.Fury * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Damage] = (long)(characterConfig.Damage * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Critical] = (long)Math.Floor(characterConfig.CriticalHit * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.CriticalDamage] = (long)Math.Floor(characterConfig.CriticalHitDamage * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Evasion] = (long)Math.Floor(characterConfig.Evasion * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Block] = (long)Math.Floor(characterConfig.Block * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.MoveSpeed] = (long)Math.Floor(characterConfig.MS * 1000);
            playerInfo.characterAttributes[CharacterAttributeType.Level] = 1000;

            playerInfo.equipments.Add(new ItemInfo()
            {
                id = IDGenerator.NewID(),
                type = Proto.ItemType.Equipment,
                equipmentInfo = new EquipmentInfo()
                {
                    configID = 1,
                    baseAttributes = new System.Collections.Generic.List<int>() { 0, 0 },
                }
            });

            return new GameInfo()
            {
                kkma = 999 * 1000L,
                bod = 100000 * 1000L,
                playerInfo = playerInfo,
            };
        }

        //public static void Save(Action onFinish = null)
        //{
        //    PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();
        //    Save(playerComponent.gameInfo, onFinish);
        //}

        //public static void Save(object data, Action onFinish = null)
        //{
        //    GameManager.Save.AddTask(new SaveTask()
        //    {
        //        Data = data,
        //        Path = SAVE_FILE_NAME,
        //        OnFinish = onFinish,
        //    });
        //}
    }
}
