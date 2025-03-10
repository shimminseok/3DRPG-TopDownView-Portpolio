using Spine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIOption : UIPanel
{
    public static UIOption Instance { get; private set; }


    [SerializeField] Slider masterVolumeController;
    [SerializeField] Slider bgmVolumeController;
    [SerializeField] Slider sfxVolumeController;


    float prevMasterVol = 1;
    float prevBgmVol = 1;
    float prevSfxVol= 1;
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public void SetMasterVolume()
    {
        AudioManager.Instance.SetMasterVolume(masterVolumeController.value);
    }

    public void SetBGMVolume()
    {
        AudioManager.Instance.SetBGMVolume(bgmVolumeController.value);

    }

    public void SetSFXVolume()
    {
        AudioManager.Instance.SetSFXVolume(sfxVolumeController.value);
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }
    public void OnClickCloseBtn()
    {
        if (prevMasterVol != masterVolumeController.value || prevBgmVol != bgmVolumeController.value || prevSfxVol != sfxVolumeController.value)
        {
            YesOrNoPopup.Instance.SetMessage("���� �Ͻðڽ��ϱ�?");
            YesOrNoPopup.Instance.SetYesButton(SaveOptionValue, "����");
            YesOrNoPopup.Instance.SetNoButton(CancleOptionValue, "���");
            YesOrNoPopup.Instance.Open();
        }
    }
    void SaveOptionValue()
    {
        prevMasterVol = masterVolumeController.value;
        prevBgmVol = bgmVolumeController.value;
        prevSfxVol = sfxVolumeController.value;
        Close();
    }

    void CancleOptionValue()
    {
        masterVolumeController.value = prevMasterVol;
        bgmVolumeController.value = prevBgmVol;
        sfxVolumeController.value = prevSfxVol;
        Close();
    }
}
