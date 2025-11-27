using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class udp_listener : MonoBehaviour
{
    private UdpClient udpClient = null!;

    // private Thread receiveThread = null!;
    private int port = 50000;

    // private string receivedMessage = null;
    // private readonly object messageLock = new object();

    void Start()
    {
        udpClient = new UdpClient(port);
        udpClient.EnableBroadcast = true;
        // receiveThread = new Thread(ReceiveData);
        // receiveThread.IsBackground = true;
        // receiveThread.Start();

        Debug.Log("UDP listening on port " + port);
    }

    // private void ReceiveData()
    // {
    //     while (true)
    //     {
    //         IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
    //         byte[] data = udpClient.Receive(ref remoteEP);
    //         string message = Encoding.UTF8.GetString(data);

    //         lock (messageLock)
    //         {
    //             receivedMessage = message;
    //         }
    //     }
    // }

    void Update()
    {
        try
        {
            while (udpClient.Available > 0)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.UTF8.GetString(data);
                Debug.Log("UDP Received: " + message);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("UDP Error: " + ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
    // void Update()
    // {
    //     string messageToHandle = null;

    //     lock (messageLock)
    //     {
    //         if (!string.IsNullOrEmpty(receivedMessage))
    //         {
    //             messageToHandle = receivedMessage;
    //             receivedMessage = null;
    //         }
    //     }

    //     if (!string.IsNullOrEmpty(messageToHandle))
    //     {
    //         Debug.Log("UDP Received: " + messageToHandle);
    //     }
    // }
}
