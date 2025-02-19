using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;

public class SkillManager : MonoBehaviour
{
    SkillTable skillTable;
    BuffTable buffTable;
    CharacterControllerBase casterController;

    Dictionary<int, SaveSkillData> skillCoolTimeDic = new Dictionary<int, SaveSkillData>();
    List<SaveSkillData> availableSkills = new List<SaveSkillData>();
    Dictionary<KeyCode, SaveSkillData> resisteredSkill = new Dictionary<KeyCode, SaveSkillData>();

    Collider[] overlabResults = new Collider[30];
    public event Action<KeyCode, float> OnSkillUsed;
    public event Action<float> OnSkillCasting;


    public List<SaveSkillData> AvailableSkills => availableSkills;

    bool isCastingSkill;

    private void Awake()
    {
        casterController = GetComponent<CharacterControllerBase>();
        skillTable = TableLoader.Instance.GetTable<SkillTable>();
        buffTable = TableLoader.Instance.GetTable<BuffTable>();
    }

    public void RegisterSkill(int _jobID)
    {
        List<SkillData> registerSkills = skillTable.GetSKillsByJobID(_jobID);
        for (int i = 0; i < registerSkills.Count; i++)
        {
            SaveSkillData saveData = new SaveSkillData();
            saveData.SkillID = registerSkills[i].ID;
            availableSkills.Add(saveData);
        }
        Debug.LogWarning("스킬 등록 완료");
    }
    public void ExecuteSkill(SaveSkillData _skillData, GameObject _caster)
    {
        if (isCastingSkill)
            return;

        SkillData skillData = _skillData.GetSkillData();

        StartCoolTime(_skillData.SkillID, _skillData);
        StartCoroutine(CastingSKill(_skillData, _caster));
    }
    void PlaySkillAnimation(SkillData _data, GameObject _caster)
    {
        _caster.GetComponent<CharacterAnimator>()?.PlaySkillAnimation(_data);
    }
    void HandleSkillEffect(SkillData _data, GameObject _caster)
    {
        if (_data.EffectPrefab != null)
        {
            //임시 (오브젝트 풀에서 가져와야함)
            //스킬을 등록하면 해당 스킬에 맞는 오브젝트를 풀에 등록시키고, 스킬 등록을 해제하면 풀에서 삭제 하는 방식 채용
            GameObject effect = ObjectPoolManager.Instance.GetObject(_data.EffectPrefab.name);
            ParticleSystem particle = effect.GetComponent<ParticleSystem>();
            effect.transform.position = _caster.transform.position;
            effect.transform.rotation = _caster.transform.rotation;
            particle.Play(true);
            ObjectPoolManager.Instance.ReturnObject(effect, particle.main.duration);
        }
    }
    IEnumerator CastingSKill(SaveSkillData _skillData, GameObject _caster)
    {
        SkillData skillData = _skillData.GetSkillData();
        isCastingSkill = true;
        OnSkillCasting?.Invoke(skillData.CastingTime);
        yield return new WaitForSeconds(_skillData.GetSkillData().CastingTime);
        OnSkillUsed?.Invoke(_skillData.HotKey, skillData.CoolTime);
        HandleSkillEffect(skillData, _caster);
        PlaySkillAnimation(skillData, _caster);
        foreach (var effect in skillData.SkillEffects)
        {
            ApplyEffect(skillData, effect, _caster);
        }

        isCastingSkill = false;
    }
    void ApplyEffect(SkillData _data, SkillEffect _effect, GameObject _caster)
    {
        List<GameObject> targets = GetTargetsByEffect(_data, _effect, _caster);
        foreach (var target in targets)
        {
            switch (_effect.EffectType)
            {
                case SkillEffectType.Damage:
                    ApplyDamage(_effect, _caster, target);
                    break;
                case SkillEffectType.Buff:
                case SkillEffectType.Debuff:
                    ApplyBuffOrDebuff(_effect, target);
                    break;
            }
        }
    }
    List<GameObject> GetTargetsByEffect(SkillData _skillData, SkillEffect _effect, GameObject _caster)
    {
        switch (_effect.TargetType)
        {
            case SkillTarget.Self:
                return new List<GameObject>() { _caster };
            case SkillTarget.Enemy:
                return new List<GameObject>();
            case SkillTarget.AreaEnemy:
                return GetTargetInRange(_skillData, 1 << LayerMask.NameToLayer("Enemy")).Select(c => c.gameObject).ToList();
            case SkillTarget.AreaParty:
                return new List<GameObject>();
            case SkillTarget.AllyParty:
                return new List<GameObject>();

            default:
                return new List<GameObject>();
        }
    }
    void ApplyDamage(SkillEffect _effect, GameObject _caster, GameObject _target)
    {
        if (_target.TryGetComponent<IDamageable>(out var damageable))
        {
            if(_caster.TryGetComponent<IAttacker>(out var attacker))
            {
                float finalDam = (attacker.FinalDam + _effect.FlatValue) * _effect.PercentValue;
                damageable.TakeDamage(Mathf.RoundToInt(finalDam), attacker);
            }
        }
    }
    void ApplyBuffOrDebuff(SkillEffect _effect, GameObject _target)
    {
        BuffData data = buffTable.GetBuffDataByID(_effect.BuffID);
        if (data == null)
        {
            Debug.LogWarning($"BuffID {_effect.BuffID}가 존재하지 않습니다.");
            return;
        }
        Buff buff = BuffFactory.CreateBuff(data, _effect, _target);

        var statusEffectManager = _target.GetComponent<BuffManager>() ?? _target.AddComponent<BuffManager>();
        statusEffectManager.AddBuff(buff);
    }

