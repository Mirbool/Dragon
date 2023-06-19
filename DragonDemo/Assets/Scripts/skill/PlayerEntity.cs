using UnityEngine;

[RequireComponent(typeof(AudioMgr))]
[RequireComponent(typeof(AnimationCtrl))]
public class PlayerEntity : MonoBehaviour
{
    public AnimationCtrl AnimationCtrl ;
    public SkillEntity SkillEntity ;


    private AudioMgr audioMgr;
    public AudioMgr AudioMgr { get => audioMgr;   }


    private void Awake()
    {
        this.AnimationCtrl = GetComponent<AnimationCtrl>();
        audioMgr = GetComponent<AudioMgr>();    
    }

    private void Update()
    {
        if (this.SkillEntity!=null)
        {
            this.SkillEntity.Update();
        }
    }
}

