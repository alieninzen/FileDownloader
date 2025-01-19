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
        PopUpConnectionLost
    }

    public static string Path(PopUpType type)
    {
        return "PopUps/" + type.ToString();
    }

    public void ShowConnectionLostPopUp(MonoBehaviour monoBehaviour,UnityAction actionOnRetry)
    {
        ShowPopUp(PopUpType.PopUpConnectionLost,monoBehaviour).GetComponent<PopUpConnectionLost>().onRetryButtonClicked += actionOnRetry;
    }


    private GameObject ShowPopUp(PopUpType popUpType, MonoBehaviour monoBehaviour)
    {
        GameObject popUpObj = Resources.Load<GameObject>(Path(popUpType));
        if (popUpObj != null)
        {
            popUpObj = diContainer.InstantiatePrefab(popUpObj, monoBehaviour.transform);
        }
        else
        {
            Debug.LogError("Pop up not found: " + popUpObj);
        }
        return popUpObj;
    }
}
