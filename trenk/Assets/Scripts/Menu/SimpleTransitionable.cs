using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleTransitionable : Transitionable
{
    public AnimationClip inClip, outClip;

    private Animator anim;
    private int inHash = Animator.StringToHash("FadeIn");
    private int outHash = Animator.StringToHash("FadeOut");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public override float In()
    {
        anim.Play(inHash);
        return inClip.length;
    }

    public override float Out()
    {
        //throw new System.NotImplementedException();
        anim.Play(outHash);
        return outClip.length;
    }
}
