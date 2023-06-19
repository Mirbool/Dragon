using Koakuma.Game;
using Koakuma.Game.ECS;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame.Message;
using UnityEngine;

public class EnterServerMessageHandler : MessageHandler<MessageType.EnterServer>
{
    public override async Task HandleMessage(MessageType.EnterServer arg)
    {
        GameManager.ECS.World.GetComponent<ServerCommponent>().res = arg.res;
        GameManager.UI.CloseUI(UIViewID.LoginUI);
        GameManager.UI.OpenUI(UIViewID.SelectServerUI);

         //GameManager.Message.Post<MessageType.EnterServer>(new MessageType.EnterServer()).Coroutine();


        await Task.Yield();

    }
}
