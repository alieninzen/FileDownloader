using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class FilesLoader : MonoBehaviour
{
    [System.Serializable]
    public class FileData
    {
        public string Name;
        public int Size;
        public string download_url;
    }

    private FilesLoader instance;

    private List<FileData> filesList = new List<FileData>();
    private int totalSize = 0;
    private int downloadedSize = 0;

    private string localDirectory;

    private string apiURL = "https://api.github.com/repos/" + GlobalConstants.ServerRootPath;
    [SerializeField] private bool logEnabled;
    public UnityAction<float> onProgressChanged;
    public UnityAction<string> onLogMessage;
    private void Awake()
    {
        // Singleton initialization
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Сохраняем объект между сценами
    }


    public void StartLoading()
    {
        // Определяем путь для сохранения файлов
        localDirectory = Path.Combine(Application.persistentDataPath, "DownloadedFiles");

        // Создаем директорию, если её нет
        if (!Directory.Exists(localDirectory))
        {
            Directory.CreateDirectory(localDirectory);
        }

        StartCoroutine(LoadFilesList());
    }

    private IEnumerator LoadFilesList()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiURL))
        {
            request.SetRequestHeader("User-Agent", "UnityApp");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Парсим JSON
                var jsonResponse = request.downloadHandler.text;
                var files = JsonConvert.DeserializeObject<List<FileData>>(jsonResponse);

                // Добавляем файлы в список и рассчитываем общий размер
                filesList.Clear();
                totalSize = 0;
                foreach (var file in files)
                {
                    filesList.Add(file);
                    totalSize += file.Size;
                }

                LogMessage($"Loaded {filesList.Count} files, total size: {totalSize} bytes");

                // Запускаем загрузку файлов с прогрессом
                StartCoroutine(DownloadFilesWithProgress());
            }
            else
            {
                LogMessage($"Failed to load directory: {request.error}",true);
            }
        }
    }

    private IEnumerator DownloadFilesWithProgress()
    {
        downloadedSize = 0; // Сбрасываем прогресс
        foreach (var file in filesList)
        {
            string localFilePath = Path.Combine(localDirectory, file.Name);

            // Проверяем, существует ли файл локально
            if (File.Exists(localFilePath))
            {
                LogMessage($"File already exists locally: {file.Name}");
                downloadedSize += file.Size;
                ShowProgress(downloadedSize, totalSize);
            }
            else
            {
                yield return StartCoroutine(DownloadFile(file, localFilePath));
            }
        }

        LogMessage("All files downloaded successfully!");
    }

    private IEnumerator DownloadFile(FileData file, string localFilePath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(file.download_url))
        {
            request.SendWebRequest();

            // Показываем прогресс загрузки файла
            while (!request.isDone)
            {
                float fileProgress = request.downloadProgress * file.Size;
                ShowProgress(downloadedSize + (int)fileProgress, totalSize);
                yield return null;
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                LogMessage($"File downloaded: {file.Name}");
                downloadedSize += file.Size;

                // Сохраняем файл локально
                File.WriteAllBytes(localFilePath, request.downloadHandler.data);

                ShowProgress(downloadedSize, totalSize);
            }
            else
            {
                LogMessage($"Failed to download file {file.Name}: {request.error}",true);
            }
        }
    }

    public void ForceUpdate()
    {
        LogMessage("Starting force update...");

        StartCoroutine(ForceUpdateFiles());
    }

    private IEnumerator ForceUpdateFiles()
    {
        downloadedSize = 0; // Сбрасываем прогресс
        foreach (var file in filesList)
        {
            string localFilePath = Path.Combine(localDirectory, file.Name);

            // Перезаписываем файл
            LogMessage($"Forcing update for file: {file.Name}");
            yield return StartCoroutine(DownloadFile(file, localFilePath));
        }

        LogMessage("Force update completed. All files have been updated.");
    }
    private void LogMessage(string message,bool isError=false)
    {
        if (!logEnabled) return;
        if (isError)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
        onLogMessage?.Invoke(message);
    }
    private void ShowProgress(int current, int total)
    {
        float progress = (float)current / total;
        onProgressChanged?.Invoke(progress);
    }
}
