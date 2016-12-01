using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AvatarTracker : MonoBehaviour {

    public GameObject[] avatarsLibrary;
    public List<GameObject> activeAvatars;
    public List<long> oldUserIds;
    public GameObject[] spawns;

    public KinectManager manager;

	// Use this for initialization
	void Start () {
        if(manager == null)
        {
            manager = FindObjectOfType<KinectManager>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCalibrate(int uidIndex)
    {
       // Debug.Log("Creating Player " + uidIndex);

        if(avatarsLibrary.Length > 0)
        {
            int prefabIndex = Random.Range(0, avatarsLibrary.Length);
            GameObject obj = Instantiate(avatarsLibrary[prefabIndex]);

            Debug.Log("Just found a user! instantiating her with uder index: " + uidIndex);
            AvatarController controller = obj.GetComponent<AvatarController>();

            controller.playerIndex = uidIndex;
            List<long> newUserIds = manager.GetAllUserIds();

            foreach(long newId in newUserIds)
            {
                bool found = false;
                foreach(long oldID in oldUserIds)
                {
                    if(newId == oldID)
                    {
                        found = true;
                    }
                }
                if(!found)
                {
                    Debug.Log("Setting their userID to: " + newId);
                    controller.playerId = newId;
                    break;
                }
            }

            //Vector3 joint = manager.GetJointKinectPosition(controller.playerId, (int)KinectInterop.JointType.SpineBase);

            //if(joint.x < 0)
            //{
            //    obj.transform.SetParent(spawns[0].transform);
            //} else
            //{
            //    obj.transform.SetParent(spawns[1].transform);
            //}
            //Debug.Log(joint.x);


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
