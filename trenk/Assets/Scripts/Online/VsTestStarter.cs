using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VsTestStarter : MonoBehaviour
{
    public bool hosting;
    public string remoteIp;

    private void Start()
    {
        EventManager.Instance.Subscribe("connect", (e) => { });
        EventManager.Instance.Raise("try-connect", new IpParam(hosting, remoteIp, 9999));
        //EventManager.Instance.Raise("connect", new BoolParam(true));
    }
}
