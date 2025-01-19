using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform fullImage;
    [SerializeField] private RectTransform fillImage;
    [SerializeField] private TextMeshProUGUI progressText;

    private float maxWidth;
    private float currentProgress = 0;
    private float smoothTime = 1f;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        maxWidth = fullImage.rect.width;
        currentProgress = 0;
        UpdateProgressBar(0);
    }

    private void UpdateText(float progress)
    {
        if (progressText == null) return;
        progressText.text = (progress * 100).ToString("0") + "%";
    }

    public void UpdateProgressBar(float targetProgress)
    {
       
        maxWidth = fullImage.rect.width;

        if (targetProgress > 1f) targetProgress = 1f;
        if (targetProgress < 0f) targetProgress = 0f;

        // јнимируем изменение прогресса
        DOTween.To(() => currentProgress, x => currentProgress = x, targetProgress, smoothTime) // 0.5f - длительность анимации
            .OnUpdate(() =>
            {
                // ќбновл€ем размер fillImage по мере анимации
                fillImage.offsetMax = new Vector2(-maxWidth * (1 - currentProgress), fillImage.offsetMax.y);
                UpdateText(currentProgress);
            }).SetEase(Ease.OutCubic);
    }
}
