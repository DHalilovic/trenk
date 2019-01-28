using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    public NetSocketManager Net { get; protected set; } // Manages client connections
    public Queue<Message> MessageQueue { get; protected set; }

    private Action<IEventParam> tryConnectListener;

    protected override void Awake()
    {
        base.Awake();

        // Initialize members
        MessageQueue = new Queue<Message>();
        Net = new NetSocketManager(new MessageSerializer(this));

        // Initialize listeners
        tryConnectListener = new Action<IEventParam>(OnTryConnect);
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("try-connect", tryConnectListener);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("try-connect", tryConnectListener);
        }
    }

    private void OnTryConnect(IEventParam e)
    {
        IpParam p = (IpParam)e;

        Net.Host = p.host;
        Net.targetIp = p.ip;
        Net.targetPort = p.port;
        Net.Listen();

        Debug.Log("Try connect as " + (p.host ? "host" : "client"));
    }

}
