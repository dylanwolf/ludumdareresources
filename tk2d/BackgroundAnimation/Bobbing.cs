using UnityEngine;
using System.Collections;

public class Bobbing : MonoBehaviour
{

    public float Period = 1;
    public float Amplitude = 0.25f;

    private float lastdY = 0;
    private float thisdY = 0;
    private Vector3 translation = new Vector3(0, 0, 0);
    private float timer = 0;

    void Start()
    {
        timer = Random.Range(0, Period);
        Period += (Period * Random.Range(-0.05f, 0.05f));
        Amplitude += (Amplitude * Random.Range(-0.05f, 0.05f));		
    }

	void FixedUpdate ()
	{
	    timer += Time.deltaTime;

        thisdY = Amplitude * Mathf.Sin((Mathf.PI*2*timer)/Period);
	    translation.y = thisdY - lastdY;
        transform.Translate(translation);

	    lastdY = thisdY;
	    timer %= Period;
	}
}
