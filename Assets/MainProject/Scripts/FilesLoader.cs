using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Zenject;
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

    private List<FileData> filesList = new List<FileData>();
    private int totalSize = 0;
    private int downloadedSize = 0;

    private string localDirectory;
    private string metadataFilePath;

    private string apiURL = "https://api.github.com/repos/" + GlobalConstants.ServerPath;
    private string rootFolder = "Test";
    [SerializeField] private bool logEnabled;

    public UnityAction<float> onProgressChanged;
    public UnityAction<string> onLogMessage;
    public UnityAction onFilesLoaded;

    private FilesLoader instance;
    [Inject] private PopUps popUps;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Удаляем дублирующий объект
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Сохраняем объект
    }
    public void StartLoading()
    {
        popUps.ShowProgressOverlay();
        localDirectory = Path.Combine(Application.persistentDataPath, "DownloadedFiles");
        metadataFilePath = Path.Combine(localDirectory, "files_metadata.json");

        if (!Directory.Exists(localDirectory))
        {
            Directory.CreateDirectory(localDirectory);
        }

        if (File.Exists(metadataFilePath))
        {
            LogMessage("Using local metadata.");
            LoadLocalMetadata();
            StartCoroutine(DownloadFilesWithProgress());
        }
        else
        {
            LogMessage("Downloading metadata from server...");
            StartCoroutine(LoadFilesList(apiURL + rootFolder, localDirectory));
        }
    }

    private void LoadLocalMetadata()
    {
        string json = File.ReadAllText(metadataFilePath);
        filesList = JsonConvert.DeserializeObject<List<FileData>>(json);
        totalSize = CalculateTotalSize();
    }

    private IEnumerator LoadFilesList(string url, string currentDirectory)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            LogMessage($"Loading: {url}");
            request.SetRequestHeader("User-Agent", "UnityApp");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
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
                        yield return StartCoroutine(LoadFilesList(apiURL + file.Path, newLocalDirectory));
                    }
                }

                if (currentDirectory == localDirectory)
                {
                    SaveDownloadedFilesInfo();
                    LogMessage("Metadata downloaded and saved locally.");
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

            if (IsFileUpToDate(file, localFilePath))
            {
                LogMessage($"File is up to date: {file.Name}");
                downloadedSize += file.Size;
                ShowProgress(downloadedSize, totalSize);
            }
            else
            {
                yield return StartCoroutine(DownloadFile(file, localFilePath));
            }
        }

        LogMessage("All files downloaded successfully!");
        onFilesLoaded?.Invoke();
    }

    private bool IsFileUpToDate(FileData file, string localFilePath)
    {
        if (!File.Exists(localFilePath)) return false;

        FileInfo fileInfo = new FileInfo(localFilePath);
        return fileInfo.Length == file.Size;
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
                File.WriteAllBytes(localFilePath, request.downloadHandler.data);
                LogMessage($"File downloaded: {file.Name}");
                downloadedSize += file.Size;
                ShowProgress(downloadedSize, totalSize);
            }
            else
            {
                LogMessage($"Failed to download file {file.Name}: {request.error}", true);
            }
        }
    }

    private void SaveDownloadedFilesInfo()
    {
        string json = JsonConvert.SerializeObject(filesList, Formatting.Indented);
        File.WriteAllText(metadataFilePath, json);
        LogMessage("Saved metadata locally.");
    }

    private int CalculateTotalSize()
    {
        int size = 0;
        foreach (var file in filesList)
        {
            size += file.Size;
        }
        return size;
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
    public byte[] GetFileContents(string fileName)
    {
        // Find the file in the loaded list
        var file = filesList.Find(f => f.Name == fileName);
        if (file == null)
        {
            LogMessage($"File with name {fileName} not found", true);
            return null;
        }

        // Local path to the file
        string localFilePath = Path.Combine(localDirectory, file.Path);

        // Check if the file exists
        if (!File.Exists(localFilePath))
        {
            LogMessage($"File {fileName} is missing in the local directory: {localFilePath}", true);
            return null;
        }

        // Read the file content
        return File.ReadAllBytes(localFilePath);
    }

    public T GetJsonContents<T>(string fileName)
    {
        // Get the file content as a byte array
        byte[] fileContents = GetFileContents(fileName);
        if (fileContents == null)
        {
            LogMessage($"Failed to load JSON content from file {fileName}", true);
            return default;
        }

        // Convert bytes to string
        string jsonString = System.Text.Encoding.UTF8.GetString(fileContents);
      
        try
        {
            // Deserialize JSON to an object of type T
            T jsonData = JsonConvert.DeserializeObject<T>(jsonString);

            return jsonData;
        }
        catch (JsonException ex)
        {
            LogMessage($"Error during JSON deserialization: {ex.Message}", true);
            return default;
        }
    }

    public AssetBundle LoadAssetBundle(string fileName)
    {
        byte[] fileContents = GetFileContents(fileName);
        if (fileContents == null)
        {
            LogMessage($"Failed to load AssetBundle: file content {fileName} not found", true);
            return null;
        }

        // Load AssetBundle from byte array
        AssetBundle assetBundle = AssetBundle.LoadFromMemory(fileContents);

        if (assetBundle == null)
        {
            LogMessage($"Error loading AssetBundle from file {fileName}", true);
        }

        return assetBundle;
    }

    public void ForceUpdate()
    {
        LogMessage("Starting forced update...");
        
        // Удаляем локальные файлы и метаданные
        if (Directory.Exists(localDirectory))
        {
            Directory.Delete(localDirectory, true);
            LogMessage("Local files and metadata deleted.");
        }

        // Создаем заново локальную директорию
        Directory.CreateDirectory(localDirectory);

        StartLoading();
    }
}
