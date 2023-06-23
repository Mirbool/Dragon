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
    public Text buttonTxt;
    public Text nameTxt;//选中后描述板面任务名称
    public List<GameObject> roleNames;//名称游戏物体
    public List<Text> nameTxts;//名称文本
    
}
