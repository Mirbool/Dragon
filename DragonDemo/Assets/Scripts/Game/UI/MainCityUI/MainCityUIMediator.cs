using System.Collections;
using System.Collections.Generic;
using TGame.UI;
using UnityEngine;

public class MainCityUIMediator : UIMediator<MainCityUI>
{
    protected override void OnInit(MainCityUI view)
    {
        base.OnInit(view);
    }
    protected override void OnShow(object arg)
    {
        base.OnShow(arg);
        view.bagBtn.onClick.AddListener(OnBagBtnClick);
        view.petBtn.onClick.AddListener();
        
    }
    private void OnBagBtnClick()
    {

    }
    protected override void OnHide()
    {
        base.OnHide();
    }
}
