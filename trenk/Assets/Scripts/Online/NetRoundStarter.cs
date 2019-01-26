using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRoundStarter : MonoBehaviour
{
    private NetGameManager manager;
    private NodeManager node;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<NetGameManager>();
        node = manager.Node;
    }

}
