using UnityEngine;
using System.Collections;

namespace UTJ
{
    public class RockAudioModulator : MonoBehaviour
    {
        public fftAnalyzer fft;
        public Renderer rend;
        // Use this for initialization

        public double Map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        void Start()
        {
            if (fft == null)
                fft = FindObjectOfType<fftAnalyzer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (fft != null)
            {
                rend.material.SetFloat("_Displacement", 0.1f + fft.spectrumBinned[0] * 20.0f);
                //hair.m_params.m_rootStiffness = fft.spectrumBinned[0] * 1.2f;
            }
        }
    }
}

