using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// 점수 획득 시 나타나는 플로팅 텍스트 이펙트 (+3, +2, +1)
/// 프리팹으로 생성되며, 자동으로 사라짐
/// </summary>
public class FloatingTextEffect : MonoBehaviour
{
    [Header("텍스트 설정")]
    private TextMeshProUGUI scoreText;

    [Header("이펙트 설정")]
    [SerializeField] private float displayDuration = 1.5f;  // 표시 지속 시간
    [SerializeField] private float floatDistance = 100f;    // 떠오르는 거리 (픽셀)

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // RectTransform 안전하게 처리
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("FloatingTextEffect: RectTransform이 없습니다. UI 오브젝트인지 확인하세요.");
            Destroy(gameObject);
            return;
        }

        // CanvasGroup 처리
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // TextMeshPro 처리
        scoreText = GetComponent<TextMeshProUGUI>();
        if (scoreText == null)
        {
            Debug.LogError("FloatingTextEffect: TextMeshProUGUI가 없습니다.");
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 플로팅 텍스트 초기화 및 시작
    /// </summary>
    public void Initialize(int points, Vector3 worldPosition, bool isCombo = false)
    {
        // 텍스트 설정
        if (isCombo)
        {
            scoreText.text = $"<b>+{points}</b><size=70%>\n(COMBO!)</size>";
        }
        else
        {
            scoreText.text = $"+{points}";
        }
        scoreText.fontSize = 50;
        scoreText.alignment = TextAlignmentOptions.Center;
        
        // 점수별 색상 설정
        Color textColor = GetColorByPoints(points, isCombo);
        scoreText.color = textColor;

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
        rectTransform.position = new Vector3(screenPosition.x, screenPosition.y, 0);

        // 애니메이션 실행
        PlayFloatingAnimation(isCombo);
    }

    /// <summary>
    /// 점수별 색상 반환 (콤보 적용 여부)
    /// </summary>
    private Color GetColorByPoints(int points, bool isCombo = false)
    {
        if (isCombo)
        {
            return new Color(1f, 0.2f, 1f); // 콤보 보너스: 핑크/마젠타
        }

        return points switch
        {
            3 => Color.yellow,   // 중앙: 노란색
            2 => new Color(1f, 0.65f, 0f), // 중간: 주황색
            1 => Color.red,      // 외곽: 빨간색
            _ => Color.white     // 기본: 흰색
        };
    }

    /// <summary>
    /// 플로팅 텍스트 애니메이션 재생
    /// 위로 떠오르면서 페이드 아웃
    /// </summary>
    private void PlayFloatingAnimation(bool isCombo = false)
    {
        // 초기 알파값 설정
        canvasGroup.alpha = 1f;

        float duration = isCombo ? displayDuration * 1.2f : displayDuration;
        float distance = isCombo ? floatDistance * 1.5f : floatDistance;

        // 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 콤보 이펙트: 점프 효과
        if (isCombo)
        {
            rectTransform.localScale = Vector3.one;
            sequence.Append(rectTransform.DOScale(1.3f, 0.2f).SetEase(Ease.OutBack));
            sequence.Append(rectTransform.DOScale(1f, 0.1f).SetEase(Ease.InQuad));
        }

        // 위로 떠오르기 + 페이드 아웃
        Vector3 endPosition = rectTransform.position + Vector3.up * distance;
        sequence.Append(rectTransform.DOMove(endPosition, duration).SetEase(Ease.OutQuad));
        sequence.Join(canvasGroup.DOFade(0f, duration).SetEase(Ease.OutQuad));

        // 애니메이션 완료 후 오브젝트 제거
        sequence.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
