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
            Debug.LogWarning("FilesLoader not found in the scene!");
        }
        var coroutineRunner = FindObjectOfType<CoroutineRunner>();

        if (coroutineRunner != null)
        {
            Container.Bind<CoroutineRunner>().FromInstance(coroutineRunner).AsSingle();
        }
        else
        {
            Debug.LogWarning("CoroutineRunner not found in the scene!");
        }
        var assetBunldeManager = FindObjectOfType<AssetBundleManager>();
        if (assetBunldeManager != null)
        {
            Container.Bind<AssetBundleManager>().FromInstance(assetBunldeManager).AsSingle();
        }
        else
        {
            Debug.LogWarning("Asset bunle manager not found in the scene!");
        }
        Container.Bind<PopUps>().AsSingle();
     
        
    }
}
