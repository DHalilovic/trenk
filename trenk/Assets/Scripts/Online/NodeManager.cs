using System;
using System.Collections.Generic;

public class NodeManager : Singleton<NodeManager>
{
    public NetSocketManager Net { get; protected set; } // Manages client connections
    public Queue<Message> MessageQueue { get; protected set; }

    private Action<IEventParam> searchGameListener, playerFoundListener;

    protected override void Awake()
    {
        base.Awake();

        // Initialize members
        MessageQueue = new Queue<Message>();
        Net = new NetSocketManager(new MessageSerializer(this));

        // Initialize listeners
        playerFoundListener = new Action<IEventParam>(OnSearchGame);
        playerFoundListener = new Action<IEventParam>(OnPlayerFound);
    }

    private void OnEnable()
    {
        EventManager.Subscribe("searchgame", OnSearchGame);
        EventManager.Subscribe("playerfound", OnPlayerFound);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe("searchgame", OnSearchGame);
        EventManager.Unsubscribe("playerfound", OnPlayerFound);
    }

    private void OnSearchGame(IEventParam e)
    {

    }

    private void OnPlayerFound(IEventParam e)
    {

    }
}
