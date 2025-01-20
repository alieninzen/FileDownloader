using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class LoadingSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var filesLoader = FindObjectOfType<FilesLoader>();

        if (filesLoader != null)
        {
            Container.Bind<FilesLoader>().FromInstance(filesLoader).AsSingle();
        }
        else
        {
            Debug.LogError("FilesLoader not found in the scene!");
        }
    }
}
