using KKSG;
using Koakuma.Game;
using Koakuma.Game.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame.UI;
using UnityEngine;
using XMainClient;
using XUtliPoolLib;

public class SelectServerUIMediator : UIMediator<SelectServerUIView>
{
    public List<LoginGateData> serverData;
    private QueryGateRes data;
    protected override void OnInit(SelectServerUIView view)
    {
        base.OnInit(view);
        serverData = GameManager.ECS.World.GetComponent<ServerCommponent>().res.allservers;
        data = GameManager.ECS.World.GetComponent<ServerCommponent>().res;
    }
    protected override void OnShow(object arg)
    {
        base.OnShow(arg);

        view.showTxt.text = serverData[0].servername+ serverData[0].zonename;
        view.server1.text= serverData[1].servername;
        view.server2.text= serverData[2].servername;
        //view.selectBtn.onClick.AddListener();
        view.btn1.onClick.AddListener(OnBtn1Click); 
        view.btn2.onClick.AddListener(OnBtn1Click);

        view.confirmBtn.onClick.AddListener(OnConfirmBtnClick);

    }
    public void OnConfirmBtnClick()
    {
        //ServerInfo info = new ServerInfo();
        //info.ServerID = data.RecommandGate.serverid;
        //info.Port = (uint)data.RecommandGate.port;
        //info.ServerName = data.RecommandGate.servername;

        //info.ZoneName = data.RecommandGate.zonename;
        //info.Ip= data.RecommandGate.ip;
        //info.State =(int) data.RecommandGate.state;

        //info.Level = data.RecommandGate.backflowlevel;


        //XSingleton<XClientNetwork>.singleton.OnAuthorized(null,data);

        XSingleton<XClientNetwork>.singleton.Login();
        //GameManager.UI.CloseUI(UIViewID.SelectServerUI);
        //GameManager.UI.OpenUI(UIViewID.SelectRoleUI);
    }


    public void OnBtn1Click()
    {
        
        view.showTxt.text = serverData[1].servername + serverData[1].zonename;
        view.selectTip.SetActive(false);
    }
    public void OnBtn2Click()
    {
        view.showTxt.text = serverData[2].servername + serverData[2].zonename;
        view.selectTip.SetActive(false);

    }


}
