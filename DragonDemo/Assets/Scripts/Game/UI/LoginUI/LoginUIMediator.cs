using Koakuma.Game;
using RootMotion;
using System;
using System.Threading.Tasks;
using TGame.UI;
using UnityEngine.UI;
using XMainClient;
using XUtliPoolLib;

public class LoginUIMediator : UIMediator<LoginUIView>
{
    protected override void OnInit(LoginUIView view)
    {
        base.OnInit(view);

    }

    private void loginBtnClickCallback()
    {
        UnityLog.Info("登录");

        string acc = view.accTxt.text;
        string psw = view.pswTxt.text;

        XSingleton<XClientNetwork>.singleton.Authorization(KKSG.LoginType.LOGIN_PASSWORD, acc, psw, acc);
        ///处理登录请求
        GameManager.Message.Post<MessageType.Login>(new MessageType.Login()).Coroutine();
    }

    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        view.loginBtn.onClick.AddListener(loginBtnClickCallback);
        GameManager.Message.Subscribe<MessageType.LoginResp>(onRespHandler);
    }

    private async Task onRespHandler(MessageType.LoginResp arg)
    {
        // 切换选服进度
        GameManager.UI.OpenUI(UIViewID.SelectRoleUI);
        GameManager.UI.CloseUI(UIViewID.SelectServerUI);


        //GameManager.Procedure.ChangeProcedure<LobbyProcedure>().Coroutine();

        await Task.Yield();
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        
    }


    protected override void OnHide()
    {
        view.loginBtn.onClick.RemoveListener(loginBtnClickCallback);


        base.OnHide();

    }


}
