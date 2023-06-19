//using Koakuma.Game.AI;
using Koakuma.Game.ECS;
//using Koakuma.Game.PathFinding;
//using Koakuma.Game.Skills;
using TGame.ECS;

namespace Koakuma.Game.Characters
{
    public static class CharacterFactory
    {
        public static CharacterEntity CreatePlayer(CharacterData characterData)
        {
            ECSScene scene = GameManager.ECS.FindEntity(characterData.scene) as ECSScene;
            if (scene == null)
            {
                UnityLog.Error($"Scene not found, id:{characterData.scene}");
                return default;
            }

            CharacterEntity characterEntity = new CharacterEntity();

            characterEntity.AddComponent<CharacterComponent, CharacterData>(characterData);
            CharacterAttributeComponent attributeComponent = characterEntity.AddComponent<CharacterAttributeComponent>();

            // attribute
            foreach (var item in characterData.characterInfo.characterAttributes)
            {
                attributeComponent.valueMap[item.Key] = item.Value;
            }

            characterEntity.AddComponent<CharacterMoveComponent>();

            scene.AddEntity(characterEntity);
            GameManager.Message.Post(new MessageType.CreateCharacterView() { character = characterEntity, characterData = characterData }).Coroutine();

            //// equipment
            //if (characterData.characterInfo.equipments != null)
            //{
            //    foreach (var itemInfo in characterData.characterInfo.equipments)
            //    {
            //        CharacterHelper.WearEquipmentAttribute(characterEntity, attributeComponent, itemInfo);
            //    }
            //}

            return characterEntity;
        }

        public static CharacterEntity CreateMonster(CharacterData characterData)
        {
            ECSScene scene = GameManager.ECS.FindEntity(characterData.scene) as ECSScene;
            if (scene == null)
            {
                UnityLog.Error($"Scene not found, id:{characterData.scene}");
                return default;
            }

            CharacterEntity characterEntity = new CharacterEntity();

            //characterEntity.AddComponent<CharacterComponent, CharacterData>(characterData);
            //characterEntity.AddComponent<BuffComponent>();
            //characterEntity.AddComponent<HatredComponent>();
            //characterEntity.AddComponent<SkillComponent>();
            //characterEntity.AddComponent<AIComponent>();

            //CharacterAttributeComponent attributeComponent = characterEntity.AddComponent<CharacterAttributeComponent>();

            //// attribute
            //foreach (var item in characterData.characterInfo.characterAttributes)
            //{
            //    attributeComponent.valueMap[item.Key] = item.Value;
            //}

            //   characterEntity.AddComponent<CharacterMoveComponent>();
            //characterEntity.AddComponent<PathfindingComponent>();

            //scene.AddEntity(characterEntity);
            //GameManager.Message.Post(new MessageType.CreateCharacterView() { character = characterEntity, characterData = characterData }).Coroutine();

            return characterEntity;
        }
    }
}
