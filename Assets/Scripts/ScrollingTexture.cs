using UnityEngine;
using System.Collections;

public class ScrollingTexture : MonoBehaviour {

    Material mat;

    float offset;

	// Use this for initialization
	void Start () 
    {
        mat = GetComponent<MeshRenderer>().material;
        offset = 0f;
	}
	
	// Update is called once per frame
    void FixedUpdate()
    {
        offset += 0.01f;
        mat.mainTextureOffset = new Vector2(0, offset);
        
    }
}
