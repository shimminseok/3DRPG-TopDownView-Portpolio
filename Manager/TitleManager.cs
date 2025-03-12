using LitJson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject selectJobWindow;
    [SerializeField] GameObject selectJobView;
    int selectedJob = 0;

    [Header("Loading")]
    [SerializeField] GameObject loadingBG;
    [SerializeField] Slider loadingBar;
    [SerializeField] TextMeshProUGUI loadingProcessPercentTxt;

    void Start()
    {
        AudioManager.Instance.PlayBGM(BGM.Lobby);
    }


    public void OnClickNewGameBtn()
    {
        if (SaveLoadManager.LoadData("SaveData") == null)
        {
            SaveLoadManager.ResetData("SaveData");
            CheckSelectedJobPopUp();
        }
        else
        {
            YesOrNoPopup.Instance.SetMessage($"�̹� ����� �����Ͱ� �ֽ��ϴ�. ���ο� ���� ���۽� ����� �����ʹ� �����˴ϴ�.\n ���� ���� �����Ͻðڽ��ϱ�?");
            YesOrNoPopup.Instance.SetYesButton(CheckSelectedJobPopUp, "���� �ϱ�");
            YesOrNoPopup.Instance.SetNoButton(null, "���");
            YesOrNoPopup.Instance.Open();
        }
    }
    public void OnClickLoadGameBtn()
    {
        SaveLoadManager.LoadData("SaveData");
        LoadScene(SceneType.InGame);
    }
    void StartGame()
    {
        SaveLoadManager.GameData.JobID = selectedJob;
        LoadScene(SceneType.InGame);
    }
    void CancleSelectedJob()
    {
        selectedJob = 0;
    }
    public void CheckSelectedJobPopUp()
    {
        SaveLoadManager.ResetData("SaveData");
        selectJobView.SetActive(true);
        selectJobWindow.SetActive(true);
    }

    public void OnSelectedJob(int _jobID)
    {
        selectedJob = _jobID;
        JobData jobData = TableLoader.Instance.GetTable<JobTable>().GetJobDataByID(selectedJob);
        YesOrNoPopup.Instance.SetMessage($"����� ������ ������ \"{jobData.JobName}\"�Դϴ�.\n ������ �����Ͻðڽ��ϱ�?\n(������ �����ϸ� �����Ͻ� �� �����ϴ�.)");
        YesOrNoPopup.Instance.SetYesButton(StartGame, "����");
        YesOrNoPopup.Instance.SetNoButton(CancleSelectedJob, "���");
        YesOrNoPopup.Instance.Open();
    }

    public void PlaySelectedAnimation(Animator _animator)
    {
        _animator.SetTrigger("IsSelected");
    }

    public void SetLoadingProcessText(float _val)
    {
        loadingProcessPercentTxt.text = $"{_val}%";

        loadingBar.value = _val;
    }


    public void LoadScene(SceneType _type)
    {
        StartCoroutine(LoadSceneAsync(_type));
    }

    IEnumerator LoadSceneAsync(SceneType _type)
    {
        AsyncOperation sceneOp = SceneManager.LoadSceneAsync((int)_type);
        sceneOp.allowSceneActivation = false;

        loadingBG.SetActive(true);
        float timer = 0f;
        while (!sceneOp.isDone)
        {
            Debug.Log("���ε���....");
            yield return null;
            if (sceneOp.progress < 0.9f)
            {
                loadingBar.value = sceneOp.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                loadingBar.value = Mathf.Lerp(0.9f, 1f, timer);
                if (loadingBar.value >= 1f)
                {
                    sceneOp.allowSceneActivation = true;
                    yield break;
                }
            }
            loadingProcessPercentTxt.text = ($"{(int)(loadingBar.value * 100)}%");
        }
        loadingBG.SetActive(false);
    }
}
