using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitioner : Singleton<SceneTransitioner>
{
    private int outHash, inHash;
    private Animator anim;
    private Action<IEventParam> outSceneListener, inSceneListener;

    protected override void Awake()
    {
        base.Awake();

        // Get animator
        anim = GetComponent<Animator>();

        // Get animator state hashes
        outHash = Animator.StringToHash("Scene_FadeOut");
        inHash = Animator.StringToHash("Scene_FadeIn");

        // Initialize listeners
        outSceneListener = (e) => anim.Play(outHash);
        inSceneListener = (e) => anim.Play(inHash);
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("request-scene", outSceneListener);
            EventManager.Instance.Subscribe("in-scene", inSceneListener);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("request-scene", outSceneListener);
            EventManager.Instance.Unsubscribe("in-scene", inSceneListener);
        }
    }

    public void RaiseLoadSceneRequest()
    {
        EventManager.Instance.Raise("load-scene-request", null);
    }
}
