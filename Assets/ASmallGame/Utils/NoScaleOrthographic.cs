using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoScaleOrthographic : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera; 
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainCamera) return;

        float height = mainCamera.orthographicSize * 2;
        float width = height * Screen.width / Screen.height; // basically height * screen aspect ratio
        this.transform.localScale = Vector3.one * height / 20f;
    }
}
