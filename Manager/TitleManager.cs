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
            YesOrNoPopup.Instance.SetMessage($"이미 저장된 데이터가 있습니다. 새로운 게임 시작시 저장된 데이터는 삭제됩니다.\n 정말 새로 시작하시겠습니까?");
            YesOrNoPopup.Instance.SetYesButton(CheckSelectedJobPopUp, "새로 하기");
            YesOrNoPopup.Instance.SetNoButton(null, "취소");
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
        YesOrNoPopup.Instance.SetMessage($"당신이 선택한 직업은 \"{jobData.JobName}\"입니다.\n 정말로 선택하시겠습니까?\n(직업은 선택하면 변경하실 수 없습니다.)");
        YesOrNoPopup.Instance.SetYesButton(StartGame, "선택");
        YesOrNoPopup.Instance.SetNoButton(CancleSelectedJob, "취소");
        YesOrNoPopup.Instance.Open();
    }

    public void PlaySelectedAnimation(Animator _animator)
    {
        _animator.SetTrigger("IsSelected");
    }
}
