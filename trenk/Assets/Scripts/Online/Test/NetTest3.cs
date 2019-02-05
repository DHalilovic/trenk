using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class NetTest3 : MonoBehaviour
{
    public short localPort = 9999;
    public short targetPort = 9999;
    public string targetIp; // Opponent's address
    public bool Server { get; set; }

    private Socket socket; // Listens for connection requests
    private BufferedStream stream; // Wraps socket for data retrieval
    private readonly byte[] readBuffer = new byte[5000];

    public void Listen()
    {
        new Thread(() =>
       {
           IPAddress myIpa = IPAddress.Any;
           IPEndPoint myIpe = new IPEndPoint(myIpa, localPort);

           IPAddress oIpa = IPAddress.Parse(targetIp);
           IPEndPoint oIpe = new IPEndPoint(oIpa, targetPort);

           if (Server)
           {
               Debug.Log("Server");

               socket = new Socket(myIpa.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);
               socket.Bind(myIpe);
               socket.Listen(100);
               socket.Accept();
           }
           else
           {
               Debug.Log("Client");

               socket = new Socket(myIpa.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);
               socket.Connect(oIpe);
               Debug.Log("Connecting");
           }

           Debug.Log("Connected");

       }).Start();
    }
}
