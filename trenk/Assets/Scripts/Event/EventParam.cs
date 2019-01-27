
public class StartListenEventParam : IEventParam
{
    public bool host;

    public StartListenEventParam(bool host)
    {
        this.host = host;
    }
}

public class PlayerFoundEventParam : IEventParam
{
    public string ip;
    public short port;

    public PlayerFoundEventParam(string ip, short port)
    {
        this.ip = ip;
        this.port = port;
    }
}

