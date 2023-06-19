namespace TGame.Event
{
    /// <summary>
    /// 作者: Teddy
    /// 时间: 2018/08/24
    /// 功能: 
    /// </summary>
    public class CommonEventArgs<T> : BaseEventType
    {
        public T Data { get; private set; }

        public CommonEventArgs(T data)
        {
            Data = data;
        }
    }
}