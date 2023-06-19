using System.Collections;
using System.Collections.Generic;
using TGame.UI;
using UnityEngine;
using UnityEngine.UI;

[UIView(typeof(SelectRoleUIMediator), UIViewID.SelectRoleUI)]

public class SelectRoleUI : UIView
{
    public List<Toggle> roleHeads;

    public Image careerIcon;
    public Text careerDes;
    public Text atkType;
    public Text careerFeature;
    public List<Image> showStarts;
    public GameObject des;

    public Button enterBtn;
    public Text nameTxt;

    
}
