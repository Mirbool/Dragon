using Koakuma.Game.ECS;
using Koakuma.Game.Save;
using Koakuma.Proto;
using Mono.CSharp;
using System.Threading.Tasks;
using TGame.Message;
using TGame.Save;

namespace Koakuma.Game.Login
{
    /// <summary>
    /// 登录的流程的处理，
    /// </summary>
    public class LoginMessageHandler : MessageHandler<MessageType.Login>
    {
        public override async Task HandleMessage(MessageType.Login arg)
        {

            GameInfo gameInfo = SaveHelper.CreateNewProfile();
            UnityLog.Info(gameInfo.playerInfo.configID.ToString());

            ///把登录获取到的服务器或者本地的玩家数据，存入玩家组件。
            PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();
            playerComponent.gameInfo = gameInfo;


           // GameManager.Message.Post<MessageType.EnterServer>(new MessageType.EnterServer()).Coroutine();

            await Task.Yield();
      
        }
    }




}
