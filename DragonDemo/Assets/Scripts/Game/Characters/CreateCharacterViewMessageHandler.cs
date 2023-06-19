using Config;
using Koakuma.Game.ECS;
using System.Threading.Tasks;
using TGame.Message;
using UnityEngine;

namespace Koakuma.Game.Characters
{
    public class CreateCharacterViewMessageHandler : MessageHandler<MessageType.CreateCharacterView>
    {
        public override async Task HandleMessage(MessageType.CreateCharacterView arg)
        { 
            CharacterComponent characterComponent = arg.character.GetComponent<CharacterComponent>();
            CharacterConfig characterConfig = CharacterConfig.ByID(characterComponent.configID);

            string characterModelName = arg.characterData.characterInfo.team == CharacterTeam.Player ? "Assets/BundleAssets/Model/Character.prefab" : "Assets/BundleAssets/Model/Monster.prefab";
            GameObject characterModel = GameManager.Asset.LoadGameObject(characterModelName);
            int layer = arg.characterData.characterInfo.team == CharacterTeam.Player ? LayerMaskUtility.PLAYER_LAYER : LayerMaskUtility.MONSTER_LAYER;
            characterModel.layer = layer;

            GameObjectComponent gameObjectComponent = arg.character.AddComponent<GameObjectComponent, GameObject>(characterModel);
            gameObjectComponent.position = arg.characterData.position;
            gameObjectComponent.rotation = arg.characterData.rotation;

            //if (!arg.characterData.useHQ)
            //{
            //    string animator = arg.characterData.useHQ ? characterConfig.AnimatorHQ : characterConfig.Animator;
            //    arg.character.AddComponent<AnimatorComponent, string>(animator);
            //}

            //arg.character.AddComponent<CharacterViewComponent, CharacterViewData>(new CharacterViewData()
            //{
            //    configID = arg.characterData.characterInfo.configID,
            //    team = arg.characterData.characterInfo.team,
            //    model = arg.characterData.useHQ ? characterConfig.ModelHQ : characterConfig.Model,
            //});

            await Task.Yield();
        }
    }
}
