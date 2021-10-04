using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    // Start is called before the first frame update
    public float minTimeSeconds = 0.0f;
    private float startTime;

    private bool readyToStart = false;
    
    void Start()
    {
        Application.targetFrameRate = 60;
        startTime = Time.time;
        StartCoroutine(LoadYourAsyncScene());
        readyToStart = true;

    }

    void Update()
    {

    }


    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");
        asyncLoad.allowSceneActivation = false;


        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {

          
            if(Time.time >  startTime + minTimeSeconds && readyToStart)
            {
                asyncLoad.allowSceneActivation = true;
            }


            yield return null;
        }
    }

}
