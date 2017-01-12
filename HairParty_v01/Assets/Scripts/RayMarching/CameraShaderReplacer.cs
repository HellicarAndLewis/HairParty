using UnityEngine;
using System.Collections;

public class CameraShaderReplacer : MonoBehaviour {

    public Camera cam;
    public Shader shader;
	// Use this for initialization
	void Start () {
        if (cam == null)
            cam = GetComponent<Camera>();
        cam.SetReplacementShader(shader, "");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
