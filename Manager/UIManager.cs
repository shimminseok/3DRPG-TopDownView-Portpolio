using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    List<UIPanel> openedPopup = new List<UIPanel>();

    public List<UIPanel> OpenedPopup => openedPopup;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }
    public void CheckOpenPopup(UIPanel _panel)
    {
        if (openedPopup.Contains(_panel))
        {
            _panel.Close();
        }
        else
        {
            _panel.Open();
        }
    }
    public void OpenPanel(UIPanel _panel)
    {
        openedPopup.Add(_panel);
        Debug.Log($"{_panel.name} Open");
    }
    public void ClosePanel(UIPanel _panel)
    {
        openedPopup.Remove(_panel);
        Debug.Log($"{_panel.name} Close");
    }
    public void AllClosePanel()
    {
        for (int i = openedPopup.Count - 1; i >= 0; i--)
        {
            openedPopup[i].Close();
        }
    }
    public void HandleEscapeKey()
    {
        if (openedPopup.Count > 0)
        {
            openedPopup.Last().Close();
        }
        else
        {
            CheckOpenPopup(YesOrNoPopup.Instance);
            YesOrNoPopup.Instance.SetMessage("게임을 종료하시겠습니까?");
            YesOrNoPopup.Instance.SetYesButton(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
            }, "종료하기");

            YesOrNoPopup.Instance.SetNoButton(null);
        }
    }
    public void PlaySkeletonAnimation(SkeletonGraphic _effect, string _aniName, bool _loop = false)
    {
        _effect.gameObject.SetActive(true);
        _effect.AnimationState.ClearTracks();
        _effect.AnimationState.SetAnimation(0, _aniName, _loop);
    }
}
