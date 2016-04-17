using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    private static GameObject thisGameObject;

    public GameObject gameOverScreen;
    public static GameObject staticGameOverScreen;

    public static bool IsDead = false;
    public static bool HasWon = false;

    // Use this for initialization
    void Start()
    {
        thisGameObject = gameObject;
        staticGameOverScreen = gameOverScreen;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Kill()
    {
        IsDead = true;
        Vector3 position = Camera.main.transform.position;
        Camera.main.transform.SetParent(null);
        Camera.main.transform.position = position;

        GameObject gameOverObj = Instantiate(staticGameOverScreen);
        gameOverObj.transform.SetParent(GameObject.Find("Canvas").transform, false);

        Destroy(thisGameObject);
    }
}
