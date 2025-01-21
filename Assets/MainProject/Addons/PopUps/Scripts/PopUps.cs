using ModestTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
public class PopUps
{
    [Inject] private DiContainer diContainer;
    public enum PopUpType
    {
        PopUpProgress
    }

    public static string Path(PopUpType type)
    {
        return "PopUps/" + type.ToString();
    }
    public void ShowProgressOverlay()
    {
        ShowPopUp(PopUpType.PopUpProgress);
    }


    private GameObject ShowPopUp(PopUpType popUpType, bool overlayCanvas = true)
    {
        GameObject popUpObj = Resources.Load<GameObject>(Path(popUpType));
        Canvas canvas = null;
        if (popUpObj != null)
        {
            if (overlayCanvas)
            {
                canvas = GameObject.FindGameObjectWithTag("overlayCanvas").GetComponent<Canvas>();
                //    Debug.Log("adding to overlay canvas:");
            }
            else
            {
                canvas = GameObject.FindGameObjectWithTag("popUpCanvas").GetComponent<Canvas>();
            }
            popUpObj = diContainer.InstantiatePrefab(popUpObj, canvas.transform);
            popUpObj.GetComponent<PopUpBase>().ShowFader(overlayCanvas);
        }
        else
        {
            Debug.LogError("Pop up not found: " + popUpObj);
        }
        return popUpObj;
    }
}
