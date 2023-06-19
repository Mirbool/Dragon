using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.ECS
{
    public class AnimatorComponent : ECSComponent
    {
        public Animator animator;
    }

    public class AnimatorComponentAwakeSystem : AwakeSystem<AnimatorComponent, string>
    {
        public override void Awake(AnimatorComponent c, string p2)
        {
            GameObjectComponent gameObjectComponent = c.Entity.GetComponent<GameObjectComponent>();
            c.animator = gameObjectComponent.gameObject.AddComponent<Animator>();
            if (!string.IsNullOrEmpty(p2))
            {
                c.animator.runtimeAnimatorController = GameManager.Asset.LoadAsset<RuntimeAnimatorController>(p2);
            }
            c.animator.enabled = false;
        }
    }

    public class AnimatorComponentDestroySystem : DestroySystem<AnimatorComponent>
    {
        public override void Destroy(AnimatorComponent c)
        {
            Object.Destroy(c.animator);
            c.animator = null;

            c.animator.enabled = false;
            c.animator.runtimeAnimatorController = null;
            c.animator = null;
        }
    }
}
