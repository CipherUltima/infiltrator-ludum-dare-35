using UnityEngine;
using System.Collections;

public class ScriptedGib : MonoBehaviour {

    public Rigidbody2D gib0;
    public Rigidbody2D gib1;
    public Rigidbody2D gib2;

    public AudioClip death0;
    public AudioClip death1;
    public AudioClip death2;

    public AudioSource source;

	// Use this for initialization
	void Start () {
        Vector2 g0vec = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.01f;
        Vector2 g1vec = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.01f;
        Vector2 g2vec = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 0.01f;

        float g0tor = Random.Range(-1f, 1f);
        float g1tor = Random.Range(-1f, 1f);
        float g2tor = Random.Range(-1f, 1f);

        gib0.AddTorque(g0tor, ForceMode2D.Impulse);
        gib1.AddTorque(g1tor, ForceMode2D.Impulse);
        gib2.AddTorque(g2tor, ForceMode2D.Impulse);

        gib0.AddForce(g0vec, ForceMode2D.Impulse);
        gib1.AddForce(g1vec, ForceMode2D.Impulse);
        gib2.AddForce(g2vec, ForceMode2D.Impulse);

        int sound = Random.Range(0, 3);
        switch (sound)
        {
            case 0:
                source.clip = death0;
                break;
            case 1:
                source.clip = death1;
                break;
            case 2:
                source.clip = death2;
                break;
            default:
                source.clip = death0;
                break;
        }

        source.Play();
	}
}
