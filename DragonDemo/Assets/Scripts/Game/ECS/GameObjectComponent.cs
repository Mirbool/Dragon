using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.ECS
{
    public class GameObjectComponent : ECSComponent
    {
        public GameObject gameObject;
        public Rigidbody rigidbody;
        public Quaternion rotation;
        public Transform fxBindPoint;
        public Transform transform => gameObject.transform;
        public Vector3 position { get => transform.position; set => transform.position = value; }
        public Vector3 forward => transform.forward;
        public bool isStatic;
    }

    public class GameObjectAwakeComponent : AwakeSystem<GameObjectComponent, GameObject>
    {
        public override void Awake(GameObjectComponent c, GameObject p1)
        {
            c.gameObject = p1;
            c.rigidbody = c.gameObject.GetComponent<Rigidbody>();
            c.fxBindPoint = c.gameObject.transform.FindChild<Transform>("VFX", 0);
            c.rotation = p1.transform.rotation;
            c.isStatic = false;
        }
    }

    public class GameObjectDestroyComponent : DestroySystem<GameObjectComponent>
    {
        public override void Destroy(GameObjectComponent c)
        {
            GameManager.Asset.UnloadGameObject(c.gameObject);
            c.gameObject = null;

            c.rigidbody = null;
        }
    }

    public class GameObjectUpdateSystem : UpdateSystem<GameObjectComponent>
    {
        public override void Update(ECSEntity entity)
        {
            GameObjectComponent gameObjectComponent = entity.GetComponent<GameObjectComponent>();
            if (gameObjectComponent == null)
                return;

            if (gameObjectComponent.isStatic)
                return;

            gameObjectComponent.transform.rotation = Quaternion.RotateTowards(gameObjectComponent.transform.rotation, gameObjectComponent.rotation, 1000 * Time.deltaTime);
        }
    }
}
