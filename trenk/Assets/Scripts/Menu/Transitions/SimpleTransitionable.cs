using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Canvas))]
public class SimpleTransitionable : Transitionable
{
    public AnimationClip inClip, outClip;

    private Animator anim;
    private Canvas canvas;
    private int inHash = Animator.StringToHash("FadeIn");
    private int outHash = Animator.StringToHash("FadeOut");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        canvas = GetComponent<Canvas>();
    }

    public override float In()
    {
        anim.Play(inHash);
        return inClip.length;
    }

    public override float Out()
    {
        anim.Play(outHash);
        return outClip.length;
    }
}
