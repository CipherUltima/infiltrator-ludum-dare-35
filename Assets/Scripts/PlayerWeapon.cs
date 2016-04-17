using UnityEngine;
using System.Collections;

public class PlayerWeapon : MonoBehaviour {

    public ProgressBar armbladeChargeBar;

    public AudioSource audioSrc;
    public AudioClip chargeClip1;
    public AudioClip attackClip1;
    public AudioClip attackClip2;
    public AudioClip attackClip3;

    public const KeyCode CHARGE_KEY = KeyCode.Mouse0;
    public const float CHARGE_RATE = 1.0f;

    public float chargeAmount = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(CHARGE_KEY))
        {
            chargeAmount += CHARGE_RATE;
        }
        if (Input.GetKeyUp(CHARGE_KEY))
        {
            if (chargeAmount > 50.0f)
            {
                Fire();
            }
            chargeAmount = 0;
        }

        armbladeChargeBar.progress = (chargeAmount / 50f);
	}

    void Fire ()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDiff = (mouseWorldPosition - transform.position);
        mouseDiff.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, mouseDiff, 1f, LayerMask.GetMask("Enemy"));
        if (hit.collider != null)
        {
            AIEnemy possibleEnemy = hit.transform.gameObject.GetComponent<AIEnemy>();
            if (possibleEnemy != null)
            {
                possibleEnemy.Kill();
            }
        } 
        
        int sound = Random.Range(0, 3);
        switch (sound)
        {
            case 0:
                audioSrc.clip = attackClip1;
                break;
            case 1:
                audioSrc.clip = attackClip2;
                break;
            case 2:
                audioSrc.clip = attackClip3;
                break;
            default:
                audioSrc.clip = attackClip1;
                break;
        }
        audioSrc.Play();
    }
}
