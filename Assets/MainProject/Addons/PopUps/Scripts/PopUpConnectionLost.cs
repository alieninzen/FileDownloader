using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopUpConnectionLost : PopUpBase
{
    [SerializeField] private Button retryButton;
    public UnityAction onRetryButtonClicked;

    public void RetryButtonClicked()
    {
        Debug.Log("retry button clicked");
        onRetryButtonClicked?.Invoke();
        onRetryButtonClicked = null;
        Close();
    }
}
