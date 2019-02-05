
public class Message
{
    public MessageType Type { get; private set; }
    public object Body { get; private set; }

    public enum MessageType : byte
    {
        PING = 0, COUNT, INPUT 
    }

    public Message(MessageType type, object body)
    {
        Type = type;
        Body = body;
    }
}
