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
using Config;
using XMainClient;
using XUtliPoolLib;
using System.Collections.Specialized;
using TGame.UI;
//using Koakuma.Game.Procedure;

/// <summary>
/// 大厅进度
/// </summary>
public class LobbyProcedure : BaseProcedure
{
    private CharacterEntity playerEntity;
    private int crtRoleIndex;

    public override async Task OnEnterProcedure(object value)
    {
        UnityLog.Info("大厅阶段");
        crtRoleIndex = (int)value;
        //GameManager.Message.Subscribe<MessageType.EnterDungeon>(OnEnterDungeon);

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


                GameManager.UI.OpenUI(UIViewID.MainCityUI);
                GameManager.UI.CloseUI(UIViewID.SelectRoleUI);


               
               await new WaitForSeconds(0.5f);
            },
        });
        await OnLobbySceneCreating();



        await Task.Yield();
    }


    //private async Task OnEnterDungeon(MessageType.EnterDungeon arg)
    //{
    //    CharacterEntity playerEntity = CharacterHelper.GetPlayerEntity();
    //    playerEntity.AddComponent<SkillComponent>();
    //    //playerEntity.AddComponent<BuffComponent>();
    //    playerEntity.AddComponent<InputComponent>();

    //    ChangeProcedure<DungeonProcedure>().Coroutine();
    //    await Task.Yield();
    //}



    private async Task OnLobbySceneCreating()
    {

        PlayerComponent pm = GameManager.ECS.World.GetComponent<PlayerComponent>();
        Role role = new Role();
        RoleInfoConfig data = RoleInfoConfig.ByIndex(crtRoleIndex);
        role.Init(crtRoleIndex, data.Prefabs, data.PartConfigData, data.Animation, data.WeaponPoint);
        role.RoleGo.transform.position = Vector3.zero;
        pm.crtSelectRole = role;
        await Task.Yield();


    }

    public override Task OnLeaveProcedure()
    {
        //GameManager.Message.Unsubscribe<MessageType.EnterDungeon>(OnEnterDungeon);

        return base.OnLeaveProcedure();
    }
}

