using UnityEngine;
using System.Collections;

public class AvatarTracker : MonoBehaviour {

    public GameObject[] basicAvatars;
    public GameObject[] avatars;
    public KinectManager manager;
    public Camera cam;
	// Use this for initialization
	void Start () {
        if(manager == null)
        {
            manager = GetComponent<KinectManager>();
        }
        if(cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCalibrate(int uidIndex)
    {
        Debug.Log("Creatung Player " + uidIndex);

        basicAvatars[uidIndex].transform.localScale = new Vector3(1f, 1f, 1f);
    }

    void OnRemove(int uidIndex)
    {
        basicAvatars[uidIndex].transform.localScale = new Vector3(0f, 0f, 0f);

    }
}
