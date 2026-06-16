using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 점수 기록 및 랭킹 시스템
/// 게임 플레이 중 점수 이력 표시 및 저장
/// </summary>
public class ScoreHistory : MonoBehaviour
{
    public static ScoreHistory Instance { get; private set; }

    [Header("점수 기록 UI")]
    [SerializeField] private TextMeshProUGUI scoreHistoryText;
    [SerializeField] private int maxHistoryCount = 5;  // 표시할 최대 기록 개수

    private List<int> scoreHistory = new List<int>();
    private int highScore = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // PlayerPrefs에서 최고 점수 로드
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        UpdateHistoryUI();
    }

    /// <summary>
    /// 게임 종료 시 점수 기록 저장
    /// </summary>
    public void SaveScore(int score)
    {
        scoreHistory.Add(score);

        // 최고 점수 업데이트
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            Debug.Log($"<color=gold>🏆 새 최고 기록! {highScore}점</color>");
        }

        UpdateHistoryUI();
    }

    /// <summary>
    /// 점수 기록 UI 업데이트
    /// </summary>
    private void UpdateHistoryUI()
    {
        if (scoreHistoryText == null)
            return;

        string historyStr = $"<b>최고 기록: {highScore}</b>\n\n";
        historyStr += "<size=80%>";

        // 최근 5개의 기록 표시 (역순)
        int startIdx = Mathf.Max(0, scoreHistory.Count - maxHistoryCount);
        for (int i = scoreHistory.Count - 1; i >= startIdx; i--)
        {
            historyStr += $"• {scoreHistory[i]}점\n";
        }

        historyStr += "</size>";
        scoreHistoryText.text = historyStr;
    }

    /// <summary>
    /// 최고 점수 반환
    /// </summary>
    public int GetHighScore()
    {
        return highScore;
    }

    /// <summary>
    /// 현재 기록 리스트 반환
    /// </summary>
    public List<int> GetScoreHistory()
    {
        return new List<int>(scoreHistory);
    }

    /// <summary>
    /// 최고 점수 리셋
    /// </summary>
    public void ResetHighScore()
    {
        highScore = 0;
        PlayerPrefs.SetInt("HighScore", 0);
        PlayerPrefs.Save();
        UpdateHistoryUI();
    }
}
