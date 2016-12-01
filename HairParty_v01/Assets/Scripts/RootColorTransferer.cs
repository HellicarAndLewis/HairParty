using UnityEngine;
using System.Collections;

namespace UTJ
{
    public class RootColorTransferer : MonoBehaviour
    {

        public Renderer rend;
        public HairInstance hair;
        private Color col;
        // Use this for initialization
        void Start()
        {
            if (rend == null)
                rend = GetComponent<Renderer>();
            if (hair == null)
                hair.GetComponent<HairInstance>();

            if(rend != null && hair != null)
            {
                col = hair.m_params.m_rootColor;
                rend.material.color = col;
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (rend != null && hair != null)
            {
                col = hair.m_params.m_rootColor;
                rend.material.color = col;
            }
        }
    }
}


