using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 게임 통계 시스템
/// 플레이 중 다양한 게임 데이터 기록 및 표시
/// </summary>
public class GameStatistics : MonoBehaviour
{
    public static GameStatistics Instance { get; private set; }

    [Header("통계 UI")]
    [SerializeField] private TextMeshProUGUI statsText;

    // 통계 데이터
    private int totalHits = 0;           // 총 명중 수
    private int totalShots = 0;          // 총 발사 수
    private float hitRate = 0f;          // 명중률 (%)
    private int maxCombo = 0;            // 최고 콤보
    private int totalScore = 0;          // 총 점수
    private float avgScore = 0f;         // 평균 점수

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
        ResetStats();
        UpdateStatsUI();
    }

    /// <summary>
    /// 명중 기록
    /// </summary>
    public void RecordHit(int scoreEarned)
    {
        totalHits++;
        totalScore += scoreEarned;
        avgScore = totalScore / (float)totalHits;

        // 최고 콤보 업데이트
        ComboSystem comboSystem = FindObjectOfType<ComboSystem>();
        if (comboSystem != null)
        {
            int currentCombo = comboSystem.GetCurrentCombo();
            if (currentCombo > maxCombo)
            {
                maxCombo = currentCombo;
            }
        }

        UpdateStatsUI();
    }

    /// <summary>
    /// 발사 기록
    /// </summary>
    public void RecordShot()
    {
        totalShots++;
        hitRate = totalShots > 0 ? (totalHits / (float)totalShots) * 100f : 0f;
        UpdateStatsUI();
    }

    /// <summary>
    /// 통계 UI 업데이트
    /// </summary>
    private void UpdateStatsUI()
    {
        if (statsText == null)
            return;

        string statsStr = $"<size=70%>";
        statsStr += $"명중: {totalHits} | 발사: {totalShots} | 명중률: {hitRate:F1}%\n";
        statsStr += $"최고콤보: {maxCombo} | 평균점수: {avgScore:F1}\n";
        statsStr += $"총점수: {totalScore}";
        statsStr += $"</size>";

        statsText.text = statsStr;
    }

    /// <summary>
    /// 통계 초기화
    /// </summary>
    public void ResetStats()
    {
        totalHits = 0;
        totalShots = 0;
        hitRate = 0f;
        maxCombo = 0;
        totalScore = 0;
        avgScore = 0f;
        UpdateStatsUI();
    }

    /// <summary>
    /// 게임 종료 시 최종 통계 반환
    /// </summary>
    public string GetFinalStats()
    {
        return $"명중률: {hitRate:F1}% | 최고콤보: {maxCombo} | 총점수: {totalScore}";
    }

    // Getter 메서드들
    public int GetTotalHits() => totalHits;
    public int GetTotalShots() => totalShots;
    public float GetHitRate() => hitRate;
    public int GetMaxCombo() => maxCombo;
    public int GetTotalScore() => totalScore;
    public float GetAvgScore() => avgScore;
}
