using UnityEngine;
using System.Collections;

public class PlayerCrosshair : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 diff = (mouseWorldPosition - transform.position);
        diff.Normalize();

        float angle = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg) - 90;

        transform.localPosition = (diff) * 0.1f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
