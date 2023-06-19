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
    /// ��¼�����̵Ĵ���
    /// </summary>
    public class LoginMessageHandler : MessageHandler<MessageType.Login>
    {
        public override async Task HandleMessage(MessageType.Login arg)
        {

            GameInfo gameInfo = SaveHelper.CreateNewProfile();
            UnityLog.Info(gameInfo.playerInfo.configID.ToString());

            ///�ѵ�¼��ȡ���ķ��������߱��ص�������ݣ�������������
            PlayerComponent playerComponent = GameManager.ECS.World.GetComponent<PlayerComponent>();
            playerComponent.gameInfo = gameInfo;


           // GameManager.Message.Post<MessageType.EnterServer>(new MessageType.EnterServer()).Coroutine();

            await Task.Yield();
      
        }
    }




}
