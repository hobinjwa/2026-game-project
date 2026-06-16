using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 게임의 점수 시스템을 관리하는 매니저 클래스
/// 단일 인스턴스(Singleton)로 관리되어 어디서나 접근 가능
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ScoreManager Instance { get; private set; }

    [Header("점수 UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("점수 이펙트")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Canvas worldCanvas; // 월드 스페이스 캔버스 (점수 이펙트 표시용)

    [Header("점수 설정")]
    [SerializeField] private int centerScore = 3;      // 중앙 구역 점수
    [SerializeField] private int middleScore = 2;      // 중간 구역 점수
    [SerializeField] private int edgeScore = 1;        // 외곽 구역 점수

    private int totalScore = 0;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    /// <summary>
    /// 점수 추가 및 UI 업데이트 (콤보 배수 적용)
    /// </summary>
    /// <param name="points">추가할 점수</param>
    /// <param name="hitPosition">맞은 위치 (이펙트 표시용)</param>
    public void AddScore(int points, Vector3 hitPosition)
    {
        // 콤보 시스템에서 배수 가져오기
        float comboMultiplier = 1f;
        int finalPoints = points;

        if (ComboSystem.Instance != null)
        {
            comboMultiplier = ComboSystem.Instance.GetComboMultiplier();
            finalPoints = Mathf.RoundToInt(points * comboMultiplier);
            ComboSystem.Instance.AddCombo();

            // 콤보 달성 시 효과음
            int currentCombo = ComboSystem.Instance.GetCurrentCombo();
            if (currentCombo == 5 || currentCombo == 10 || currentCombo == 20)
            {
                if (SoundEffectManager.Instance != null)
                {
                    SoundEffectManager.Instance.PlayComboSound(currentCombo);
                }
            }
        }

        totalScore += finalPoints;
        UpdateScoreUI();
        
        // 점수 획득 이펙트 표시 (배수 적용된 점수 표시)
        ShowFloatingText(finalPoints, hitPosition, comboMultiplier > 1f);

        // 사운드 효과음 재생
        if (SoundEffectManager.Instance != null)
        {
            SoundEffectManager.Instance.PlayHitSound(points);
        }
    }

    /// <summary>
    /// 정확도에 따라 자동으로 점수 계산 및 추가
    /// 정확도 0~1 (0 = 외곽, 1 = 중앙)
    /// </summary>
    /// <param name="accuracy">정확도 (0~1)</param>
    /// <param name="hitPosition">맞은 위치</param>
    public void AddScoreByAccuracy(float accuracy, Vector3 hitPosition)
    {
        int points = CalculateScoreByAccuracy(accuracy);
        AddScore(points, hitPosition);
    }

    /// <summary>
    /// 정확도를 바탕으로 점수 계산
    /// </summary>
    private int CalculateScoreByAccuracy(float accuracy)
    {
        // 정확도를 0~1 사이로 정규화
        accuracy = Mathf.Clamp01(accuracy);

        if (accuracy >= 0.6f)
        {
            return centerScore;  // 60% 이상 = 중앙 (3점)
        }
        else if (accuracy >= 0.3f)
        {
            return middleScore;  // 30~60% = 중간 (2점)
        }
        else
        {
            return edgeScore;    // 30% 미만 = 외곽 (1점)
        }
    }

    /// <summary>
    /// 점수 UI 업데이트
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }
    }

    /// <summary>
    /// 점수 이펙트 표시 (+3, +2, +1 텍스트)
    /// </summary>
    private void ShowFloatingText(int points, Vector3 hitPosition, bool isCombo = false)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("ScoreManager: FloatingText 프리팹이 할당되지 않았습니다.");
            return;
        }

        // 월드 스페이스 캔버스가 없으면 씬의 Canvas 찾기
        if (worldCanvas == null)
        {
            worldCanvas = FindObjectOfType<Canvas>();
        }

        if (worldCanvas != null)
        {
            GameObject floatingTextObj = Instantiate(floatingTextPrefab, worldCanvas.transform);
            FloatingTextEffect floatingText = floatingTextObj.GetComponent<FloatingTextEffect>();
            
            if (floatingText != null)
            {
                floatingText.Initialize(points, hitPosition, isCombo);
            }
        }
    }

    /// <summary>
    /// 현재 총 점수 반환
    /// </summary>
    public int GetTotalScore()
    {
        return totalScore;
    }

    /// <summary>
    /// 점수 초기화 (새 게임 시작 시)
    /// </summary>
    public void ResetScore()
    {
        totalScore = 0;
        UpdateScoreUI();
    }
}
