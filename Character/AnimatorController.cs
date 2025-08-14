using System.Collections;
using System.Collections.Generic;
using StarCloudgamesLibrary;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        animator = GetComponent<Animator>();
    }

    public void SetUp(RuntimeAnimatorController controller)
    {
        if (animator == null)
        {
            Initialize();
        }
        animator.runtimeAnimatorController = controller;
    }

    public void SetBool(int name, bool value)
    {
        animator.SetBool(name, value);
    }

    public void SetInt(int name, int value)
    {
        animator.SetInteger(name, value);
    }

    public void SetTrigger(int name)
    {
        animator.SetTrigger(name);
    }

    public void SetFloat(int name, float value)
    {
        animator.SetFloat(name, value);
    }

    public void ResetTrigger(int name)
    {
        animator.ResetTrigger(name);
    }

    public bool IsPlaying(int name)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash == name;
    }

    public float GetCurrentAnimationTime()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
    }

    public bool GetBool(int name)
    {
        return animator.GetBool(name);
    }
}