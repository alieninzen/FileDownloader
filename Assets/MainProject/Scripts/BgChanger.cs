using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class BgChanger : MonoBehaviour
{
    [Inject] private AssetBundleManager assetBundleManager;
    [Inject] private FilesLoader filesLoader;
    [SerializeField] private BgFit bgFit;
    void Start()
    {
        LoadSprite();

    }
    private void OnEnable()
    {
        filesLoader.onFilesLoaded += LoadSprite;
    }
    private void OnDisable()
    {
        filesLoader.onFilesLoaded -= LoadSprite;
    }
    private void LoadSprite()
    {
        var sprite = assetBundleManager.LoadSpriteFromBundleByName("sprites", "bg_tropic.png");
        bgFit.SetSprite(sprite);
    }
}
