using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowsManager : MonoBehaviour
{
    private List<Fader> faders = new List<Fader>();
    private Canvas canvas;

    private static WindowsManager instance;
    private static bool isDestroyed = false;

    public static WindowsManager Instance
    {
        get
        {
            if (instance == null && isDestroyed)
            {
                instance = FindObjectOfType<WindowsManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<WindowsManager>();
                    singletonObject.name = "WindowsManager";
                }
            }

            return instance;
        }
    }
    public static void Reset()
    {
        isDestroyed = true;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            canvas = GameObject.FindGameObjectWithTag("popUpCanvas").GetComponent<Canvas>();
            if (canvas == null)
            {
                Debug.Log("please add tag  <popUpCanvas> to canvas ");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        faders.Clear();
        isDestroyed = true;
    }

    private void HideFader()
    {
        if (faders.Count == 0) return; 
        var lastFader = faders[faders.Count - 1];
        faders.Remove(lastFader);
        lastFader.Hide();
        Destroy(lastFader.FaderObj, 0.5f);
        
    }
    private void OnEnable()
    {
     
    }

    public void Show()
    {
        ShowFader();
    }
    public void Hide()
    {
        HideFader();
    }

    private void ShowFader()
    {
        Fader screenFade = new Fader();
        screenFade.Show();
        faders.Add(screenFade);
    }
}
