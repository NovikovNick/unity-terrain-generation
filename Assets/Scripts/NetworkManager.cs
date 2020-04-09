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

    public byte playerId = 1;
    

    UdpClient udpClient;
    IPEndPoint hostEndPoint;
    int datagramNumber = Int32.MinValue;

    #endregion

    public ConcurrentQueue<PlayerSnapshot> snapshots;
    public ConcurrentQueue<PlayerRequest> requests;

    void Start()
    {
        hostEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.102"), 7777);
        udpClient = new UdpClient();
        udpClient.Connect(hostEndPoint);
        udpClient.Client.Blocking = false;
        udpClient.Client.ReceiveTimeout = 1000;

        snapshots = new ConcurrentQueue<PlayerSnapshot>();
        requests = new ConcurrentQueue<PlayerRequest>();
    }

    public void processDgram(IAsyncResult res)
    {
        try
        {

            byte[] recieved = udpClient.EndReceive(res, ref hostEndPoint);
            string json = System.Text.Encoding.UTF8.GetString(recieved);

            PlayerSnapshot responce = JsonUtility.FromJson<PlayerSnapshot>(json);
            snapshots.Enqueue(responce);

            Debug.Log("received from server: " + responce.position + ":" + responce.lastDatagramNumber);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public void ChangePlayerPosition(Vector3 direction, float magnitude, bool isRunning)
    {
        PlayerRequest request = new PlayerRequest();
        request.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        request.datagramNumber = datagramNumber++;
        request.playerId = playerId;
        request.magnitude = magnitude;
        request.direction = direction;
        request.isRunning = isRunning;
        requests.Enqueue(request);

        string json = JsonUtility.ToJson(request).ToString();


        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        udpClient.Send(bytes, bytes.Length);
        udpClient.BeginReceive(new AsyncCallback(processDgram), udpClient);

        //Debug.Log("sent to server: " + json);
    }


    /*
    void Update()
    {
        //PlayerRequest responce = JsonUtility.FromJson<PlayerRequest>(request.text);
        byte[] buffer = new byte[1 + sizeof(float) * 6];
        

        //Debug.Log(transform.position + " " + transform.forward);

        Buffer.BlockCopy(BitConverter.GetBytes(transform.position.x), 0, buffer, 0 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(transform.position.y), 0, buffer, 1 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(transform.position.z), 0, buffer, 2 * sizeof(float), sizeof(float));

        Buffer.BlockCopy(BitConverter.GetBytes(transform.forward.x), 0, buffer, 3 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(transform.forward.y), 0, buffer, 4 * sizeof(float), sizeof(float));
        Buffer.BlockCopy(BitConverter.GetBytes(transform.forward.z), 0, buffer, 5 * sizeof(float), sizeof(float));

        buffer[6 * sizeof(float)] = playerId;

        udpClient.Send(buffer, buffer.Length);
        udpClient.BeginReceive(new AsyncCallback(processDgram), udpClient);

        foreach (var item in playersPosition)
        {
            if (playerId != item.Key)
            {
                if (!players.ContainsKey(item.Key))
                {
                    players[item.Key] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
                players[item.Key].transform.position = item.Value;
            }
        }
    }
    */
}
