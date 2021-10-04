using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoScalePerspective : MonoBehaviour
{
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float size = (mainCamera.transform.position - transform.position).magnitude;
        transform.localScale = new Vector3(size*.1f, size*.1f, size*.1f);
    }
}
