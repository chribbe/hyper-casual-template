using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioInAnimation : AutoDestroy
{
    public void OnEnable()
    {
        this.transform.parent = null;
    }
}
