using Config;
using Koakuma.Game;
using Koakuma.Game.Cameras;
//using Koakuma.Game.Cameras;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using Koakuma.Game.GameCamera;
//using Koakuma.Game.GameCamera;
using Koakuma.Game.Scenes;
using Koakuma.Game.Scenes.Dungeons;
//using Koakuma.Game.Scenes.Dungeons;
using System.Threading.Tasks;
using TGame.ECS;
using TGame.Procedure;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class DungeonProcedure : BaseProcedure
{
    private int dungeonID = 1001;
    private int dungeonMapIndex;

    public override async Task OnEnterProcedure(object value)
    {
        dungeonMapIndex = 0;
        ///  GameManager.UI.OpenUI(UIViewID.BattleUI);

        GameManager.Message.Subscribe<MessageType.ExitDungeon>(OnExitDungeon);
        GameManager.Message.Subscribe<MessageType.EnterNextDungeon>(OnEnterNextDungeon);
        GameManager.Message.Subscribe<MessageType.PlayerDead>(OnPlayerDead);

        // show loading ui
        GameManager.Audio.StopBGM();

        // teleport entities
        await SceneFactory.TeleportPlayerToNewScene(new CreateSceneData()
        {
            showLoading = true,
            loadingTask = async () =>
            {
                // 主角模型更新到低模
                GameManager.ECS.SendMessageToEntity(CharacterHelper.GetPlayerEntity().InstanceID, new EntityMessageType.ChangeHQModel()
                {
                    useHQ = false,
                }).Coroutine();


                await LoadDungeon();
            }
        });
    }

    public override async Task OnLeaveProcedure()
    {
        GameManager.UI.CloseUI(UIViewID.BattleUI);

        GameManager.Message.Unsubscribe<MessageType.ExitDungeon>(OnExitDungeon);
        GameManager.Message.Unsubscribe<MessageType.EnterNextDungeon>(OnEnterNextDungeon);
        GameManager.Message.Unsubscribe<MessageType.PlayerDead>(OnPlayerDead);

        // 主角模型更新到高模
        //GameManager.ECS.SendMessageToEntity(CharacterHelper.GetPlayerEntity().InstanceID, new EntityMessageType.ChangeHQModel()
        //{
        //    useHQ = true,
        //});

        await Task.Yield();
    }

    private async Task LoadDungeon()
    {
        // load scene
        DungeonConfig dungeonConfig = DungeonConfig.ByID(dungeonID);
        int dungeonMapID = dungeonConfig.MapList[dungeonMapIndex];
        DungeonMapConfig dungeonMapConfig = DungeonMapConfig.ByID(dungeonMapID);

        await Addressables.LoadSceneAsync(dungeonMapConfig.Scene, UnityEngine.SceneManagement.LoadSceneMode.Single);

        GameManager.UI.CloseUI(UIViewID.LobbyUI);

        GameObject sceneSettingObject = GameObject.FindGameObjectWithTag("SceneSetting");
        if (sceneSettingObject != null)
        {
            DungeonSetting dungeonSetting = sceneSettingObject.GetComponent<DungeonSetting>();

            CharacterEntity playerEntity = CharacterHelper.GetPlayerEntity();

            // 更新主角位置
            GameObjectComponent gameObjectComponent = playerEntity.GetComponent<GameObjectComponent>();
            gameObjectComponent.position = dungeonSetting.entrancePoint.position;
            gameObjectComponent.rotation = dungeonSetting.entrancePoint.rotation;

            // 绑定相机
            GameManager.ECS.World.GameScene.AddComponent<CameraComponent, CameraSetting>(dungeonSetting.cameraSetting);

            await new WaitForSeconds(0.5f);
        }
    }

    private async Task OnExitDungeon(MessageType.ExitDungeon arg)
    {
        UnityLog.Info("on exit dungeon");
        SceneFactory.DisposeGameScene();
        await ChangeProcedure<LobbyProcedure>();
    }

    public async Task OnEnterNextDungeon(MessageType.EnterNextDungeon arg)
    {
        UnityLog.Info("on enter next dungeon");
        dungeonMapIndex++;
        ECSEntity playerEntity = CharacterHelper.GetPlayerEntity();
        if (playerEntity == null)
            return;

        await GameManager.Message.Post(new MessageType.SetInputEnableState() { enabled = false });

        GameManager.ECS.SendMessageToEntity(playerEntity.InstanceID, new EntityMessageType.StopMove()).Coroutine();
        //GameManager.ECS.SendMessageToEntity(playerEntity.ID, new EntityMessageType.EndSkillCasting());

        //  await CameraHelper.TweenColor(Color.black, 1);

        await SceneFactory.TeleportPlayerToNewScene(new CreateSceneData()
        {
            showLoading = false,
            loadingTask = async () =>
            {
                await LoadDungeon();
            }
        });

        await GameManager.Message.Post(new MessageType.SetInputEnableState() { enabled = true });
    }

    private async Task OnPlayerDead(MessageType.PlayerDead arg)
    {
        await new WaitForSeconds(1);
        await ChangeProcedure<LobbyProcedure>();
    }
}

