using Koakuma.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using TGame.UI;
using UnityEngine;

public class MainSceneUIMediator : UIMediator<MainSceneUIView>
{
    public override void InitMediator(UIView view)
    {
        base.InitMediator(view);
    }

    protected override void OnShow(object arg)
    {
        base.OnShow(arg);

        view.Role_Btn.onClick.AddListener(RoleUIBtn);
    }

    private void RoleUIBtn()
    {
        GameManager.UI.OpenUI(UIViewID.RoleUI);
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
