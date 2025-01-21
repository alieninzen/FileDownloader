using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{

    public async Task RunCoroutine(AsyncOperation operation)
    {
        while (!operation.isDone)
        {
            await Task.Yield();
        }
    }

}
