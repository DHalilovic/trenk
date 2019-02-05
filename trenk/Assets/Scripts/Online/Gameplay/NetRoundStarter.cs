using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRoundStarter : MonoBehaviour
{
    private NetGameManager manager;
    private NodeManager node;

    void Start()
    {
        manager = GetComponent<NetGameManager>();
        node = manager.Node;
    }

}
