using System.Collections;
using System.Collections.Generic;
using TGame.UI;
using UnityEngine;
using UnityEngine.UI;

[UIView(typeof(SelectServerUIMediator), UIViewID.SelectServerUI)]

public class SelectServerUIView : UIView
{
    public Text showTxt;
    public GameObject selectTip;
    public Button selectBtn;
    public Text server1;
    public Text server2;
    public Button btn1;
    public Button btn2;

    public Button confirmBtn;
}
