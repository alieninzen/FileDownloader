using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AssetBundleManager
{
    [Inject] private FilesLoader filesLoader;
    private List<AssetBundle> loadedBundles = new List<AssetBundle>();
    public Sprite LoadSpriteFromBundleByName(string bundleName,string spriteName )
    {
    
       var assetBundle = filesLoader.LoadAssetBundle(bundleName);
        if (loadedBundles.Contains(assetBundle) == false) loadedBundles.Add(assetBundle);
        return assetBundle.LoadAsset<Sprite>(spriteName);
    }
    public void UnloadAllBundles()
    {
        foreach(var assetBundle in loadedBundles)
        {
            assetBundle.Unload(true);
        }
        loadedBundles.Clear();
    }
}