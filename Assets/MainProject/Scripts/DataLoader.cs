using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using System;

public class DataLoader : MonoBehaviour
{
    [Inject] private FilesLoader filesLoader;
    [Inject] private PlayerDataManager playerManager;
    [Inject] private AssetBundleManager assetBundleManager;
    [SerializeField] private TextMeshProUGUI counterText;
    private int currentNumber {
        set { counterText.text = value.ToString(); }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerManager.Load();
    }
    private void OnEnable()
    {
        playerManager.onScoreChanged += RefreshScore;
        filesLoader.onFilesLoaded += ResetScore;
    }

  
    public void AddScore()
    {
        playerManager.score++;
    }
    private void RefreshScore(int newScore)
    {
        currentNumber = newScore;
    }

    private void OnDisable()
    {
        playerManager.onScoreChanged -= RefreshScore;
        filesLoader.onFilesLoaded -= ResetScore;
    }

    private void ResetScore()
    {
        playerManager.ResetScore();
    }

    private void OnApplicationQuit()
    {
        playerManager.Save();
    }
    
    public void ForceUpdate()
    {
        assetBundleManager.UnloadAllBundles();
        filesLoader.ForceUpdate();
    }
}
