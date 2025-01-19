using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

public class ScreenChanger : MonoBehaviour
{

    public static ScreenChanger instance;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 1.3f;
    [SerializeField] private Image bg;
    private AsyncOperation nextSceneLoader;
    public UnityAction<float> onProgressChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(instance.gameObject);

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadScene(Scenes scene)
    {
        ShowScreenChanger(scene);
    }

    private IEnumerator LoadSceneAsync(Scenes scene)
    {
        nextSceneLoader = SceneManager.LoadSceneAsync((int)scene);
        while (!nextSceneLoader.isDone)
        {
            float progress = Mathf.Clamp01(nextSceneLoader.progress / 0.9f);
            SetProgress(progress);
            yield return null;
        }
        SetProgress(1);
        HideScrennChanger();
        Debug.Log("scene loaded");
        nextSceneLoader.allowSceneActivation = true;
    }

    private void SetProgress(float progress)
    {
        onProgressChanged?.Invoke(progress);
    }
   

    public void ShowScreenChanger(Scenes scene)
    {
        bg.raycastTarget = true;
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        canvasGroup.DOFade(1, fadeTime).onComplete+=() => StartCoroutine(LoadSceneAsync(scene));
    }
    private void DisableRayCast()
    {
        bg.raycastTarget = false;
    }
    public void HideScrennChanger()
    {
        canvasGroup.DOFade(0, fadeTime).onComplete += DisableRayCast;
    }
}
