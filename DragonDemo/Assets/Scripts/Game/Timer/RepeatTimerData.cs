namespace Koakuma.Game.Timer
{
    public struct RepeatTimerData
    {
        public float delay;
        public int maxCount;
        public float internval;
        public RepeatTimerComponent.TickEventArgs callback;
    }
}
