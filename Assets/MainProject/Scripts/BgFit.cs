using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BgFit : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image img;
    private void Awake()
    {
        img = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }


    public void SetSprite(Sprite spr)
    {
        // Получаем ссылку на Canvas
        img.sprite = spr;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError("This script requires the Canvas to be in Screen Space Overlay mode.");
            return;
        }

        // Размеры экрана (в пикселях)
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Получаем размеры и соотношение изображения
        Sprite sprite = GetComponent<Image>().sprite;
        float imageWidth = sprite.rect.width;
        float imageHeight = sprite.rect.height;
        float imageAspect = imageWidth / imageHeight;

        // Получаем размеры базового разрешения Canvas
        Vector2 referenceResolution = canvas.GetComponent<CanvasScaler>().referenceResolution;
        float referenceAspect = referenceResolution.x / referenceResolution.y;

        // Сравниваем соотношение сторон экрана и базового разрешения
        if ((screenWidth / screenHeight) > referenceAspect)
        {
            // Экран шире, чем базовое разрешение: растягиваем по ширине
            rectTransform.sizeDelta = new Vector2(referenceResolution.x, referenceResolution.x / imageAspect);
        }
    }
}