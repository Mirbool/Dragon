using TGame.ECS;
using UnityEngine;

namespace Koakuma.Game.Timer
{
    public class CountdownTimerComponent : ECSComponent
    {
        public delegate void CountdownEventArgs(bool canceled);

        public float delay;
        public bool canceled;
        public CountdownEventArgs callback;
    }

    public class CountdownComponentAwakeSystem : AwakeSystem<CountdownTimerComponent, CountdownTimerData>
    {
        public override void Awake(CountdownTimerComponent c, CountdownTimerData p1)
        {
            c.delay = p1.delay;
            c.callback = p1.callback;
            c.canceled = true;
        }
    }

    public class CountdownComponentDestroySystem : DestroySystem<CountdownTimerComponent>
    {
        public override void Destroy(CountdownTimerComponent c)
        {
            c.delay = default;

            CountdownTimerComponent.CountdownEventArgs cb = c.callback;
            cb?.Invoke(c.canceled);
            c.callback = default;
            c.canceled = default;
        }
    }

    public class CountdownComponentUpdateSystem : UpdateSystem<CountdownTimerComponent>
    {
        public override void Update(ECSEntity entity)
        {
            CountdownTimerComponent c = entity.GetComponent<CountdownTimerComponent>();
            c.delay -= Time.deltaTime;
            if (c.delay <= 0)
            {
                c.canceled = false;
                entity.Dispose();
            }
        }
    }
}