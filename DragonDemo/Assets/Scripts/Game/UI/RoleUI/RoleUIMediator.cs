using Koakuma.Game;
using Koakuma.Game.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using TGame.UI;
using UnityEngine;
using UnityEngine.UI;

public class RoleUIMediator : UIMediator<RoleUIView>
{
    SelectRoleComponent sc;
    public override void InitMediator(UIView view)
    {
        base.InitMediator(view);
        sc = GameManager.ECS.World.GetComponent<SelectRoleComponent>();

    }

    protected override void OnShow(object arg)
    {
        base.OnShow(arg);

        view.close_Btn.onClick.AddListener(CloseUI);

        for (int i = 0; i < sc.role.Bag.Items.Count; i++)
        {
            GameObject item = GameObject.Instantiate(Resources.Load<GameObject>("Item"), view.tr);
            item.transform.GetChild(0).GetComponent<Text>().text = sc.role.Bag.Items[i].ItemCount.ToString();
            Debug.LogError(sc.role.SpriteRecord.SpriteData.Count);
            //sc.role.Bag.Items[i].ItemType


        }
    }

    private void CloseUI()
    {
        GameManager.UI.CloseUI(UIViewID.RoleUI);
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
