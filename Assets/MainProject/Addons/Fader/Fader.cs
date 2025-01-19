using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fader
{
    private Color backgroundColor = new Color(10.0f / 255.0f, 10.0f / 255.0f, 10.0f / 255.0f, 0.6f);
    private GameObject m_background;
    public GameObject FaderObj
    {
        get { return m_background; }
    }
    public void Show()
    {
        var canvas = GameObject.FindGameObjectWithTag("popUpCanvas").GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.Log("please add tag  <popUpCanvas> to canvas ");
        }
        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, backgroundColor);
        bgTex.Apply();

        m_background = new GameObject("PopupBackground");
        var image = m_background.AddComponent<Image>();
        var rect = new Rect(0, 0, bgTex.width, bgTex.height);
        var sprite = Sprite.Create(bgTex, rect, new Vector2(0.5f, 0.5f), 1);
        image.material.mainTexture = bgTex;
        image.sprite = sprite;
        var newColor = image.color;
        image.color = newColor;
        image.canvasRenderer.SetAlpha(0.0f);
        image.CrossFadeAlpha(1.0f, 0.3f, false);

        m_background.transform.localScale = new Vector3(1, 1, 1);
        m_background.GetComponent<RectTransform>().sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        m_background.transform.SetParent(canvas.transform, false);
        m_background.transform.SetAsLastSibling();
    }
    public void Hide()
    {
        m_background.GetComponent<Image>().raycastTarget = false;
        m_background.GetComponent<Image>().DOFade(0, 0.5f);
    }

}
