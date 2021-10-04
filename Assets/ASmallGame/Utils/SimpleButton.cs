using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SimpleButton : MonoBehaviour
{
    public UnityEvent onMouseDown;

    public bool selectable = false;

    public bool isSelected = false;
    public Transform selectedState;

    public int value = 0;
    public string valueString = "";

    void Start()
    {
        if (selectable)
        {
            if (selectedState)
            {
                selectedState.gameObject.SetActive(isSelected);
            }
        }
    }

    void OnMouseDown()
    {
        #if (UNITY_EDITOR)
                OnTouch();
        #endif
    }

    public virtual void OnTouch()
    {

        if (selectable)
        {
            isSelected = !isSelected;
        }

        if(selectedState)
        {
            selectedState.gameObject.SetActive(isSelected);
        }

        onMouseDown.Invoke();
    }

    public void UpdateState()
    {
        if (selectedState)
        {
            selectedState.gameObject.SetActive(isSelected);
        }
    }
}
