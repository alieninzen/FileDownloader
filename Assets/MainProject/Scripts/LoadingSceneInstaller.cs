using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
      
        Container.Bind<FilesLoader>().FromInstance(FindObjectOfType<FilesLoader>()).AsSingle();
        Container.Bind<PlayerDataManager>().AsSingle();
        Container.Bind<CoroutineRunner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PopUps>().AsSingle();
        Container.Bind<AssetBundleManager>().AsSingle();
    }
}
