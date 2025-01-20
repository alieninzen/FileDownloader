using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using System;

public class GamePreloader : MonoBehaviour
{
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private Logger logger;
    [Inject] private FilesLoader filesLoader;
    void Start()
    {
        filesLoader.StartLoading();
    }
    private void OnEnable()
    {
        filesLoader.onProgressChanged += UpdateProgressBar;
        filesLoader.onLogMessage += UpdateLogText;
    }
    private void OnDisable()
    {
        filesLoader.onProgressChanged -= UpdateProgressBar;
        filesLoader.onLogMessage -= UpdateLogText;
    }

    private void UpdateLogText(string logMessage)
    {
        logger.LogMessage(logMessage);
    }

    private void UpdateProgressBar(float currentProgress)
    {
        progressBar.UpdateProgressBar(currentProgress);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
