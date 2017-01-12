using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace UTJ
{
    public class AvatarTracker : MonoBehaviour
    {

        public GameObject[] avatarsLibrary;
        public List<GameObject> activeAvatars;
        public List<long> oldUserIds;
        public GameObject[] spawns;

        public KinectManager manager;

        public bool active;

        // Use this for initialization
        void Start()
        {
            if (manager == null)
            {
                manager = FindObjectOfType<KinectManager>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyUp(KeyCode.Space) && activeAvatars.Count > 0)
            {
                HairInstance hair = activeAvatars[0].GetComponent<HairInstance>();
                RandomizeHairParameters(hair);
            }
        }

        public void RandomizeHairParameters(HairInstance hair)
        {
            float Hue = Random.Range(0.0f, 1.0f);
            hair.m_params.m_rootColor = Color.HSVToRGB(Hue, 1, 0.8f);//Random.ColorHSV(0.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f);
            Hue += 0.5f;
            Hue = 1.0f - Hue;
            hair.m_params.m_tipColor = Color.HSVToRGB(1, 1, 1);
            //hair.m_params.m_lengthScale = Random.Range(0.2f, 0.2f);
            //hair.m_params.m_widthRootScale = Random.Range(0.01f, 4.0f);
            hair.m_params.m_widthTipScale = Random.Range(0.0f, 0.5f);
            hair.m_params.m_rootStiffness = Random.Range(0.0f, 4.0f);
            hair.m_params.m_specularPrimary = Random.Range(0.5f, 1.0f);
            hair.m_params.m_density = Random.Range(0.1f, 0.2f);
            //hair.m_params.m_lengthScale = Random.Range(0.0f, 10.0f);
            //hair.m_params.m_waveScale = Random.Range(0.0f, 2.0f);
            //hair.m_params.m_diffuseBlend = Random.Range(-0.2f, 0.2f);
            //hair.m_params.m_specularPrimary = Random.Range(0.0f, 1.0f);
            //hair.m_params.m_gravityDir.y = Random.Range(0.0f, 0.0f);
            //hair.m_params.m_rootStiffness = Random.Range(0.0f, 0.0f);




        }

        void OnCalibrate(int uidIndex)
        {
            // Debug.Log("Creating Player " + uidIndex);
            
            if (avatarsLibrary.Length > 0 && activeAvatars.Count == 0)
            {
                int prefabIndex = Random.Range(0, avatarsLibrary.Length);
                GameObject obj = Instantiate(avatarsLibrary[prefabIndex]);

                Debug.Log("Just found a user! instantiating her with user index: " + uidIndex);
                AvatarController controller = obj.GetComponent<AvatarController>();

                controller.playerIndex = uidIndex;
                List<long> newUserIds = manager.GetAllUserIds();

                foreach (long newId in newUserIds)
                {
                    bool found = false;
                    foreach (long oldID in oldUserIds)
                    {
                        if (newId == oldID)
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        Debug.Log("Setting their userID to: " + newId);
                        controller.playerId = newId;
                        break;
                    }
                }

                HairInstance hair = obj.GetComponent<HairInstance>();

                RandomizeHairParameters(hair);

                oldUserIds = newUserIds;

                activeAvatars.Add(obj);

                DontDestroyOnLoad(obj);

                manager.avatarControllers.Add(obj.GetComponent<AvatarController>());
            }

            //avatars[uidIndex].transform.localScale = new Vector3(1f, 1f, 1f);
        }

        void OnRemove(int uidIndex)
        {
            GameObject avatarToDestroy = activeAvatars[uidIndex];
            activeAvatars.Remove(avatarToDestroy);
            manager.avatarControllers.Remove(avatarToDestroy.GetComponent<AvatarController>());
            Destroy(avatarToDestroy);

            //avatars[uidIndex].transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }
}


