using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GamePreloader : MonoBehaviour
{
    [SerializeField] private Logger logger;
    [Inject] private FilesLoader filesLoader;
    [Inject] private PopUps popUps;
    void Start()
    {
        popUps.ShowProgressOverlay();
        filesLoader.onFilesLoaded += ChangeScene;
        filesLoader.StartLoading();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnEnable()
    {
        filesLoader.onLogMessage += UpdateLogText;
    }
    private void OnDisable()
    {
        filesLoader.onLogMessage -= UpdateLogText;
    }

    private void UpdateLogText(string logMessage)
    {
        logger.LogMessage(logMessage);
    }
}
