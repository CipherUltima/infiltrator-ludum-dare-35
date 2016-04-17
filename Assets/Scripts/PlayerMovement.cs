using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    private const float MOVE_SPEED = 0.07f;

    private const KeyCode MOVE_UP = KeyCode.W;
    private const KeyCode MOVE_DOWN = KeyCode.S;
    private const KeyCode MOVE_LEFT = KeyCode.A;
    private const KeyCode MOVE_RIGHT = KeyCode.D;

    private static Transform thisTransform;
    private static Rigidbody2D thisRigidbody;

    private static bool canMove = true;

	// Use this for initialization
	void Start () {
        thisTransform = GetComponent<Transform>();
        thisRigidbody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (!thisTransform || !thisRigidbody)
        {
            Debug.LogError("PlayerMovement missing Transform or Rigidbody2D!");
        }
        else if (!canMove)
        {
            return;
        }
        else
        {
            Vector3 totalMovement = new Vector3();
            if (Input.GetKey(MOVE_UP))
            {
                totalMovement += new Vector3(0, MOVE_SPEED, 0);
            }
            if (Input.GetKey(MOVE_DOWN))
            {
                totalMovement += new Vector3(0, -MOVE_SPEED, 0);
            }
            if (Input.GetKey(MOVE_LEFT))
            {
                totalMovement += new Vector3(-MOVE_SPEED, 0, 0);
            }
            if (Input.GetKey(MOVE_RIGHT))
            {
                totalMovement += new Vector3(MOVE_SPEED, 0, 0);
            }
            if (totalMovement == new Vector3())
            {
                PlayerShift.SetMoving(false);
            }
            else
            {
                PlayerShift.SetMoving(true);
            }
            TranslateWithCollisions(totalMovement);
        }
	}

    private void TranslateWithCollisions(Vector3 movement)
    {
        Vector3 result = movement + transform.position;
        thisRigidbody.MovePosition(result);
    }

    public static void SetMovement(bool value = true)
    {
        canMove = value;
    }

    public static Vector3 GetPosition()
    {
        if (thisTransform != null)
            return thisTransform.position;
        else
            return new Vector3(500, 500, 500);
    }
}
