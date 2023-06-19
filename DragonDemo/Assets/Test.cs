using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    [Range(0,1)]    
    public float pos;

    public Animator ani;
    public AnimationClip clip;

    private AnimatorOverrideController overrideController;

    public RuntimeAnimatorController runTimecon;
    void Start()
    {
        this.overrideController = new AnimatorOverrideController(this.runTimecon);
        this.ani.runtimeAnimatorController = overrideController;
        this.overrideController["State1"] = clip;
    }

    // Update is called once per frame
    void Update()
    {
        this.ani.Play("State1", 0, pos);
    }
}
