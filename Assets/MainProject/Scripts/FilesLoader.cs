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
        public string Path;
        public int Size;
        public string download_url;
        public string type; // "file" or "dir"
    }

    private FilesLoader instance;

    private List<FileData> filesList = new List<FileData>();
    private int totalSize = 0;
    private int downloadedSize = 0;

    private string localDirectory;

    private string apiURL = "https://api.github.com/repos/" + GlobalConstants.ServerPath;
    private string rootFolder = "Test";
    [SerializeField] private bool logEnabled;

    public UnityAction<float> onProgressChanged;
    public UnityAction<string> onLogMessage;
    public UnityAction onFilesLoaded;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLoading()
    {
        localDirectory = Path.Combine(Application.persistentDataPath, "DownloadedFiles");

        if (!Directory.Exists(localDirectory))
        {
            Directory.CreateDirectory(localDirectory);
        }

        StartCoroutine(LoadFilesList(apiURL+rootFolder, localDirectory));
    }

    private IEnumerator LoadFilesList(string url, string currentDirectory)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "UnityApp");
            yield return request.SendWebRequest();
           
            if (request.result == UnityWebRequest.Result.Success)
            {
                var jsonResponse = request.downloadHandler.text;
                var files = JsonConvert.DeserializeObject<List<FileData>>(jsonResponse);

                foreach (var file in files)
                {
                    if (file.type == "file")
                    {
                        filesList.Add(file);
                        totalSize += file.Size;
                    }
                    else if (file.type == "dir")
                    {
                        string newLocalDirectory = Path.Combine(currentDirectory, file.Path);
                        if (!Directory.Exists(newLocalDirectory))
                        {
                            Directory.CreateDirectory(newLocalDirectory);
                        }
                       

                        yield return StartCoroutine(LoadFilesList(apiURL+file.Path, newLocalDirectory));
                    }
                }

                if (currentDirectory == localDirectory) // Запускаем загрузку только в самом начале
                {
                    LogMessage($"Loaded {filesList.Count} files, total size: {totalSize} bytes");
                    StartCoroutine(DownloadFilesWithProgress());
                }
            }
            else
            {
                LogMessage($"Failed to load directory: {request.error}", true);
            }
        }
    }

    private IEnumerator DownloadFilesWithProgress()
    {
        downloadedSize = 0;

        foreach (var file in filesList)
        {
            string localFilePath = Path.Combine(localDirectory, file.Path);

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
        onFilesLoaded?.Invoke();
        LogMessage("All files downloaded successfully!");
    }

    private IEnumerator DownloadFile(FileData file, string localFilePath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(file.download_url))
        {
            request.SendWebRequest();

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
                File.WriteAllBytes(localFilePath, request.downloadHandler.data);
                ShowProgress(downloadedSize, totalSize);
            }
            else
            {
                LogMessage($"Failed to download file {file.Name}: {request.error}", true);
            }
        }
    }

    public byte[] GetFileContents(string fileName)
    {
        // Ищем файл в списке загруженных
        var file = filesList.Find(f => f.Name == fileName);
        if (file == null)
        {
            LogMessage($"Файл с именем {fileName} не найден", true);
            return null;
        }

        // Локальный путь к файлу
        string localFilePath = Path.Combine(localDirectory, file.Path);

        // Проверяем, существует ли файл
        if (!File.Exists(localFilePath))
        {
            LogMessage($"Файл {fileName} отсутствует в локальной директории: {localFilePath}", true);
            return null;
        }

        // Читаем содержимое файла
        return File.ReadAllBytes(localFilePath);
    }

    public AssetBundle LoadAssetBundle(string fileName)
    {
        byte[] fileContents = GetFileContents(fileName);
        if (fileContents == null)
        {
            LogMessage($"Не удалось загрузить AssetBundle: содержимое файла {fileName} не найдено", true);
            return null;
        }

        // Загружаем AssetBundle из массива байт
        AssetBundle assetBundle = AssetBundle.LoadFromMemory(fileContents);

        if (assetBundle == null)
        {
            LogMessage($"Ошибка загрузки AssetBundle из файла {fileName}", true);
        }

        return assetBundle;
    }

    private void LogMessage(string message, bool isError = false)
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
