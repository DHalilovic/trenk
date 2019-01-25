using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ZeroFormatter;
using ZeroFormatter.Formatters;

public class MessageSerializer : INetSerializer
{
    public bool Receive(short type, byte[] data)
    {
        return false;
    }
}
