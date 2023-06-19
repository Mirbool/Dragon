using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum SkillActComponentType : byte
{
    Animation,
    Auido,
    ActorEffect
}


public abstract class SkillActComponentBase
{
    protected SkillActComponentDataBase _data;
    protected  SkillEntity _skillentity;

    private float timeCount = 0;
    private bool isRunning = false;
    private bool hasTrigger = false;
    public SkillActComponentBase(SkillActComponentDataBase data,SkillEntity entity)
    {
        this._data = data;
        this._skillentity = entity;
    }
    public void Play()
    {
        if (isRunning == false)
        {
            isRunning = true;
        }
    }

    public void Stop()
    {
        if (isRunning)
        {
            isRunning = false;
            hasTrigger = false;
            timeCount = 0;
        }
    }

    public virtual void Update()
    {
        if (isRunning)
        {
            timeCount += Time.deltaTime;
            if (timeCount > _data.triggerTime)
            {
                timeCount = 0;
                isRunning= false;
                startLogic();
                hasTrigger = true;
            }
        }

        if (hasTrigger)
        {
            LogicUpdate();
        }
    }


    public abstract void startLogic();
    //public abstract void stopLogic();
    public abstract void LogicUpdate();

}

