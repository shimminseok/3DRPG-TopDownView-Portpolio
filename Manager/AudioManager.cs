using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGM
{
    Lobby,
    StartingVilige,
    Forest
}
public enum SFX
{
    Foot,
    WarriorAttack,
    AssassinAttack,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioMixer audiioMixer;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] List<AudioClip> bgmList = new List<AudioClip>();


    [Header("Effect Sound")]
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] List<AudioClip> sfxList = new List<AudioClip>();
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBGM(BGM _bgm)
    {
        if (bgmAudioSource.clip == bgmList[(int)_bgm])
            return;
        bgmAudioSource.loop = true;
        bgmAudioSource.clip = bgmList[(int)_bgm];
        bgmAudioSource.Play();
    }
    public void PlaySFX(SFX _sfx)
    {
        sfxAudioSource.PlayOneShot(sfxList[(int)_sfx]);
    }
    public void SetMasterVolume(float _vol)
    {
        audiioMixer.SetFloat("Master", Mathf.Log10(_vol) * 20);
    }
    public void SetBGMVolume(float _vol)
    {
        audiioMixer.SetFloat("BGM", Mathf.Log10(_vol) * 20);
    }
    public void SetSFXVolume(float _vol)
    {
        audiioMixer.SetFloat("SFX", Mathf.Log10(_vol) * 20);
    }


    public BGM GetCurrentPlayingBGMClip()
    {
        return (BGM)bgmList.FindIndex(clip => clip == bgmAudioSource.clip);
    }
}
