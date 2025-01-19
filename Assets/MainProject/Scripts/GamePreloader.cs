using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePreloader : MonoBehaviour
{
    [SerializeField] private ProgressBar progressBar;
    void Start()
    {
        progressBar.UpdateProgressBar(0.5f);
        progressBar.UpdateProgressBar(1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