    public void AssignSkill(HUDSkillSlot _skillData)
    {
        resisteredSkill[_skillData.slotHotKey] = _skillData.assigendSkill;
        resisteredSkill[_skillData.slotHotKey].HotKey = _skillData.slotHotKey;
        if (_skillData.assigendSkill.GetSkillData().EffectPrefab != null)
            ObjectPoolManager.Instance.CreatePool(_skillData.assigendSkill.GetSkillData().EffectPrefab, 3);
    }
    public void StartCoolTime(int _id, SaveSkillData _data)
    {
        if (!skillCoolTimeDic.ContainsKey(_id))
        {
            skillCoolTimeDic[_id] = _data;
        }
    }
    public void RemoveCoolTime(int _id)
    {
        if (skillCoolTimeDic.ContainsKey(_id))
        {
            skillCoolTimeDic.Remove(_id);
        }
    }
    public bool CanUseSkill(int _id)
    {
        return skillCoolTimeDic.ContainsKey(_id);
    }
    public SaveSkillData GetSKillDataByHotKey(KeyCode _hotkey)
    {
        if (resisteredSkill.TryGetValue(_hotkey, out var skillData))
        {
            return skillData;
        }
        else
        {
            Debug.LogWarning("등록된 스킬이 없습니다.");
            return null;
        }
    }


    public List<Collider> GetTargetInRange(SkillData _data, LayerMask _targetLayer)
    {

        List<Collider> hitColliders = new List<Collider>();
        int hitCnt = 0;
        switch (_data.RangeType)
        {
            case SkillRangeType.Rectangle:
                Vector3 halfExtents = new Vector3(_data.Width / 2, 1, _data.Length / 2);
                Quaternion rotation = Quaternion.LookRotation(transform.forward);
                hitCnt = Physics.OverlapBoxNonAlloc(transform.position + transform.forward.normalized * (_data.Length / 2), halfExtents, overlabResults, rotation, _targetLayer);
                for (int i = 0; i < hitCnt; i++)
                {
                    hitColliders.Add(overlabResults[i]);
                }
                break;
            case SkillRangeType.Sector:
                hitCnt = Physics.OverlapSphereNonAlloc(transform.position, _data.Radius, overlabResults, _targetLayer);
                for (int i = 0; i < hitCnt; i++)
                {
                    Vector3 dirToTarget = (overlabResults[i].transform.position - transform.position).normalized;
                    if (Vector3.Angle(transform.position, dirToTarget) <= _data.Angle / 2)
                    {
                        hitColliders.Add(overlabResults[i]);
                    }
                }
                break;
            case SkillRangeType.Circle:
                hitCnt = Physics.OverlapSphereNonAlloc(transform.position, _data.Radius, overlabResults, _targetLayer);
                for (int i = 0; i < hitCnt; i++)
                {
                    hitColliders.Add(overlabResults[i]);
                }
                break;
            case SkillRangeType.Point:
                break;
        }
        return hitColliders;
    }
}
