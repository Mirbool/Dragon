using Config;
using KKSG;
using Koakuma.Game;
using Koakuma.Game.ECS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TGame.UI;
using UnityEngine;
using XMainClient;
using XUtliPoolLib;

public class SelectRoleUIMediator : UIMediator<SelectRoleUI>
{
    private bool isPlayShow;
    private AnimatorStateInfo crtAniInfo;
    private PlayerComponent pc;
    private List<RoleBriefInfo> playerInfo;

    private int crtRoleIndex;
    private Animator crtAni;
    public override void InitMediator(UIView view)
    {
        base.InitMediator(view);
        
    }

   public void OnHeadClick()
    {

        pc = GameManager.ECS.World.GetComponent<PlayerComponent>();
        crtRoleIndex = -1;


        for (int i = 0; i < view.roleHeads.Count; i++)
        {
            int crtIndex = i;
            view.roleHeads[i].onValueChanged.AddListener((x) =>
           {
               if(view.des.activeSelf==false) view.des.SetActive(true);
               if (x)
               {
                   //view.roleHeads.ForEach((x) => { if (x != view.roleHeads[crtIndex]) x.isOn = false; });

                   if(crtRoleIndex!=-1)
                   {
                       if (playerInfo[crtRoleIndex] != null)
                       {
                           view.roleNames[crtRoleIndex].SetActive(true);

                       }
                       ChangeAni("Idle", "idle_new");
                       crtAni.SetTrigger("ToStand");
                   }
                   crtAni = pc.roles[crtIndex].RoleGo.GetComponent<Animator>();
                   crtRoleIndex = crtIndex;
                   ChangeAni("Select", "select_new");
                   ChangeAni("AA","show_new");
                   ChangeAni("A","show_turn");
                   crtAni.SetTrigger("ToShow");

                   RefreshDes(RoleInfoConfig.ByIndex(crtRoleIndex));

               }
           });
        }
       
      

    }
 
    public  void ChangeAni(string stateName,string aniName)
    {
        string allName = RoleInfoConfig.ByIndex(crtRoleIndex).Animation+aniName;
        pc.roles[crtRoleIndex].ChangeAnimation(stateName, allName);
    }
    /// <summary>
    /// 刷新描述页面
    /// </summary>
    public void RefreshDes(RoleInfoConfig data )
    {
        if (!view.des.activeSelf)
        {
            view.des.SetActive(true);
        }
        view.atkType.text = data.AtkType;
        view.careerFeature.text = data.CareerType;
        view.careerIcon.sprite = Resources.Load<Sprite>("atlas/UI/LoginSelectChar/LoginSelectChar_"+data.Icon_Index);
        for (int i = 0; i < view.showStarts.Count; i++)
        {
            if (i<data.Starts)
            {
                view.showStarts[i].gameObject.SetActive(true);
            }
            else
            {
                view.showStarts[i].gameObject.SetActive(false);
            }
        }
        if (playerInfo[crtRoleIndex]!=null)
        {
            view.roleNames[crtRoleIndex].SetActive(false);
            view.nameTxt.text = playerInfo[crtRoleIndex].name;
            view.buttonTxt.text = "开始游戏";
        }
        else
        {
          
            view.buttonTxt.text = "创建角色";
            view.nameTxt.text = "未创建角色";

        }
        view.careerDes.text = data.Description;

    }
    private void LoadMyRole()
    {
        Camera cam= Camera.main;
        for (int i = 0; i < playerInfo.Count; i++)
        {
            if (playerInfo[i]==null)
            {
                view.roleNames[i].SetActive(false);
            }
            else
            {
                view.nameTxts[i].text = playerInfo[i].name;
                //Vector3 pos = cam.WorldToScreenPoint(pc.roles[i].RoleGo.transform.position);
                //view.roleNames[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x-Screen.width/2,pos.y)+Vector2.up*600;
            }
        }
    }

    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        playerInfo = XSingleton<RolesMgr>.singleton.XPlayerCharacters.PlayerBriefInfo;
        OnHeadClick();
        LoadMyRole();
        view.enterBtn.onClick.AddListener(EnterBtnClick);
        GameManager.Message.Subscribe<MessageType.EnterLobby>(OnEnterLobbyRes);

    }

    private  async Task OnEnterLobbyRes(MessageType.EnterLobby arg)
    {
        GameManager.Procedure.ChangeProcedure<LobbyProcedure>(crtRoleIndex).Coroutine();
        await Task.Yield();         
    }



    /// <summary>
    /// 选择角色进入主场景
    /// </summary>
    public void EnterBtnClick()
    {

        if (playerInfo[crtRoleIndex] != null)
        {
            RpcC2M_SelectRoleNew rpcC2M_SelectRoleNew = new RpcC2M_SelectRoleNew();
            rpcC2M_SelectRoleNew.oArg.index = crtRoleIndex;
            XSingleton<XClientNetwork>.singleton.Send(rpcC2M_SelectRoleNew);
        }

        else
        {
            Debug.Log("未创建角色");
            //TODO  创建角色
            //RpcC2M_CreateRoleNew rpcC2M_CreateRoleNew = new RpcC2M_CreateRoleNew();
            //rpcC2M_CreateRoleNew.oArg.type = pc.roles[crtRoleIndex].role;
        }

    }
    protected override void OnHide()
    {
        base.OnHide();
    }
}
