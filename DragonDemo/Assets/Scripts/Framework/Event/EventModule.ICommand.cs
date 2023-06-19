namespace TGame.Event
{
    /// <summary>
    /// 作者: Teddy
    /// 时间: 2018/03/07
    /// 功能: 
    /// </summary>
	public partial class EventModule : BaseGameModule
    {
        internal interface ICommand
        {
            void Do();
        }
    }
}