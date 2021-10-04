using UnityEngine;
using System.Collections;

public class SineMove : MonoBehaviour
{
    public float duration = 1.0f;
    public float offset = 0.0f;
    public Vector3 values = new Vector3();
    private Vector3 startPos;

    private float startTime;
    void Start()
    {
        this.startPos = this.transform.position;
        this.startTime = Time.time;
    }

    void Update()
    {
        Vector3 pos = new Vector3();
        float add = simpleSine((Time.time-startTime)+offset, duration);

        pos.x = add* values.x;
        pos.y = add * values.y;
        pos.z = add * values.z;

        transform.position = pos + this.startPos;
    }

    float simpleSine(float time,float duration)
    {
        return Mathf.Sin((time * 2 * Mathf.PI) / duration);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 start;
        Vector3 end;
        if (!Application.isPlaying)
        {
            start = transform.position - values;
            end = transform.position + values;
        } else
        {
            start = this.startPos - values;
            end = this.startPos + values;
        }


        Gizmos.DrawLine(start, end);  
    }

    /*float oscillator(time, frequency = 1, amplitude = 1, phase = 0, offset = 0)
    {
        return Math.sin(time * frequency * Math.PI * 2 + phase * Math.PI * 2) * amplitude + offset;
    }*/
}
