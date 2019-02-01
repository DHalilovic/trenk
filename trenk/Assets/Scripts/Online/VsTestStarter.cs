using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VsTestStarter : MonoBehaviour
{
    public bool host;
    public string targetIp;

    private void Start()
    {
        EventManager.Instance.Subscribe("connect", (e) => { });
        EventManager.Instance.Raise("try-connect", new IpParam(host, targetIp, 9999));
        //EventManager.Instance.Raise("connect", new BoolParam(true));
    }
}
