using System;
using System.Collections.Generic;

namespace TGame.Event
{
    /// <summary>
    /// 作者: Teddy
    /// 时间: 2018/03/07
    /// 功能: 
    /// </summary>
	public partial class EventModule : BaseGameModule
    {
        internal class FireCommand : ICommand
        {
            private Type messageType;
            private BaseEventType arg;

            public FireCommand(Type messageType, BaseEventType arg)
            {
                this.messageType = messageType;
                this.arg = arg;
            }

            void ICommand.Do()
            {
                List<EventHandler> handlers = ListPool<EventHandler>.Obtain();
                handlers.AddRangeNonAlloc(TGameFramework.Instance.GetModule<EventModule>().FindHandlerList(messageType));
                foreach (var handler in handlers)
                {
                    handler?.Invoke(arg);
                }
                ListPool<EventHandler>.Release(handlers);
            }
        }
    }
}