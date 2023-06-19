using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using Koakuma.Game;
using Koakuma.Game.Scenes;
using System.Threading.Tasks;
using TGame.Procedure;
using UnityEngine;
using UnityEngine.AddressableAssets;

using System;
using Koakuma.Game.Inputs;
using Koakuma.Game.Skills;
//using Koakuma.Game.Procedure;

/// <summary>
/// 大厅进度
/// </summary>
public class LobbyProcedure : BaseProcedure
{
    private CharacterEntity playerEntity;

    public override async Task OnEnterProcedure(object value)
    {
        UnityLog.Info("大厅阶段");

        GameManager.Message.Subscribe<MessageType.EnterDungeon>(OnEnterDungeon);

        await OnLobbySceneCreating();

        await SceneFactory.TeleportPlayerToNewScene(new CreateSceneData()
        {
            showLoading = true,
            loadingTask = async () =>
            {
                await Addressables.LoadSceneAsync("Assets/Scenes/Lobby.unity", UnityEngine.SceneManagement.LoadSceneMode.Single);

                //GameObject sceneSettingObject = GameObject.FindGameObjectWithTag("SceneSetting");
                //if (sceneSettingObject != null)
                //{
                //    LobbySetting lobbySetting = sceneSettingObject.GetComponent<SceneSetting>() as LobbySetting;

                //    GameObjectComponent gameObjectComponent = playerEntity.GetComponent<GameObjectComponent>();
                //    gameObjectComponent.position = lobbySetting.characterPoint.position;
                //    gameObjectComponent.rotation = lobbySetting.characterPoint.rotation;
                //}

                GameManager.UI.OpenUI(UIViewID.SelectRoleUI);
                //GameManager.UI.CloseUI(UIViewID.LoginUI);
                GameManager.UI.CloseUI(UIViewID.SelectServerUI);
                ///  await GameManager.Message.Post(new MessageType.EnterLobby());

                await new WaitForSeconds(0.5f);
            },
        });


        await Task.Yield();
    }

    private async Task OnEnterDungeon(MessageType.EnterDungeon arg)
    {
        CharacterEntity playerEntity = CharacterHelper.GetPlayerEntity();
        playerEntity.AddComponent<SkillComponent>();
        //playerEntity.AddComponent<BuffComponent>();
        playerEntity.AddComponent<InputComponent>();

        ChangeProcedure<DungeonProcedure>().Coroutine();
        await Task.Yield();
    }



    private async Task OnLobbySceneCreating()
    {

        PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();

        CharacterData characterData = new CharacterData()
        {
            characterInfo = playerComponent.gameInfo.playerInfo,
            scene = GameManager.ECS.World.GameScene.InstanceID,
            useHQ = true,
        };

        playerEntity = CharacterFactory.CreatePlayer(characterData);
        playerComponent.playerEntityID = playerEntity.InstanceID;

        if (playerEntity != null)
        {
            GameManager.ECS.SendMessageToEntity(playerEntity.InstanceID, new EntityMessageType.CharacterVitalize()).Coroutine();
        }
        await Task.Yield();
    }

    public override Task OnLeaveProcedure()
    {
        GameManager.Message.Unsubscribe<MessageType.EnterDungeon>(OnEnterDungeon);

        return base.OnLeaveProcedure();
    }
}

