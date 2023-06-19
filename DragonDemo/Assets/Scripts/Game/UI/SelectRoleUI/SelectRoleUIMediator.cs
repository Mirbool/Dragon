using Config;
using Koakuma.Game;
using Koakuma.Game.ECS;
using System;
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
                   view.roleHeads.ForEach((x) => { if (x != view.roleHeads[crtIndex]) x.isOn = false; });

                   if(crtRoleIndex!=-1)
                   {
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
    /// À¢–¬√Ë ˆ“≥√Ê
    /// </summary>
    public void RefreshDes(RoleInfoConfig data )
    {
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
        view.careerDes.text = data.Description;

    }


    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        //int randomIndex = UnityEngine.Random.Range(0, GameManager.ECS.World.GetComponent<PlayerComponent>().roles.Count);
        //view.roleHeads[randomIndex].isOn = true;
        //GameManager.ECS.World.GetComponent<PlayerComponent>().roles[randomIndex].ChangeAnimation("Idle", RoleInfoConfig.ByIndex(randomIndex).Animation + "select_new");
        OnHeadClick();

    }
    protected override void OnHide()
    {
        base.OnHide();
    }
}
