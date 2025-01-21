using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BgFit : MonoBehaviour
{
    private RectTransform imageRect;
    private Image img;
    private Canvas canvas;

    private void Awake()
    {
        img = GetComponent<Image>();
        imageRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetSprite(Sprite spr)
    {
        // Устанавливаем спрайт
        img.sprite = spr;

        // Получаем размеры Canvas
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        float canvasWidth = canvasRect.rect.width;

        // Проверяем, если ширина картинки больше ширины экрана
        if (imageRect.rect.width > canvasWidth)
        {
           
            imageRect.sizeDelta = new Vector2(imageRect.rect.width*2, canvasRect.rect.height);
            return;
        }

        float imageAspect = imageRect.rect.width / imageRect.rect.height;
        float newHeight = canvasWidth / imageAspect;
        imageRect.sizeDelta = new Vector2(canvasWidth, newHeight);
    }
}
