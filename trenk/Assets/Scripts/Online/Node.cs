using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public NetSocketManager Net { get; protected set; } // Manages client connections
    public Queue<Message> MessageQueue { get; protected set; }

    void Start()
    {
        // Initialize members
        MessageQueue = new Queue<Message>();
        Net = new NetSocketManager(new MessageSerializer(this));
    }

}
