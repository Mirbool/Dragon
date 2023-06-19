using Koakuma.Game.ECS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Scenes
{
    public static class SceneFactory
    {
        public static async Task SwitchScene(CreateSceneData data)
        {
          //  GameManager.ECS.SendMessageToEntity(GameManager.ECS.World.InstanceID, new EntityMessageType.SetGameSceneLoadingState() { isLoading = true }).Coroutine();

            await GameManager.Message.Post(new MessageType.GameSceneCreating()
            {
                data = data,
            });

            ECSScene oldGameScene = GameManager.ECS.World.GameScene;
            ECSScene newScene = new ECSScene();

            // 销毁Old Scene
            oldGameScene?.Dispose();

            GameManager.ECS.World.GameScene = newScene;

            if (data.loadingTask != null)
            {
                await data.loadingTask();  ///lobby.scene
            }

            await GameManager.Message.Post(new MessageType.GameSceneCreated()
            {
                data = data,
            });

         //   GameManager.ECS.SendMessageToEntity(GameManager.ECS.World.InstanceID, new EntityMessageType.SetGameSceneLoadingState() { isLoading = false }).Coroutine();
        }

 

        public static async Task TeleportPlayerToNewScene(CreateSceneData data)
        {
            //GameManager.ECS.SendMessageToEntity(GameManager.ECS.World.InstanceID, new EntityMessageType.SetGameSceneLoadingState() { isLoading = true }).Coroutine();

            await GameManager.Message.Post(new MessageType.GameSceneCreating()
            {
                data = data,
            });

            ECSScene oldGameScene = GameManager.ECS.World.GameScene;
            ECSScene newScene = new ECSScene();

            // 传送Entity
            {
                PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();
                List<long> teleportEntities = ListPool<long>.Obtain();
                teleportEntities.Add(playerComponent.playerEntityID);
                foreach (var teleportEntityID in teleportEntities)
                {
                    ECSEntity teleportEntity = GameManager.ECS.FindEntity(teleportEntityID);
                    if (teleportEntity == null)
                        continue;

                    oldGameScene?.RemoveEntity(teleportEntityID);
                    newScene.AddEntity(teleportEntity);
                }
                ListPool<long>.Release(teleportEntities);
            }

            // 销毁Old Scene
            oldGameScene?.Dispose();

            GameManager.ECS.World.GameScene = newScene;

            if (data.loadingTask != null)
            {
                await data.loadingTask();
            }

            await GameManager.Message.Post(new MessageType.GameSceneCreated()
            {
                data = data,
            });

           // GameManager.ECS.SendMessageToEntity(GameManager.ECS.World.InstanceID, new EntityMessageType.SetGameSceneLoadingState() { isLoading = false }).Coroutine();
        }

        public static void DisposeGameScene()
        {
            GameManager.ECS.World.GameScene?.Dispose();
            GameManager.ECS.World.GameScene = new ECSScene();
        }

        //public static ECSEntity CreateSceneObject(CreateSceneObjectData createSceneObjectData)
        //{
        //    if (createSceneObjectData.objectMono == null)
        //        return null;

        //    ECSEntity sceneObjectEntity = new ECSEntity();
        //    GameObjectComponent goComponent = sceneObjectEntity.AddComponent<GameObjectComponent, GameObject>(createSceneObjectData.objectMono.gameObject);
        //    goComponent.isStatic = true;
        //    switch (createSceneObjectData.objectMono.SceneObjectType)
        //    {
        //        case SceneObjectType.Grass:
        //            sceneObjectEntity.AddComponent<GrassComponent, GrassObjectMono>(createSceneObjectData.objectMono as GrassObjectMono);
        //            break;
        //        default:
        //            UnityLog.Error($"Unsupported scene object type:{createSceneObjectData.objectMono.SceneObjectType}");
        //            break;
        //    }

        //    createSceneObjectData.objectMono.entityID = sceneObjectEntity.InstanceID;
        //    return sceneObjectEntity;
        //}
    }
}
