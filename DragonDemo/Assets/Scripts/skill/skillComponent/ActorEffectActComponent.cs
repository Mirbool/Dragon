using UnityEngine;

public class ActorEffectActComponent : SkillActComponentBase
{
    public ActorEffectActComponent(ActorEffectActData data, SkillEntity e) : base(data, e) { }

    public override void LogicUpdate()
    {

    }

    public override void startLogic()
    {
        ActorEffectActData dt = this._data as ActorEffectActData;
        if (dt != null)
        {
            GameObject effect = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("effect/"+dt.effectPath));
            Transform bindTrans = null;
            switch (dt.bindPoint)
            {
                case EffectBindPoint.None:
                    break;
                case EffectBindPoint.Body:
                    break;
                case EffectBindPoint.Head:
                    break;
                case EffectBindPoint.LeftHand:
                    bindTrans = this._skillentity.PlayerEntity.transform.FindChildByName("leftHandBindPoint");
                    break;
                case EffectBindPoint.RightHand:
                    break;
                case EffectBindPoint.Foot:
                    break;
                default:
                    break;
            }

            if (bindTrans!=null)
            {
                effect.transform.SetParent(bindTrans, false);
            }
        }
    }
}

