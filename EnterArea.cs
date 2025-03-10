using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnterArea : MonoBehaviour
{
    [SerializeField] BGM areaBGM;
    [SerializeField] string areaName;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && AudioManager.Instance?.GetCurrentPlayingBGMClip() != areaBGM)
        {
            AudioManager.Instance?.PlayBGM(areaBGM);
            UIHUD.Instance.UpdateAreaName(areaName);


        }
    }
}
