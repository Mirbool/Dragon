using System;
using System.Collections.Generic;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationActComponent : SkillActComponentBase
{

   
    public AnimationActComponent(AnimationActData data, SkillEntity e) : base(data, e)
    {
        
    }

    public override void LogicUpdate()
    {
        Debug.Log("动画组件，美珍执行");
    }

    public override void startLogic()
    {
       
        this._skillentity.PlayerEntity.AnimationCtrl.Play((this._data as AnimationActData).animationName);

    }
}

