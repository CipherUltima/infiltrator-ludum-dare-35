using UnityEngine;
using System.Collections;

public class VictoryCondition : MonoBehaviour {

    public GameObject victoryOverlay;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D (Collision2D collision)
    {
        if (collision.transform.gameObject == GameObject.FindGameObjectWithTag("Player"))
        {
            GameObject gameOverObj = Instantiate(victoryOverlay);
            gameOverObj.transform.SetParent(GameObject.Find("Canvas").transform, false);
            Player.HasWon = true;
        }
    }
}
