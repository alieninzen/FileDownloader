using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class BgChanger : MonoBehaviour
{
    [Inject] private AssetBundleManager assetBundleManager;
    [SerializeField] private BgFit bgFit;
    void Start()
    {
        LoadSprite();
    }

    private void LoadSprite()
    {
        var sprite = assetBundleManager.LoadSpriteFromBundleByName("sprites", "bg_tropic.png");
        bgFit.SetSprite(sprite);
    }
}
