using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    public static UIHUD Instance;

    [Header("Bottom")]
    [SerializeField] Image hpBar;
    [SerializeField] Image mpBar;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI mpText;
    [SerializeField] GameObject castingBar;
    [SerializeField] Image castingBarFillImg;
    [SerializeField] TextMeshProUGUI castingTimeTxt;
    [SerializeField] Slider expBar;

    [SerializeField] List<HUDSkillSlot> hudSkillSlots = new List<HUDSkillSlot>();
    [SerializeField] List<HUDItemSlot> hudItemSlots = new List<HUDItemSlot>();

    [SerializeField] Transform buffEffectIconRoot;
    [SerializeField] HUDBuffIconSlot buffSlotIconPrefabs;


    [Header("Center")]
    [SerializeField] GameObject interactBtn;
    [SerializeField] GameObject interactShadowObj;
    Coroutine blinkCorotine;
    public event Action<bool> OnViewInteractUI;
    Dictionary<Buff, GameObject> activeBuffSlot = new Dictionary<Buff, GameObject>();

    [SerializeField] CanvasGroup levelUpCanvasGroup;
    [SerializeField] Image levelUpImg;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] Transform hudQuestSlotRoot;
    [SerializeField] List<HUDQuestSlot> activeHUDQuestSlots = new List<HUDQuestSlot>();
    [SerializeField] HUDQuestSlot hudQuestSlot;

    [Header("Top")]
    [SerializeField] TextMeshProUGUI areaName;

    public List<HUDItemSlot> HUDItemSlot => hudItemSlots;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        PlayerController.Instance.SkillManager.OnSkillUsed += HandleSkillUsed;
        PlayerController.Instance.OnHealthChanged += UpdateHPUI;
        PlayerController.Instance.OnMPChanged += UpdateMPUI;

        PlayerController.Instance.BuffManager.OnBuffAdded += AddBuffIcon;
        PlayerController.Instance.BuffManager.OnBuffRemoved += RemoveBuffIcon;
        PlayerController.Instance.SkillManager.OnSkillCasting += UpdateSkillCastingTime;

        PlayerController.Instance.characterStat.OnGainExp += UpdateExpUI;
        PlayerController.Instance.characterStat.OnLevelUp += UpdateLevelUI;

        int index = 0;
        foreach (var slot in GameManager.Instance.LoadGameData().ResisteredSkills)
        {
            PlayerController.Instance.SkillManager.ResisteredSkill[slot.Key] = slot.Value;
            hudSkillSlots[index++].AssingedSkill(slot.Value);
        }

        index = 0;
        foreach(var slot in GameManager.Instance.LoadGameData().ResisteredItems)
        {
            hudItemSlots[index].slotHotKey = slot.Key;
            hudItemSlots[index++].SetItemSlot(slot.Value);
        }
    }
    public void UpdateHPUI(int _current, int _max)
    {
        hpText.text = $"{_current} / {_max}";
        hpBar.DOFillAmount((float)_current / _max, 0.3f);
    }
    public void UpdateMPUI(int _current, int _max)
    {
        mpText.text = $"{_current} / {_max}";
        mpBar.fillAmount = (float)_current / _max;
    }
    public void UpdateExpUI(float _current, int _max)
    {
        expBar.DOValue(_current / _max, 0.1f);
    }
    public void UpdateLevelUI(int _level)
    {

        Sequence sq = DOTween.Sequence();
        sq.Append(levelUpCanvasGroup.DOFade(1, 1f).From(0.7f))
            .Join(levelUpImg.rectTransform.DOLocalMoveY(200f, 1f).From(0f)).SetEase(Ease.InOutQuart)
            .Insert(2f, levelUpCanvasGroup.DOFade(0, 1f));

        levelText.text = _level.ToString();
    }
    public void UpdateAreaName(string _areaName)
    {

    }
    public void HandleSkillUsed(KeyCode _hotkey, float _coolTime)
    {
        hudSkillSlots.Find(x => x.slotHotKey == _hotkey).StartCoolTime(_coolTime);
    }
    void AddBuffIcon(Buff _buff)
    {
        HUDBuffIconSlot slot = Instantiate(buffSlotIconPrefabs, buffEffectIconRoot);
        slot.SetBuffIcon(_buff);
        activeBuffSlot[_buff] = slot.gameObject;
    }
    void RemoveBuffIcon(Buff _buff)
    {
        if (activeBuffSlot.TryGetValue(_buff, out var slot))
        {
            Destroy(slot);
            activeBuffSlot.Remove(_buff);
        }
    }

    public HUDItemSlot GetHUDItemSlot(int _index)
    {
        return hudItemSlots[_index];
    }
    public HUDSkillSlot GetHUDSkillSlot(int _index)
    {
        return hudSkillSlots[_index];
    }
    public HUDItemSlot GetHUDItemSlotByRegisterSlot(int _index)
    {
        return hudItemSlots.Find(x => x != null && x.registedInventoryIndex == _index);
    }
    public void UpdateSkillCastingTime(float _castingtime)
    {
        if (_castingtime <= 0)
            return;

        StartCoroutine(StartCastingTime(_castingtime));
    }
    IEnumerator StartCastingTime(float _castingTime)
    {
        float time = 0;
        castingBar.SetActive(true);
        while (time <= _castingTime)
        {
            yield return null;
            time += Time.deltaTime;
            float t = time / _castingTime;
            castingBarFillImg.fillAmount = Mathf.Lerp(0, 1, t);
            castingTimeTxt.text = time.ToString("F1");
        }
        castingBar.SetActive(false);
    }
    public void OnInteractHUDUI()
    {
        interactBtn.SetActive(true);
        if (gameObject.activeSelf)
            blinkCorotine = StartCoroutine(StartInteract());
    }
    public void OffInteractHUDUI()
    {
        StopStartInteract();
    }
    public IEnumerator StartInteract()
    {
        while (true)
        {
            interactShadowObj.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            interactShadowObj.SetActive(false);
            yield return new WaitForSeconds(0.3f);

        }
    }
    public void StopStartInteract()
    {
        StopCoroutine(blinkCorotine);
        interactBtn.SetActive(false);
    }
    public void ShowHUDQusetSlot(SaveQuestData _quest)
    {
        if (_quest != null && !activeHUDQuestSlots.Exists(x => x.QuestData.QuestID == _quest.QuestID))
        {
            HUDQuestSlot hudquestSlot = Instantiate(hudQuestSlot, hudQuestSlotRoot);
            hudquestSlot.ActiveHUDQuestSlot(_quest);
            activeHUDQuestSlots.Add(hudquestSlot);
            QuestManager.Instance.OnQuestCompleted += HideHUDQuestSlot;
            QuestManager.Instance.OnQuestCountChanged += UpdateHUDQuestSlot;
        }
    }
    public void HideHUDQuestSlot(SaveQuestData _quest)
    {
        HUDQuestSlot findSlot = activeHUDQuestSlots.Find(x => x.QuestData.QuestID == _quest.QuestID);
        if (findSlot != null)
        {
            activeHUDQuestSlots.Remove(findSlot);
            Destroy(findSlot.gameObject);
            QuestManager.Instance.OnQuestCompleted -= HideHUDQuestSlot;
            QuestManager.Instance.OnQuestCountChanged -= UpdateHUDQuestSlot;
        }
    }
    void UpdateHUDQuestSlot(SaveQuestData _quest)
    {
        HUDQuestSlot findSlot = activeHUDQuestSlots.Find(x => x.QuestData.QuestID == _quest.QuestID);
        if (findSlot != null)
        {
            findSlot.UpdateHUDQuestSlot(_quest);
        }
    }
}
