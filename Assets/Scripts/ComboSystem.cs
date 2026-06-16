using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 향상된 콤보 시스템: 연속 명중 시 점수 배수 증가
/// 콤보 유지 시간이 더 길어져서 쉽게 끊기지 않음
/// 예: 5번 연속 명중 시 1.25배, 10번 명중 시 1.5배, 20번 명중 시 2배
/// </summary>
public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }

    [Header("콤보 설정")]
    [SerializeField] private float comboResetTime = 5f;        // 콤보 리셋 시간 증가 (5초)
    [SerializeField] private TextMeshProUGUI comboText;        // 콤보 UI 텍스트

    [Header("배수 설정")]
    [SerializeField] private int combo5Threshold = 5;          // 5연타 시 배수 증가
    [SerializeField] private float combo5Multiplier = 1.25f;   // 1.25배
    [SerializeField] private int combo10Threshold = 10;        // 10연타 시 배수 증가
    [SerializeField] private float combo10Multiplier = 1.5f;   // 1.5배
    [SerializeField] private int combo20Threshold = 20;        // 20연타 시 배수 증가
    [SerializeField] private float combo20Multiplier = 2f;     // 2배

    private int currentCombo = 0;
    private Coroutine resetCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateComboUI();
    }

    /// <summary>
    /// 성공적인 명중 시 콤보 증가 (리셋 타이머 재설정)
    /// </summary>
    public void AddCombo()
    {
        currentCombo++;

        // 콤보 리셋 타이머 리셋 (계속 명중하면 콤보 유지)
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(ComboResetTimer());

        UpdateComboUI();

        // 특정 콤보 달성 시 피드백
        CameraShake cameraShake = FindObjectOfType<CameraShake>();

        if (currentCombo == combo5Threshold)
        {
            Debug.Log($"<color=yellow>🔥 {combo5Threshold}연타! 결승까지 지속 쉐이크 시작!</color>");
            // 5연타 달성 시 결승선까지 지속되는 쉐이크를 시작
            if (cameraShake != null) cameraShake.StartContinuousShake();

            // 연속 쉐이크 강도는 현재 콤보로 업데이트
            if (cameraShake != null) cameraShake.UpdateContinuousMultiplier(currentCombo);

            if (SoundEffectManager.Instance != null)
                SoundEffectManager.Instance.PlayComboSound(combo5Threshold);
        }
        else if (currentCombo == combo10Threshold)
        {
            Debug.Log($"<color=orange>🔥🔥 {combo10Threshold}연타! {combo10Multiplier}배 보너스!</color>");
            if (cameraShake != null) cameraShake.ShakeMedium();
            
            if (SoundEffectManager.Instance != null)
                SoundEffectManager.Instance.PlayComboSound(combo10Threshold);
        }
        else if (currentCombo == combo20Threshold)
        {
            Debug.Log($"<color=red>🔥🔥🔥 {combo20Threshold}연타!!! {combo20Multiplier}배 최대 보너스!!!</color>");
            if (cameraShake != null) cameraShake.ShakeStrong();
            
            if (SoundEffectManager.Instance != null)
                SoundEffectManager.Instance.PlayComboSound(combo20Threshold);
        }
        else if (currentCombo > combo20Threshold && currentCombo % 5 == 0)
        {
            // 20연타 이상에서는 5연타마다 연속 쉐이크
            if (cameraShake != null)
            {
                cameraShake.UpdateContinuousMultiplier(currentCombo);
                cameraShake.ShakeContinuous();
            }
        }
        
        // 콤보가 5 이상이면 매 추가 콤보에서 연속 쉐이크 강도 업데이트
        if (currentCombo > combo5Threshold && cameraShake != null)
        {
            cameraShake.UpdateContinuousMultiplier(currentCombo);
        }
    }

    /// <summary>
    /// 현재 점수 배수 반환
    /// </summary>
    public float GetComboMultiplier()
    {
        if (currentCombo >= combo20Threshold)
            return combo20Multiplier;
        else if (currentCombo >= combo10Threshold)
            return combo10Multiplier;
        else if (currentCombo >= combo5Threshold)
            return combo5Multiplier;
        else
            return 1f;
    }

    /// <summary>
    /// 콤보 리셋 (실패, 맞춘 후 시간 초과 등)
    /// </summary>
    public void ResetCombo()
    {
        if (currentCombo > 0)
        {
            Debug.Log($"<color=red>콤보 끊김! {currentCombo}연타 달성했습니다.</color>");
        }
        currentCombo = 0;
        UpdateComboUI();
    }

    /// <summary>
    /// 콤보 UI 업데이트
    /// </summary>
    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            if (currentCombo > 0)
            {
                float multiplier = GetComboMultiplier();
                string multiplierText = multiplier > 1f ? $" ({multiplier}x)" : "";
                
                // 콤보가 높을수록 크기 커짐
                float scale = 1f + (currentCombo / 20f) * 0.5f;
                comboText.fontSize = 50 * scale;
                
                comboText.text = $"<color=yellow>COMBO: {currentCombo}{multiplierText}</color>";
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 콤보 리셋 타이머 (더 길어져서 콤보 유지 쉬움)
    /// </summary>
    private IEnumerator ComboResetTimer()
    {
        yield return new WaitForSeconds(comboResetTime);
        ResetCombo();
    }

    /// <summary>
    /// 현재 콤보 값 반환
    /// </summary>
    public int GetCurrentCombo()
    {
        return currentCombo;
    }

    /// <summary>
    /// 외부에서 5연타 임계값을 얻기 위한 접근자
    /// </summary>
    public int GetStartContinuousThreshold()
    {
        return combo5Threshold;
    }

    /// <summary>
    /// 게임 리셋 시 콤보 초기화
    /// </summary>
    public void ResetForNewGame()
    {
        currentCombo = 0;
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        UpdateComboUI();
    }
}
