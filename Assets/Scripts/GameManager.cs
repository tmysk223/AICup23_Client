using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Text debug;
    string logContent = "";
    string mapContent = "";

    private void Awake()
    {
        if(Instance  == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string getLog()
    {
        return logContent;
    }

    public void setLog(string log)
    {
        logContent = log;
    }

    public string getMap()
    {
        return mapContent;
    }
    public void setMap(string map) { 
        mapContent = map;   
    }

    public bool readyToPlay()
    {
        return mapContent.Length > 0 && logContent.Length > 0;
    }

    public void Debug(string text, float time)
    {
        StartCoroutine(RenderDebug(text, time));
    }

    IEnumerator RenderDebug(string text, float time)
    {
        debug.text = text;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            debug.color = Color.Lerp(new Color(1,1,1,1), new Color(1,1,1,0), t);
            yield return null;
        }
    }
}
