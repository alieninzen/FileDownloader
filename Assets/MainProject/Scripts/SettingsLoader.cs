using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
public class SettingsLoader : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        public int startingNumber = 1;
    }
    [Inject] private FilesLoader filesLoader;
    [SerializeField] private TextMeshProUGUI counterText;
    private int currentNumber {
        set { counterText.text = value.ToString(); }
    }
    // Start is called before the first frame update
    void Start()
    {
      // currentNumber = filesLoader.GetJsonContents<Settings>(GlobalConstants.StartSettingsFileName).startingNumber;
       
    }

   
}
