
public class ByteParam : IEventParam
{
    public byte val;

    public ByteParam(byte val)
    {
        this.val = val;
    }
}

public class BoolParam : IEventParam
{
    public bool val;

    public BoolParam(bool val)
    {
        this.val = val;
    }
}

public class IntParam : IEventParam
{
    public int val;

    public IntParam(int val)
    {
        this.val = val;
    }
}

public class StringParam : IEventParam
{
    public string val;

    public StringParam(string val)
    {
        this.val = val;
    }
}

public class IpParam : IEventParam
{
    public bool host;
    public string ip;
    public short port;

    public IpParam(bool host, string ip, short port)
    {
        this.host = host;
        this.ip = ip;
        this.port = port;
    }
}

