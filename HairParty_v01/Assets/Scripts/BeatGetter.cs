using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatGetter : MonoBehaviour
{
    //private int samples = 256;
    public float beat;
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
        OSCHandler.Instance.Init();
    }

    // --------------------------------------------------------------------------------------------------------
    //
    void Update()
    {
        OSCHandler.Instance.UpdateLogs();
        servers = OSCHandler.Instance.Servers;
        var packets = servers["fftInput"].packets;

        if (packets.Count > 0)
        {
            int newestIndex = packets.Count - 1;
            var packet = packets[newestIndex];
            foreach (object data in packet.Data)
            {
                beat = float.Parse(data.ToString());
                Debug.DrawLine(Vector3.zero, Vector3.zero + new Vector3(0, beat * 10.0f, 0));
            }
        }
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
}