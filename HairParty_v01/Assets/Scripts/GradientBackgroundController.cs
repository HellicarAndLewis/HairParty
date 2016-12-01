using UnityEngine;
using System.Collections;

public class GradientBackgroundController : MonoBehaviour {

    public Renderer rend;
    public Color col1 = new Color(255, 0, 0);
    public Color col2 = new Color(0, 0, 0);

	// Use this for initialization
	void Start () {
        if (rend == null)
            rend = GetComponent<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
        float h = Mathf.PerlinNoise(Time.time * 0.1f, 0);
        col1 = Color.HSVToRGB(h, 1, 1);
        h += 0.5f;
        h = 1.0f - h;

        col2 = Color.HSVToRGB(h, 1, 1);

        rend.material.SetColor("_Color1", col1);
        rend.material.SetColor("_Color2", col2);
    }
}
