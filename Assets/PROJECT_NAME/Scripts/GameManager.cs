using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State { Start, Playing, Fail, Win };

  
    public bool ui = true;
    [HideInInspector]
    public bool debugInfo = false;
    [HideInInspector]
    public bool haptics = true;

    public State currentState { get; private set; } = State.Start;
    public UnityEvent OnStateChange;


    [HideInInspector]
    public LocaleManager localeManager;
    public TextAsset localeJson;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Input.simulateMouseWithTouches = true;

        /*UniversalRenderPipelineAsset urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        urp.renderScale = 0.75f;*/
    }

    public void SetState(State state)
    {
        if (state == this.currentState) return;

        currentState = state;
        OnStateChange.Invoke();
    }

    public void DebugMenu()
    {
        SceneManager.LoadScene("DebugMenu");
    }

  

    void Update()
    {
        if (currentState == State.Start)
        {
          
        }

        if (currentState == State.Playing)
        {
            
        }
    }

  
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }


        this.localeManager = new LocaleManager();

        if (this.localeJson)
            this.localeManager.init(this.localeJson.text);
        else
        {
            Debug.Log("[GameManger] No LocaleJson file set. LocaleManager will not init.");

        }

        Debug.Log("[GameManager] Inited. Let's go!");

    }

}
