using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    [SerializeField] private GameObject textPrefab; // Префаб текста с TextMeshPro
    [SerializeField] private Transform contentParent; // Родитель для текста (объект с VerticalLayoutGroup)
    [SerializeField] private int maxMessages = 10; // Лимит сообщений на экране
    [SerializeField] private float messageLifetime = 3f; // Время жизни сообщения
    [SerializeField] private float fadeDuration = 1f; // Длительность плавного исчезновения
    private Logger instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Удаляем дублирующий объект
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Сохраняем объект
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
            Destroy(contentParent.GetChild(0).gameObject); // Удаляем самое старое сообщение
        }

        GameObject newMessage = Instantiate(textPrefab, contentParent); // Создаем новый объект текста
        TMP_Text textComponent = newMessage.GetComponent<TMP_Text>();
        textComponent.text = message;

        StartCoroutine(HandleMessageLifetime(newMessage, textComponent)); // Запускаем таймер жизни
    }


    private IEnumerator HandleMessageLifetime(GameObject messageObject, TMP_Text textComponent)
    {
        yield return new WaitForSeconds(messageLifetime); // Ждем время жизни

        float elapsed = 0f;
        Color originalColor = textComponent.color;

        // Плавное исчезновение
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(messageObject); // Удаляем объект после исчезновения
    }
}