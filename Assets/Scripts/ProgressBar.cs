using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {

    public float progress;

    public bool isFlashing = false;
    public float flashingUntil = float.NegativeInfinity;
    public const float FLASH_INTERVAL = 0.25f;
    public float flashIntervalProgress = 0.0f;

    private Image thisImage;

	// Use this for initialization
	void Start () {
        thisImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        thisImage.fillAmount = progress;
        if (Time.time <= flashingUntil)
        {
            flashIntervalProgress += Time.deltaTime;
            if (flashIntervalProgress >= FLASH_INTERVAL)
            {
                isFlashing = !isFlashing;
                flashIntervalProgress = 0f;
            }
        }
        else if (Time.time > flashingUntil)
        {
            isFlashing = false;
            flashIntervalProgress = 0f;
        }
        if (isFlashing)
        {
            thisImage.fillAmount = 0f;
        }
	}

    public void Flash (float duration)
    {
        flashingUntil = Time.time + duration;
    }
}
