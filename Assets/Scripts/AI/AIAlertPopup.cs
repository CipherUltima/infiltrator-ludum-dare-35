using UnityEngine;
using System.Collections;

public class AIAlertPopup : MonoBehaviour
{
    public GameObject parent;

    private const float DISPLAY_TIME = 1.0f;
    private const float FADE_TIME = 0.5f;

    private float timeAlerted = float.NegativeInfinity;

    private bool isAlreadyAlerted = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float timeDifference = Time.time - timeAlerted;
        float timeForFade = timeDifference - DISPLAY_TIME;
        float alpha = (1f) - ((1 / FADE_TIME) * timeForFade);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color newColor = sr.color;
        newColor.a = alpha;
        sr.color = newColor;

        if (Time.time > (timeAlerted + DISPLAY_TIME + FADE_TIME))
        {
            gameObject.SetActive(false);
        }

        transform.position = parent.transform.position + new Vector3(0, 0.35f, 0);
        transform.rotation = Quaternion.identity;
    }

    public void Alert()
    {
        if (!isAlreadyAlerted)
        {
            gameObject.SetActive(true);
            timeAlerted = Time.time;
            isAlreadyAlerted = true;
            GetComponent<AudioSource>().Play();
        }

    }

    public void Reset()
    {
        isAlreadyAlerted = false;
    }
}