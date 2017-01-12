using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatGetter : MonoBehaviour
{
    //private int samples = 256;
    public float beat;
    public float downSpeed = 0.05f;
    public OSCHandler oscHandler;
    //public float[] spectrumBinned;
    //[Range(1, 256)]
    //public int bins;

    // --------------------------------------------------------------------------------------------------------
    //
    //private float average;
    //private float peak;
    private Dictionary<string, ServerLog> servers;

    // --------------------------------------------------------------------------------------------------------
    //
    void Start()
    {
        if (oscHandler == null)
            oscHandler = GetComponent<OSCHandler>();

        oscHandler.Init();
    }

    // --------------------------------------------------------------------------------------------------------
    //
    void Update()
    {
        oscHandler.UpdateLogs();
        //servers = OSCHandler.Instance.Servers;
        //var packets = servers["fftInput"].packets;
        beat = Mathf.Lerp(beat, 0.0f, downSpeed);

        //if (packets.Count == 1)
        //{
        //    //Debug.Log("Recieved First Packet!");
        //    var packet = packets[packets.Count - 1];
        //    foreach (object data in packet.Data)
        //    {
        //        beat = 1.0f;// float.Parse(data.ToString());
        //    }
        //} else if (servers["fftInput"].server.LastReceivedPacket.TimeStamp != packets[packets.Count - 2].TimeStamp)
        //{
        //   // Debug.Log("Recieved Packet!");
        //    var packet = packets[packets.Count - 1];
        //    foreach (object data in packet.Data)
        //    {
        //        beat = 1.0f;// float.Parse(data.ToString());
        //    }
        //}
        Debug.DrawLine(Vector3.zero, Vector3.zero + new Vector3(0, beat * 10.0f, 0));
    }

    // --------------------------------------------------------------------------------------------------------
    //
    public float Beat
    {
        get
        {
            return this.beat;
        }
    }

    void OnRecieved(float _beat)
    {
        beat = _beat;
    }
}