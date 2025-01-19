using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private List<FileData> filesList = new List<FileData>();
    private int totalSize = 0;
    private int downloadedSize = 0;

    // Start is called before the first frame update
    private string apiURL = "https://api.github.com/repos/" + GlobalConstants.ServerRootPath;
    void Start()
    {
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
                foreach (var file in files)
                {
                    filesList.Add(file);
                    totalSize += file.Size;
                }

                Debug.Log($"Loaded {filesList.Count} files, total size: {totalSize} bytes");

                // Запускаем загрузку файлов с прогрессом
              StartCoroutine(DownloadFilesWithProgress());
            }
            else
            {
                Debug.LogError($"Failed to load directory: {request.error}");
            }
        }
    }
    private IEnumerator DownloadFilesWithProgress()
    {
        foreach (var file in filesList)
        {
            yield return StartCoroutine(DownloadFile(file));
        }

        Debug.Log("All files downloaded successfully!");
    }

    private IEnumerator DownloadFile(FileData file)
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
                Debug.Log($"File downloaded: {file.Name}");
                downloadedSize += file.Size;
                ShowProgress(downloadedSize, totalSize);
            }
            else
            {
                Debug.LogError($"Failed to download file {file.Name}: {request.error}");
            }
        }
    }

    private void ShowProgress(int current, int total)
    {
        float progress = (float)current / total;
    }
}
