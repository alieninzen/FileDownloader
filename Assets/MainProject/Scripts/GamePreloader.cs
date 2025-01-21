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
  
    void Start()
    {
      
        filesLoader.StartLoading();
    }
   
    private void ChangeScene()
    {
        SceneManager.LoadScene("Game");
    }

    private void OnEnable()
    {
        filesLoader.onLogMessage += UpdateLogText;
        filesLoader.onFilesLoaded += ChangeScene;
    }
    private void OnDisable()
    {
        filesLoader.onLogMessage -= UpdateLogText;
        filesLoader.onFilesLoaded -= ChangeScene;
    }

    private void UpdateLogText(string logMessage)
    {
        logger.LogMessage(logMessage);
    }
}
