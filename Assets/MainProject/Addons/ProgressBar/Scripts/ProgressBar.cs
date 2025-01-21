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
    private bool isLoaded = false; // Флаг для предотвращения повторного вызова onLoaded
    public UnityAction onLoaded;

    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        maxWidth = fullImage.rect.width;
        currentProgress = 0;
        isLoaded = false; // Сбрасываем флаг при инициализации
        UpdateProgressBar(0);
    }

    private void UpdateText(float progress)
    {
        if (progressText == null) return;
        progressText.text = (progress * 100).ToString("0") + "%";

        // Проверяем, достиг ли прогресс 100% и не вызывался ли onLoaded ранее
        if (progress >= 1 && !isLoaded)
        {
            isLoaded = true; // Устанавливаем флаг
            onLoaded?.Invoke(); // Вызываем onLoaded только один раз
        }
    }

    public void UpdateProgressBar(float targetProgress)
    {
        maxWidth = fullImage.rect.width;

        if (targetProgress > 1f) targetProgress = 1f;
        if (targetProgress < 0f) targetProgress = 0f;

        // Анимируем изменение прогресса
        DOTween.To(() => currentProgress, x => currentProgress = x, targetProgress, smoothTime)
            .OnUpdate(() =>
            {
                // Обновляем размер fillImage по мере анимации
                fillImage.offsetMax = new Vector2(-maxWidth * (1 - currentProgress), fillImage.offsetMax.y);
                UpdateText(currentProgress);
            }).SetEase(Ease.OutCubic);
    }
}
