using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxLifeSeconds = 10.0f;

    private float startTime; 
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > startTime + maxLifeSeconds )
        {
            Destroy(this.gameObject);
        }
    }
}
