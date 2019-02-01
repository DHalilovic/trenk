using System;

public class MessageSerializer : INetSerializer
{
    private readonly NodeManager node;

    public MessageSerializer(NodeManager node)
    {
        this.node = node;
    }

    public bool Receive(byte type, byte[] data)
    {
        Message message = null;
        bool result = true;

        try
        {
            switch (type)
            {
                case (byte)Message.MessageType.INPUT:
                    message = new Message(Message.MessageType.INPUT, new InputMessage(BitConverter.ToInt16(data, 0), data[2]));
                    node.MessageQueue.Enqueue(message);
                    break;
                case (byte)Message.MessageType.COUNT:

                    break;
                case (byte)Message.MessageType.PING:

                    break;
            }
        }
        catch (Exception)
        {
            result = false;
        }

        return result;
    }
}
