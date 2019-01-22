using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetSerializer
{
    /// <summary>
    /// Handles received data over network
    /// </summary>
    bool Receive(short type, byte[] data);
}
