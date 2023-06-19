using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCtrl : MonoBehaviour
{
    public List<AnimationData> animationDatas;

    private RuntimeAnimatorController controller;
    private AnimatorOverrideController overrideController;

    private Animator animator;
    private string Default;
    private string State1 = string.Empty;
    private AnimationData currentAnimationData;


    private void Awake()
    {
        Default = "Default";
        State1 = "State1";
        animator = this.GetComponent<Animator>();
        if (overrideController == null)
        {
            controller = GameObject.Instantiate<RuntimeAnimatorController>(Resources.Load<RuntimeAnimatorController>("mc_controller"));
            overrideController = new AnimatorOverrideController(controller);
            animator.runtimeAnimatorController = overrideController;
        }
    }

    /// <summary>
    /// 播放某个动画
    /// </summary>
    /// <param name="clipName"></param>
    public void Play(string clipName)
    {
        currentAnimationData = animationDatas.Find((x) => x.name.Equals(clipName));

        if (currentAnimationData != null && currentAnimationData.isRunning == false)
        {
            currentAnimationData.isRunning = true;
            currentAnimationData.crtAniTotalTime = currentAnimationData.clip.length;
            overrideController[State1] = currentAnimationData.clip;
        }
    }

    private void Update()
    {
        if (currentAnimationData != null && currentAnimationData.isRunning)
        {
            currentAnimationData.crtPlayTime += Time.deltaTime;

            currentAnimationData.crtPlayNormalizeTime = Mathf.Clamp01(currentAnimationData.crtPlayTime / currentAnimationData.crtAniTotalTime);

            this.animator.Play(State1, 0, currentAnimationData.crtPlayNormalizeTime);

            Debug.Log(currentAnimationData.crtPlayTime + "    "+ currentAnimationData.crtAniTotalTime+"   "+currentAnimationData.crtPlayNormalizeTime);

            if (currentAnimationData.crtPlayTime >= currentAnimationData.crtAniTotalTime)
            {
                
                currentAnimationData.crtPlayTime = 0;
                currentAnimationData.crtPlayNormalizeTime = 0;
                currentAnimationData.isRunning = false;
                 
            }
        }

    }
}

[Serializable]
public class AnimationData
{
    public string name;
    public AnimationClip clip;

    /// <summary>
    /// 当前动画播放到哪个时间了，秒
    /// </summary>
    [NonSerialized]
    public float crtPlayTime = 0;
    /// <summary>
    /// 标准化时间   0---1
    /// </summary>
    [NonSerialized]
    public float crtPlayNormalizeTime = 0;
    /// <summary>
    /// 动画总时间
    /// </summary>
    [NonSerialized]
    public float crtAniTotalTime;
    /// <summary>
    /// 动画是否播放中
    /// </summary>
    [NonSerialized]
    public bool isRunning = false;

}

