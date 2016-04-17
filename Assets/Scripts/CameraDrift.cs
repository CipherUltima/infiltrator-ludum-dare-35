using UnityEngine;
using System.Collections;

public class CameraDrift : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Player.IsDead)
        {
            return;
        }
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 diff = (mouseWorldPosition - transform.position);
        diff *= 0.25f;
        diff.z = -10;

        transform.localPosition = diff;
	}
}
