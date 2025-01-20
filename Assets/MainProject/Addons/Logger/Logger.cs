using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab; // ������ ������ � TextMeshPro
    [SerializeField] private Transform contentParent; // �������� ��� ������ (������ � VerticalLayoutGroup)
    [SerializeField] private int maxMessages = 10; // ����� ��������� �� ������
    [SerializeField] private float messageLifetime = 3f; // ����� ����� ���������
    [SerializeField] private float fadeDuration = 1f; // ������������ �������� ������������
    private Logger instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // ������� ����������� ������
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // ��������� ������
        if (contentParent == null)
        {
            Debug.LogError("Content parent not assigned!");
        }

        if (textPrefab == null)
        {
            Debug.LogError("Text prefab not assigned!");
        }
    }


    public void LogMessage(string message)
    {
        if (contentParent.childCount >= maxMessages)
        {
            Destroy(contentParent.GetChild(0).gameObject); // ������� ����� ������ ���������
        }

        GameObject newMessage = Instantiate(textPrefab, contentParent); // ������� ����� ������ ������
        TMP_Text textComponent = newMessage.GetComponent<TMP_Text>();
        textComponent.text = message;

        StartCoroutine(HandleMessageLifetime(newMessage, textComponent)); // ��������� ������ �����
    }


    private IEnumerator HandleMessageLifetime(GameObject messageObject, TMP_Text textComponent)
    {
        yield return new WaitForSeconds(messageLifetime); // ���� ����� �����

        float elapsed = 0f;
        Color originalColor = textComponent.color;

        // ������� ������������
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(messageObject); // ������� ������ ����� ������������
    }
}