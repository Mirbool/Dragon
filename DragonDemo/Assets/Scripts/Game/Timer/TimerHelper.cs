using Koakuma.Game.Timer;
using TGame.ECS;

namespace Koakuma.Game.Times
{
    public static class TimerHelper
    {
        public static void CreateCountdown(long parentEntityID, CountdownTimerData data)
        {
            ECSEntity parentEntity = GameManager.ECS.FindEntity(parentEntityID);
            if (parentEntity == null)
            {
                UnityLog.Error($"Create countdown fail, parent not found:{parentEntityID}");
                return;
            }

            TimerEntity timerEntity = new TimerEntity();
            timerEntity.AddComponent<CountdownTimerComponent, CountdownTimerData>(data);

            parentEntity.AddChild(timerEntity);
        }

        public static long CreateRepeat(long parentEntityID, RepeatTimerData data)
        {
            if (data.internval == 0)
            {
                UnityLog.Error($"Create repeat timer fail, interval must be set");
                return 0;
            }

            if (data.maxCount <= 0 && data.maxCount != -1)
            {
                UnityLog.Error($"Create repeat timer fail, maxCount must greater than 0 or equal to -1(infinite)");
                return 0;
            }

            ECSEntity parentEntity = GameManager.ECS.FindEntity(parentEntityID);

            if (parentEntity == null)
            {
                UnityLog.Error($"Create countdown fail, parent not found:{parentEntityID}");
                return 0;
            }

            TimerEntity timerEntity = new TimerEntity();
            timerEntity.AddComponent<RepeatTimerComponent, RepeatTimerData>(data);
            parentEntity.AddChild(timerEntity);

            return timerEntity.InstanceID;
        }

        public static void DestroyRepeat(long timerID)
        {
            ECSEntity timerEntity = GameManager.ECS.FindEntity(timerID);
            timerEntity?.Dispose();
        }
    }
}
