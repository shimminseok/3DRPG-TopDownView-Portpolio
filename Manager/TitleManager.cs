using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class TitleManager : MonoBehaviour
{
    [SerializeField] GameObject selectJobWindow;
    [SerializeField] GameObject selectJobView;
    int selectedJob = 0;
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
        LoadSceneManager.Instance.LoadScene(SceneType.InGame);
    }
    void StartGame()
    {
        SaveLoadManager.GameData.JobID = selectedJob;
        LoadSceneManager.Instance.LoadScene(SceneType.InGame);
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
}
