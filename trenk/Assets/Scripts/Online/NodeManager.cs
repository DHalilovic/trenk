using System;
using System.Collections.Generic;

public class NodeManager : Singleton<NodeManager>
{
    public NetSocketManager Net { get; protected set; } // Manages client connections
    public Queue<Message> MessageQueue { get; protected set; }

    private Action<IEventParam> hostListener, clientListener, connectListener;

    protected override void Awake()
    {
        base.Awake();

        // Initialize members
        MessageQueue = new Queue<Message>();
        Net = new NetSocketManager(new MessageSerializer(this));

        // Initialize listeners
        hostListener = new Action<IEventParam>(OnHost);
        clientListener = new Action<IEventParam>(OnClient);
        connectListener = new Action<IEventParam>(OnConnect);
    }

    private void OnEnable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Subscribe("host", OnHost);
            EventManager.Instance.Subscribe("client", OnClient);
            EventManager.Instance.Subscribe("connect", OnConnect);
        }
    }

    private void OnDisable()
    {
        EventManager e = EventManager.Instance;

        if (e != null)
        {
            EventManager.Instance.Unsubscribe("host", OnHost);
            EventManager.Instance.Unsubscribe("client", OnClient);
            EventManager.Instance.Unsubscribe("connect", OnConnect);
        }
    }

    private void OnHost(IEventParam e)
    {

    }

    private void OnClient(IEventParam e)
    {

    }

    private void OnConnect(IEventParam e)
    {

    }
}
