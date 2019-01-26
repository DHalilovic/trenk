using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueEntry
{
    public readonly Type type;
    public readonly Object body;

    public enum Type
    {
        Message = 0
    }

    public QueueEntry(Type type, Object body)
    {
        this.type = type;
        this.body = body;
    }
}
