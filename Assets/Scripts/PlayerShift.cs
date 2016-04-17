using UnityEngine;
using System.Collections;

public class PlayerShift : MonoBehaviour
{

    public Sprite triangle;
    public Sprite circle;

    public Sprite crosshairFilled;
    public Sprite crosshairEmpty;


    private const KeyCode KEY_SHIFT = KeyCode.LeftShift;

    private SpriteRenderer thisRenderer;
    public SpriteRenderer crosshairRenderer;

    public const float shiftStaminaMax = 100f;
    public const float shiftStaminaRegen = 0.6f;
    public const float shiftStaminaUsageStill = 0.15f;
    public const float shiftStaminaUsageMoving = 1f;

    public float shiftStamina = 100f;

    public const float cooldownTime = 2f;
    public float cooldown = float.NegativeInfinity;

    public ProgressBar shiftEnergyBar;

    public enum Mode
    {
        Circle,
        Triangle
    };

    private static Mode currentMode = Mode.Circle;

    private static bool isMoving = false;

    // Use this for initialization
    void Start()
    {
        thisRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentMode == Mode.Triangle)
        {
            thisRenderer.sprite = triangle;
            crosshairRenderer.sprite = crosshairEmpty;
        } 
        else if (currentMode == Mode.Circle)
        {
            thisRenderer.sprite = circle;
            crosshairRenderer.sprite = crosshairFilled;
        }

        if (Input.GetKey(KEY_SHIFT) && Time.time > cooldown)
        {
            currentMode = Mode.Triangle;
        }
        else
        {
            currentMode = Mode.Circle;
        }

        if (currentMode == Mode.Circle)
        {
            shiftStamina += shiftStaminaRegen;
            shiftStamina = Mathf.Min(shiftStamina, shiftStaminaMax);
        }
        else if (currentMode == Mode.Triangle)
        {
            if (isMoving)
            {
                shiftStamina -= shiftStaminaUsageMoving;
            }
            else
            {
                shiftStamina -= shiftStaminaUsageStill;
            }
        }
        if (shiftStamina < 0)
        {
            shiftStamina = 0;
            currentMode = Mode.Circle;
            cooldown = Time.time + cooldownTime;
            shiftEnergyBar.Flash(cooldownTime);
        }

        shiftEnergyBar.progress = (shiftStamina / shiftStaminaMax);
    }

    public static void SetMoving(bool value)
    {
        isMoving = value;
    }

    public static Mode GetMode()
    {
        return currentMode;
    }
}
