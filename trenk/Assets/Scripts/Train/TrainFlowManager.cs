using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainFlowManager : MonoBehaviour
{
    private TrainGameManager m;
    private TrainRoundManager r;
    private MovementInput i;
 
    private void Awake()
    {
        m = GetComponent<TrainGameManager>();
        r = GetComponent<TrainRoundManager>();
        i = GetComponent<MovementInput>();
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe("round-prepare", OnRoundPrepare);
        EventManager.Instance.Subscribe("round-end", OnRoundEnd);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe("round-prepare", OnRoundPrepare);
        EventManager.Instance.Unsubscribe("round-end", OnRoundEnd);
    }

    private void OnRoundPrepare(IEventParam e)
    {
        r.Reset();
        m.Clean();

        r.enabled = true;
        i.enabled = true;
    }

    private void OnRoundEnd(IEventParam e)
    {
        r.enabled = false;
        i.enabled = false;

        EventManager.Instance.Raise("round-prepare", null);
    }
}
