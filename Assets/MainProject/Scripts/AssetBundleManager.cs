using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AssetBundleManager:MonoBehaviour
{
    [Inject] private FilesLoader filesLoader;

    public Sprite LoadSpriteFromBundleByName(string bundleName,string spriteName )
    {
       var assetBundle = filesLoader.LoadAssetBundle(bundleName);
       return assetBundle.LoadAsset<Sprite>(spriteName);
    }
    
}