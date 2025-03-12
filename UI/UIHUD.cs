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
    Dictionary<Buff, GameObject> activeBuffSlot = new Dictionary<Buff, GameObject>();

    [SerializeField] CanvasGroup levelUpCanvasGroup;
    [SerializeField] Image levelUpImg;
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] Transform hudQuestSlotRoot;
    [SerializeField] List<HUDQuestSlot> activeHUDQuestSlots = new List<HUDQuestSlot>();
    [SerializeField] HUDQuestSlot hudQuestSlot;

    [Header("Top")]
    [SerializeField] TextMeshProUGUI areaName;

    [Header("AlertMessage")]
    [SerializeField] CanvasGroup alertMessageCanvas;
    [SerializeField] TextMeshProUGUI alertMessage;

    [Header("DropItem")]
    [SerializeField] List<HUDDropItemSlot> hudDropItems = new List<HUDDropItemSlot>();
    Queue<SaveItemData> dropQueue = new Queue<SaveItemData>();
    bool isProcessing = false;
    int curSlotIndex = 0;

    [Header("PlayerDeath")]
    [SerializeField] GameObject PlayerDeathBG;
    public List<HUDItemSlot> HUDItemSlot => hudItemSlots;


    public Action<string> OnAletMessage;
    public Action<SaveItemData> OnGetItemDisplayed;
    public Action<int> OnGetGoldDisplayed;
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

        OnAletMessage += ShowAlertMessage;
        OnGetItemDisplayed += ShowGetItem;
        OnGetGoldDisplayed += ShowGetGold;

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

        PlayerController.Instance.OnPlayerDeath += PlayerDeath;
        PlayerController.Instance.OnPlayerRevive += PlayerRevive;

        int index = 0;
        foreach (var slot in GameManager.Instance.LoadGameData().ResisteredSkills)
        {
            PlayerController.Instance.SkillManager.ResisteredSkill[slot.Key] = slot.Value;
            hudSkillSlots[index++].AssingedSkill(slot.Value);
        }

        index = 0;
        foreach (var slot in GameManager.Instance.LoadGameData().ResisteredItems)
        {
            hudItemSlots[index].slotHotKey = slot.Key;
            hudItemSlots[index++].SetItemSlot(slot.Value);
        }
    }

    private void Instance_OnPlayerRevive()
    {
        throw new NotImplementedException();
    }

    public void UpdateHPUI(int _current, int _max)
    {
        hpText.text = $"{_current} / {_max}";
        hpBar.DOKill();
        hpBar.DOFillAmount((float)_current / _max, 0.3f);
    }
    public void UpdateMPUI(int _current, int _max)
    {
        mpText.text = $"{_current} / {_max}";
        mpBar.DOKill();
        mpBar.DOFillAmount((float)_current / _max, 0.3f);
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
        areaName.DOFade(1, 3f).SetEase(Ease.OutExpo).From(0).OnComplete(() => areaName.DOFade(0, 1.5f).SetEase(Ease.OutExpo));
        areaName.text = _areaName;

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
    void PlayerDeath()
    {
        PlayerDeathBG.SetActive(true);
    }
    void PlayerRevive()
    {
        PlayerDeathBG.SetActive(false);
    }
    void ShowAlertMessage(string _msg)
    {
        alertMessage.text = _msg;

        alertMessageCanvas.DOKill();
        alertMessageCanvas.alpha = 0;
        alertMessageCanvas.gameObject.SetActive(true);

        alertMessageCanvas.DOFade(1, 0.3f)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    alertMessageCanvas.DOFade(0, 0.3f).OnComplete(() => alertMessageCanvas.gameObject.SetActive(false));
                });
            });
    }
    public void ShowGetItem(SaveItemData _item)
    {
        dropQueue.Enqueue(_item);
        //hudDropItems[curSlotIndex].SetItemInfo(_item);
        //curSlotIndex = (curSlotIndex + 1) % hudItemSlots.Count;
        if (!isProcessing)
            StartCoroutine(ProcessDropQueue());
    }
    IEnumerator ProcessDropQueue()
    {
        isProcessing = true;
        while (dropQueue.Count > 0)
        {
            //if (hudDropItems[curSlotIndex].IsShow)
            //{
            //    yield return new WaitUntil(() => !hudDropItems[curSlotIndex].IsShow);
            //    continue;
            //}

            SaveItemData itemData = dropQueue.Dequeue();

            hudDropItems[curSlotIndex].SetItemInfo(itemData);
            curSlotIndex = (curSlotIndex + 1) % hudItemSlots.Count;

            yield return new WaitForSeconds(0.1f);
        }

        isProcessing = false;
    }
    public void ShowGetGold(int _amount)
    {

        hudDropItems[curSlotIndex].SetGoldInfo(_amount);
        curSlotIndex = (curSlotIndex + 1) % hudItemSlots.Count;
    }

    private void OnDestroy()
    {
        OnAletMessage -= ShowAlertMessage;
        OnGetItemDisplayed -= ShowGetItem;
        OnGetGoldDisplayed -= ShowGetGold;
    }
}
