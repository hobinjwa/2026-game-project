using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드 이펙트 매니저
/// 점수별, 콤보별 사운드 재생
/// </summary>
public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance { get; private set; }

    [Header("점수 효과음")]
    [SerializeField] private AudioClip centerHitSound;   // 중앙 명중음
    [SerializeField] private AudioClip middleHitSound;   // 중간 명중음
    [SerializeField] private AudioClip edgeHitSound;     // 외곽 명중음

    [Header("콤보 효과음")]
    [SerializeField] private AudioClip combo5Sound;      // 5연타 효과음
    [SerializeField] private AudioClip combo10Sound;     // 10연타 효과음
    [SerializeField] private AudioClip combo20Sound;     // 20연타 효과음

    [Header("볼륨 설정")]
    [SerializeField] private float masterVolume = 0.5f;
    [SerializeField] private float hitVolume = 0.7f;
    [SerializeField] private float comboVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // AudioSource 생성
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// 점수별 사운드 재생
    /// </summary>
    public void PlayHitSound(int points)
    {
        AudioClip clip = points switch
        {
            3 => centerHitSound,
            2 => middleHitSound,
            1 => edgeHitSound,
            _ => null
        };

        if (clip != null)
        {
            PlaySound(clip, hitVolume);
        }
    }

    /// <summary>
    /// 콤보 달성 사운드 재생
    /// </summary>
    public void PlayComboSound(int comboCount)
    {
        AudioClip clip = comboCount switch
        {
            5 => combo5Sound,
            10 => combo10Sound,
            20 => combo20Sound,
            _ => null
        };

        if (clip != null)
        {
            PlaySound(clip, comboVolume);
        }
    }

    /// <summary>
    /// 실제 사운드 재생
    /// </summary>
    private void PlaySound(AudioClip clip, float volume)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip, volume * masterVolume);
    }

    /// <summary>
    /// 마스터 볼륨 조정
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// 마스터 볼륨 반환
    /// </summary>
    public float GetMasterVolume()
    {
        return masterVolume;
    }
}
