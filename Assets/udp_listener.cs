using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class udp_listener : MonoBehaviour
{
    private UdpClient udpClient = null!;

    private int port = 50000;

    void Start()
    {
        udpClient = new UdpClient(port);
        //listens to the broadcast address 255.255.255.255 https://en.wikipedia.org/wiki/Broadcast_address
        udpClient.EnableBroadcast = true;

        Debug.Log("UDP listening on port " + port);
    }

    void Update()
    {
        try
        {
            while (udpClient.Available > 0)
            {
                //connect to "any" ip address, in this case the broadcast address
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
                //see if there is a message that has been sent, if so print it
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
}
