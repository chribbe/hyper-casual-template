using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchManager : MonoBehaviour
{

    private static TouchManager _instance;
    public static TouchManager Instance { get { return _instance; } }

    public UnityEvent OnPlayerInput = new UnityEvent();

    private Camera UICamera;
    void Start()
    {
       
    }

    private void getUICamera()
    {
        if(!UICamera)
        {
            UICamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
            Debug.Log("[ToucManager] Found UI Camera");
        }
    }

    void Update()
    {
        getUICamera();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            OnPlayerInput.Invoke();

            Ray ray = UICamera.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.0f));
            RaycastHit2D hitInformation = Physics2D.GetRayIntersection(ray);

            if (hitInformation.collider != null)
            {
                GameObject touchedObject = hitInformation.transform.gameObject;

                if (touchedObject.GetComponent<SimpleButton>())
                {
                    touchedObject.GetComponent<SimpleButton>().OnTouch();
                }
            }
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
    }

}
