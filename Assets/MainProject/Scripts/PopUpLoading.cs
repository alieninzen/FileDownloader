using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class PopUpLoading : PopUpBase
{
    [Inject] private FilesLoader filesLoader;
    [SerializeField] private ProgressBar progressBar;

    private void OnEnable()
    {
        progressBar.UpdateProgressBar(0);
        progressBar.onLoaded += Close;
        filesLoader.onFilesLoaded += ChangeScene;
        filesLoader.onProgressChanged += UpdateProgres; 
    }

    private void ChangeScene()
    {
       
    }

    private void UpdateProgres(float progress)
    {
        progressBar.UpdateProgressBar(progress);
    }

    private void OnDisable()
    {
        filesLoader.onProgressChanged -= UpdateProgres;
    }
}
