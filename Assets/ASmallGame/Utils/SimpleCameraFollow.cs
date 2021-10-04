using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    private Vector3 startOffset;
    public Transform target;
    public float smooth = 0.3f;
    private Vector3 velocity = Vector3.zero;
    public bool lockY;
    private Vector3 startPos;
    void Start()
    {
        startPos = this.transform.position;
        startOffset = this.transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPosition = this.target.position + startOffset;
        this.transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smooth);

        if(lockY)
        {
            this.transform.position = new Vector3(transform.position.x, startPos.y, transform.position.z);
        }

        //this.transform.LookAt()
    }
}
