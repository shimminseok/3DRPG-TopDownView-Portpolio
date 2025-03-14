using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance { get; private set; }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void LoadScene(SceneType _type)
    {
        StartCoroutine(LoadSceneAsync(_type));
    }

    IEnumerator LoadSceneAsync(SceneType _type)
    {
        AsyncOperation _sceneOp = SceneManager.LoadSceneAsync((int)_type);
        _sceneOp.allowSceneActivation = false;


        while (_sceneOp.progress < 0.9f)
        {
            Debug.Log("로딩중....");
            yield return null;
        }

        _sceneOp.allowSceneActivation = true;
        while(!_sceneOp.isDone)
        {
            Debug.Log("씬로드중....");
            yield return null;
        }
    }
}
