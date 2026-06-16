using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 표적(Block)에 붙어서 공이 맞은 위치의 정확도를 판단하는 클래스
/// 중앙/중간/외곽 구역을 판단하여 점수를 부여함
/// </summary>
public class TargetDetector : MonoBehaviour
{
    [Header("정확도 판단")]
    [SerializeField] private float targetRadius = 5f; // 표적의 전체 반지름

    private Collider targetCollider;
    private Vector3 targetCenter;

    private void Start()
    {
        targetCollider = GetComponent<Collider>();
        
        if (targetCollider == null)
        {
            Debug.LogError("TargetDetector: Collider가 없습니다. Collider를 추가하세요.");
        }

        // 표적의 중심점 저장
        targetCenter = transform.position;
    }

    /// <summary>
    /// 맞은 위치를 바탕으로 정확도(0~1) 계산
    /// 1 = 중앙, 0 = 외곽
    /// </summary>
    public float CalculateAccuracy(Vector3 hitPosition)
    {
        // 표적 중심에서 맞은 위치까지의 거리
        float distance = Vector3.Distance(targetCenter, hitPosition);

        // 거리를 정확도로 변환 (역수 관계: 거리 작을수록 정확도 높음)
        float accuracy = 1f - (distance / targetRadius);

        // 정확도를 0~1 범위로 제한
        return Mathf.Clamp01(accuracy);
    }

    /// <summary>
    /// 정확도를 바탕으로 점수 등급 반환 (디버깅용)
    /// </summary>
    public string GetAccuracyGrade(float accuracy)
    {
        if (accuracy >= 0.6f)
            return "중앙 (3점)";
        else if (accuracy >= 0.3f)
            return "중간 (2점)";
        else
            return "외곽 (1점)";
    }

    /// <summary>
    /// 표적의 반지름 반환
    /// </summary>
    public float GetTargetRadius()
    {
        return targetRadius;
    }

    /// <summary>
    /// 표적의 중심 위치 반환
    /// </summary>
    public Vector3 GetTargetCenter()
    {
        return targetCenter;
    }
}
