using Cinemachine;
//using FMODUnity;
using Koakuma.Game.Characters;
using Koakuma.Game.ECS;
using Koakuma.Game.GameCamera;
//using Koakuma.Game.GameCamera;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Cameras
{
    public class CameraComponent : ECSComponent
    {
        public CinemachineBrain brain;
        public CinemachineVirtualCamera mainCamera;
        //public StudioListener listener;
        public CinemachineImpulseListener impulseListener;
        private GameObject listenerAttenuationObject;

        //public GameObject mask;
        //public Camera uiCamera;
        //public PostProcessVolume mainPPV;
        //private Material maskMaterial;
        //private CinemachineTransposer bodyTransposer;
    }

    public class CameraComponentAwakeSystem : AwakeSystem<CameraComponent, CameraSetting>
    {
        public override void Awake(CameraComponent c, CameraSetting p1)
        {
            c.brain = p1.brain;
            c.mainCamera = p1.mainCamera;
          //  c.listener = p1.listener;
            c.impulseListener = p1.impulseListener;

            CharacterEntity characterEntity = CharacterHelper.GetPlayerEntity();
            GameObjectComponent gameObjectComponent = characterEntity.GetComponent<GameObjectComponent>();
            if (gameObjectComponent != null)
            {
                c.mainCamera.Follow = gameObjectComponent.transform;
                c.mainCamera.LookAt = gameObjectComponent.transform;
                //  c.listener.attenuationObject = gameObjectComponent.gameObject;
            }

            c.mainCamera.UpdateCameraState(Vector3.up, 5);
        }
    }

    public class CameraComponentDestroySystem : DestroySystem<CameraComponent>
    {
        public override void Destroy(CameraComponent c)
        {
            c.brain = default;
            c.mainCamera = default;
         //  c.listener = default;
            c.impulseListener = default;
        }
    }
}
