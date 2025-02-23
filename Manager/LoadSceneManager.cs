using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager Instance { get; private set; }

    [SerializeField] string loadingSceneName = "";

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

    public void LoadScene(int _type)
    {
        StartCoroutine(LoadSceneAsync((SceneType)_type));
    }

    IEnumerator LoadSceneAsync(SceneType _type)
    {
        //AsyncOperation op = SceneManager.LoadSceneAsync((int)_type);

        //while(!op.isDone)
        //{
        //    yield return null;
        //}

        AsyncOperation _sceneOp = SceneManager.LoadSceneAsync((int)_type);
        _sceneOp.allowSceneActivation = false;


        while (_sceneOp.progress < 0.9f)
        {
            yield return null;
        }

        _sceneOp.allowSceneActivation = true;
        while(!_sceneOp.isDone)
        {
            yield return null;
        }
    }
}
