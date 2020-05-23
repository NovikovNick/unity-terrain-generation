using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;

public class NetworkManager : Singleton<NetworkManager>
{

    #region Data

    UdpClient udpClient;
    IPEndPoint hostEndPoint;

    int sequenceNumber = 0;
    public long updatedAt;
    public PlayerSnapshot previousSnapshot;
    public PlayerSnapshot snapshot;
    public ConcurrentQueue<PlayerRequest> requests = new ConcurrentQueue<PlayerRequest>();

    #endregion

    void Awake()
    {
        hostEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.102"), 7777);
        udpClient = new UdpClient();
        udpClient.Connect(hostEndPoint);
        udpClient.Client.Blocking = false;
        udpClient.Client.ReceiveTimeout = 1000;
        udpClient.BeginReceive(new AsyncCallback(processDgram), udpClient);
    }
    

    public void processDgram(IAsyncResult res)
    {
        try
        {

            byte[] recieved = udpClient.EndReceive(res, ref hostEndPoint);
            string json = System.Text.Encoding.UTF8.GetString(recieved);

            PlayerSnapshot responce = JsonUtility.FromJson<PlayerSnapshot>(json);
            
            updatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            previousSnapshot = snapshot;
            snapshot = responce;
            
            udpClient.BeginReceive(new AsyncCallback(processDgram), udpClient);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void ChangePlayerPosition(Vector3 direction, float magnitude, float timeDelta, bool isRunning)
    {
        PlayerRequest request = new PlayerRequest();
        request.sequenceNumber = sequenceNumber++;
        if(snapshot != null)
        {
            request.acknowledgmentNumber = snapshot.sequenceNumber;
        }


        request.magnitude = magnitude;
        request.timeDelta = timeDelta;
        request.direction = direction;
        request.isRunning = isRunning;

        requests.Enqueue(request);

        string json = JsonUtility.ToJson(request).ToString();

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        udpClient.Send(bytes, bytes.Length);

        // Debug.Log("sent: " + request);
    }
}
