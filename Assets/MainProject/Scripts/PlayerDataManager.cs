using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Zenject;


public class PlayerDataManager
{
    [System.Serializable]
    public class PlayerData
    {
        public int score = 0;
    }

    private PlayerData playerData = new PlayerData();
    public UnityAction<int> onScoreChanged;
    [Inject] private FilesLoader filesLoader;
    public void Load(UnityAction actionOnLoaded = null)
    {

        if (File.Exists(GetPlayerSavePath()))
        {
            string jsonData = File.ReadAllText(GetPlayerSavePath());
            playerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);
            score = playerData.score;
        }
        else
        {
            ResetScore();
        }

        actionOnLoaded?.Invoke();

    }
    public void ResetScore()
    {
        score = filesLoader.GetJsonContents<Settings>(GlobalConstants.StartSettingsFileName).startingNumber;
    }
    
    public int score
    {
        get { return playerData.score; }
        set
        {
            playerData.score = value;
            onScoreChanged?.Invoke(value);
        }
    }
    public void Save()
    {

        string jsonData = JsonConvert.SerializeObject(playerData, Formatting.Indented);
        string savePath = GetPlayerSavePath();
        File.WriteAllText(savePath, jsonData);
    }

    private string GetPlayerSavePath()
    {
        return Application.persistentDataPath + "/playerData.json";
    }
}
