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

    int datagramNumber = 0;


    public PlayerSnapshot snapshot;
    // public ConcurrentQueue<PlayerSnapshot> snapshots = new ConcurrentQueue<PlayerSnapshot>();
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
            //snapshots.Enqueue(responce);
            snapshot = responce;
            
            // Debug.Log("received from server: " + responce.position + ":" + responce.lastDatagramNumber);
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
        request.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        request.datagramNumber = datagramNumber++;
        request.magnitude = round(magnitude);
        request.timeDelta = round(timeDelta);
        request.direction = new Vector3(round(direction.x), round(direction.y), round(direction.z));
        request.isRunning = isRunning;

        requests.Enqueue(request);

        string json = JsonUtility.ToJson(request).ToString();


        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        udpClient.Send(bytes, bytes.Length);

        // Debug.Log("sent to server: " + json);
    }

    float round(float value)
    {
        return Mathf.Round(value * 10000) / 10000;
    }
}
