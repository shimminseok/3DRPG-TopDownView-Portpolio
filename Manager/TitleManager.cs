using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnClickNewGameBtn()
    {
        SaveLoadManager.ResetData("SaveData");
        LoadSceneManager.Instance.LoadScene(SceneType.InGame);
    }
    public void OnClickLoadGameBtn()
    {
        SaveLoadManager.LoadData("SaveData");
        LoadSceneManager.Instance.LoadScene(SceneType.InGame);
    }
}
