using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ZeroFormatter;
using ZeroFormatter.Formatters;

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

        switch (type)
        {
            case (byte) Message.Type.PING:

                break;
            case (byte) Message.Type.COUNT:

                break;
            case (byte)Message.Type.INPUT:

                break;
        }

        return false;
    }
}
