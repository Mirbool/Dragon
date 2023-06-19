using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class AudioActComponent : SkillActComponentBase
{
    public AudioActComponent(AudioActData data, SkillEntity e) : base(data, e) { }

    public override void LogicUpdate()
    {

    }

    public override void startLogic()
    {
        AudioActData dt = this._data as AudioActData;
        this._skillentity.PlayerEntity.AudioMgr.Play(dt.audioPath, dt.audioVolume);
    }
}

