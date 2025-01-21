using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform fullImage;
    [SerializeField] private RectTransform fillImage;
    [SerializeField] private TextMeshProUGUI progressText;

    private float maxWidth;
    private float currentProgress = 0;
    private float smoothTime = 1f;
    private bool isLoaded = false; // ���� ��� �������������� ���������� ������ onLoaded
    public UnityAction onLoaded;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        maxWidth = fullImage.rect.width;
        currentProgress = 0;
        isLoaded = false; // ���������� ���� ��� �������������
        UpdateProgressBar(0);
    }

    private void UpdateText(float progress)
    {
        if (progressText == null) return;
        progressText.text = (progress * 100).ToString("0") + "%";

        // ���������, ������ �� �������� 100% � �� ��������� �� onLoaded �����
        if (progress >= 1 && !isLoaded)
        {
            isLoaded = true; // ������������� ����
            onLoaded?.Invoke(); // �������� onLoaded ������ ���� ���
        }
    }

    public void UpdateProgressBar(float targetProgress)
    {
        maxWidth = fullImage.rect.width;

        if (targetProgress > 1f) targetProgress = 1f;
        if (targetProgress < 0f) targetProgress = 0f;

        // ��������� ��������� ���������
        DOTween.To(() => currentProgress, x => currentProgress = x, targetProgress, smoothTime)
            .OnUpdate(() =>
            {
                // ��������� ������ fillImage �� ���� ��������
                fillImage.offsetMax = new Vector2(-maxWidth * (1 - currentProgress), fillImage.offsetMax.y);
                UpdateText(currentProgress);
            }).SetEase(Ease.OutCubic);
    }
}
