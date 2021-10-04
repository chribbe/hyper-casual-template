using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenAnchor : MonoBehaviour
{
   
    public Camera viewCamera;
    public float cameraPlaneZ = 10.0f;
    public float top = -1.0f;
    public float bottom = -1.0f;
    public float left = -1.0f;
    public float right = -1.0f;
    public float verticalCenter = -1.0f;

    public bool lookAtCamera = false;

    void Start()
    {
        // if no camera specified use main camera
        if (!this.viewCamera)
        {
            Camera uicamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            this.viewCamera = uicamera;
        }


        if (lookAtCamera)
        {
            this.transform.LookAt(this.viewCamera.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!this.viewCamera) return;

        Vector3 topRight = this.viewCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cameraPlaneZ));
        Vector3 bottomLeft = this.viewCamera.ScreenToWorldPoint(new Vector3(0, 0, cameraPlaneZ));


        if(verticalCenter != -1)
        {
            this.transform.position = new Vector3(transform.position.x, topRight.y/2 - verticalCenter, transform.position.z);
        }
        if (top != -1)
        {
            this.transform.position = new Vector3(transform.position.x, topRight.y - top, transform.position.z);
        } else if(bottom != -1)
        {
            this.transform.position = new Vector3(transform.position.x, bottomLeft.y + bottom, transform.position.z);
        }


        if (left != -1)
        {
            this.transform.position = new Vector3(bottomLeft.x + left,transform.position.y, transform.position.z);
        }
        else if (right != -1)
        {
            this.transform.position = new Vector3(topRight.x - right, transform.position.y, transform.position.z);
        }

        if (lookAtCamera)
        {
            this.transform.LookAt(this.viewCamera.transform);
        }


        /*if(anchor == Anchor.Top)
         {
             this.transform.position = new Vector3(transform.position.x, topRight.y - offset.y,transform.position.z);
         } else if(anchor == Anchor.Bottom)
         {
             this.transform.position = new Vector3(transform.position.x, bottomLeft.y +(bottomLeft.y - offset.y), transform.position.z);
         }*/
    }
}
