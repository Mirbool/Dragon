using Config;
using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.FX
{
    public class FXComponent : ECSComponent
    {
        public int id;
        public FXPlayMode mode;
        public bool followRotationX;
        public bool followRotationY;
        public bool followRotationZ;
        public Transform bindTransform;
        public GameObject fxObject;
        public FXStopListener fxStopListener;
    }

    public class FXComponentAwake : AwakeSystem<FXComponent, FXData>
    {
        public override void Awake(FXComponent c, FXData p1)
        {
            c.id = p1.id;
            c.bindTransform = p1.bindTransform;

            FXConfig fXConfig = FXConfig.ByID(c.id);
            c.mode = fXConfig.PlayMode;
            c.fxObject = GameManager.Asset.LoadGameObject(fXConfig.Asset);
            c.followRotationX = fXConfig.FollowRotationX;
            c.followRotationY = fXConfig.FollowRotationY;
            c.followRotationZ = fXConfig.FollowRotationZ;

            c.fxObject.transform.position = c.bindTransform.position;
            c.fxObject.transform.rotation = c.bindTransform.rotation;

            c.fxObject.SetActive(true);
            c.fxStopListener = c.fxObject.AddComponent<FXStopListener>();
            c.fxStopListener.entityID = c.EntityID;

            ParticleSystem particleSystem = c.fxObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                mainModule.stopAction = ParticleSystemStopAction.Callback;

                particleSystem.Play();
            }
        }
    }

    public class FXComponentDestroy : DestroySystem<FXComponent>
    {
        public override void Destroy(FXComponent c)
        {
            c.id = default;
            c.mode = default;
            c.bindTransform = default;
            c.followRotationX = default;
            c.followRotationY = default;
            c.followRotationZ = default;

            Object.Destroy(c.fxStopListener);
            c.fxStopListener = null;

            GameManager.Asset.UnloadGameObject(c.fxObject);
            c.fxObject = null;
        }
    }

    public class FXComponentUpdateSystem : LateUpdateSystem<FXComponent>
    {
        public override void LateUpdate(ECSEntity entity)
        {
            FXComponent fxComponent = entity.GetComponent<FXComponent>();
            if (fxComponent.mode == FXPlayMode.AtPosition)
                return;

            fxComponent.fxObject.transform.position = fxComponent.bindTransform.position;
            if (!fxComponent.followRotationX && !fxComponent.followRotationY && !fxComponent.followRotationZ)
            {
                fxComponent.fxObject.transform.rotation = Quaternion.identity;
            }
            else
            {
                Vector3 rotationEulerAngles = fxComponent.bindTransform.rotation.eulerAngles;
                if (!fxComponent.followRotationX)
                {
                    rotationEulerAngles.x = 0;
                }
                if (!fxComponent.followRotationY)
                {
                    rotationEulerAngles.y = 0;
                }
                if (!fxComponent.followRotationZ)
                {
                    rotationEulerAngles.z = 0;
                }
                fxComponent.fxObject.transform.rotation = Quaternion.Euler(rotationEulerAngles);
            }
        }
    }

    public class FXStopListener : MonoBehaviour
    {
        public long entityID;

        private void OnParticleSystemStopped()
        {
            ECSEntity fxEntity = GameManager.ECS.FindEntity(entityID);
            fxEntity?.Dispose();
        }
    }
}
