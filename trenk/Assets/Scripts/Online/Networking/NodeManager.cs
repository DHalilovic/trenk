using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    public NetSocketManager Net { get; protected set; } // Manages client connections
    public Queue<Message> MessageQueue { get; protected set; }

    private Action<IEventParam> tryConnectListener, tryConnectTimeoutListener;

    protected override void Awake()
    {
        base.Awake();

        // Initialize members
        MessageQueue = new Queue<Message>();
        Net = gameObject.AddComponent<NetSocketManager>();
        Net.Init(new MessageSerializer(this));

        // Initialize listeners
        tryConnectListener = new Action<IEventParam>(
            (e) =>
            {
                IpParam p = (IpParam)e;

                Net.Hosting = p.host;
                Net.remoteIp = p.ip;
                Net.remotePort = p.port;
                Net.Listen();

                //Debug.Log("Try connect as " + (p.host ? "host" : "client"));
            });

        tryConnectTimeoutListener = new Action<IEventParam>(
            (e) =>
            {
                Net.OnDisconnect();
                Net.StopListening();
            });
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("try-connect", tryConnectListener);
            EventManager.Instance.Subscribe("try-connect-timeout", tryConnectTimeoutListener);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("try-connect", tryConnectListener);
            EventManager.Instance.Unsubscribe("try-connect-timeout", tryConnectTimeoutListener);
        }
    }
}
