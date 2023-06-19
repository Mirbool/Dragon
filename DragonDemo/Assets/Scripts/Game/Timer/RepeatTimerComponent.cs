using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Timer
{
    public class RepeatTimerComponent : ECSComponent
    {
        public delegate void TickEventArgs(int count);

        public int maxCount;
        public float internval;
        public int count;
        public float nextTickTime;
        public TickEventArgs callback;
    }

    public class RepeatTimerComponentAwake : AwakeSystem<RepeatTimerComponent, RepeatTimerData>
    {
        public override void Awake(RepeatTimerComponent c, RepeatTimerData p1)
        {
            c.maxCount = p1.maxCount;
            c.internval = p1.internval;
            c.count = 0;
            c.nextTickTime = p1.delay + Time.time;
            c.callback = p1.callback;
        }
    }

    public class RepeatTimerComponentDestroy : DestroySystem<RepeatTimerComponent>
    {
        public override void Destroy(RepeatTimerComponent c)
        {
            c.maxCount = default;
            c.internval = default;
            c.count = default;
            c.nextTickTime = default;
            c.callback = null;
        }
    }

    public class RepeatTimerComponentUpdate : UpdateSystem<RepeatTimerComponent>
    {
        public override void Update(ECSEntity entity)
        {
            RepeatTimerComponent repeatTimerComponent = entity.GetComponent<RepeatTimerComponent>();
            if (repeatTimerComponent.count >= repeatTimerComponent.maxCount && repeatTimerComponent.maxCount != -1)
            {
                entity.Dispose();
                return;
            }

            float time = Time.time;
            while (time >= repeatTimerComponent.nextTickTime)
            {
                repeatTimerComponent.callback?.Invoke(++repeatTimerComponent.count);
                repeatTimerComponent.nextTickTime += repeatTimerComponent.internval;

                if (repeatTimerComponent.count >= repeatTimerComponent.maxCount && repeatTimerComponent.maxCount != -1)
                {
                    break;
                }
            }
        }
    }
}
