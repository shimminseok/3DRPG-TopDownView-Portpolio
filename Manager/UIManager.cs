using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;


    List<UIPanel> openedPopup = new List<UIPanel>();

    

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
    void Update()
    {

    }
    public void CheckOpenPopup(UIPanel _panel)
    {
        if(openedPopup.Contains(_panel))
        {
            _panel.OnClickCloseButton();
        }
        else
        {
            _panel.OnClickOpenButton();
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
        for(int i = openedPopup.Count - 1; i >= 0; i--)
        {
            openedPopup[i].OnClickCloseButton();
        }
    }
    public void ShowPopup(string _msg)
    {

    }

    public void PlaySkeletonAnimation(SkeletonGraphic _effect, string _aniName, bool _loop = false)
    {
        _effect.gameObject.SetActive(true);
        _effect.AnimationState.ClearTracks();
        _effect.AnimationState.SetAnimation(0,_aniName, _loop);
    }
}
