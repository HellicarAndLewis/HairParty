using UnityEngine;
using System.Collections;

namespace UTJ
{
    public class HairAudioModulator : MonoBehaviour
    {
        public BeatGetter beat;
        public HairInstance hair;
        public AvatarTracker tracker;
        [Range(0, 1)]
        public float sensitivity = 0.1f;

        private int numBeats;
        // Use this for initialization

        public double Map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        void Start()
        {
            if (beat == null)
                beat = FindObjectOfType<BeatGetter>();
            if (tracker == null)
                tracker = FindObjectOfType<AvatarTracker>();

            sensitivity = 0.25f;

            numBeats = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if(beat != null)
            {
                hair.m_params.m_rootStiffness = beat.beat;
                if(beat.beat > 1.0f - sensitivity)
                {
                    numBeats++;
                    if (numBeats == 16)
                    {
                        tracker.RandomizeHairParameters(hair);
                        numBeats = 0;
                    }
                }
            }
        }
    }
}

