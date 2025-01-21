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
        // �������� ������ �� Canvas
        img.sprite = spr;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null || canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogError("This script requires the Canvas to be in Screen Space Overlay mode.");
            return;
        }

        // ������� ������ (� ��������)
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // �������� ������� � ����������� �����������
        Sprite sprite = GetComponent<Image>().sprite;
        float imageWidth = sprite.rect.width;
        float imageHeight = sprite.rect.height;
        float imageAspect = imageWidth / imageHeight;

        // �������� ������� �������� ���������� Canvas
        Vector2 referenceResolution = canvas.GetComponent<CanvasScaler>().referenceResolution;
        float referenceAspect = referenceResolution.x / referenceResolution.y;

        // ���������� ����������� ������ ������ � �������� ����������
        if ((screenWidth / screenHeight) > referenceAspect)
        {
            // ����� ����, ��� ������� ����������: ����������� �� ������
            rectTransform.sizeDelta = new Vector2(referenceResolution.x, referenceResolution.x / imageAspect);
        }
    }
}