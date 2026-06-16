using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BallManager : MonoBehaviour
{
    private CameraCharacter cameraCharacter;
    private ContactPoint lastContactPoint;
    private CameraShake cameraShake;

    private void Start()
    {
        cameraCharacter = FindObjectOfType<CameraCharacter>();
        cameraShake = FindObjectOfType<CameraShake>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Block"))
        {
            GameObject obj = other.gameObject;
            cameraCharacter.hitCount += 1;

            // 충돌 위치 정보 저장
            if (other.contactCount > 0)
            {
                lastContactPoint = other.GetContact(0);
            }

            // 점수 시스템 연동
            int scoreEarned = ProcessTargetHit(obj, lastContactPoint.point);

            // 충돌 이펙트 표시
            HitEffectParticles.ShowHitEffect(lastContactPoint.point, scoreEarned);

            // 콤보를 고려한 카메라 쉐이크: ScoreManager.AddScoreByAccuracy() 내부에서 콤보가 증가하므로,
            // ProcessTargetHit가 완료된 후 현재 콤보를 조회하여 쉐이크 강도를 결정합니다.
            int currentCombo = 0;
            if (ComboSystem.Instance != null)
            {
                currentCombo = ComboSystem.Instance.GetCurrentCombo();
            }

            if (cameraShake != null)
            {
                // 콤보가 연속 쉐이크 임계값보다 크거나 같으면 연속 쉐이크로 처리 (이미 시작되었을 수 있음)
                if (ComboSystem.Instance != null && currentCombo >= ComboSystem.Instance.GetStartContinuousThreshold())
                {
                    // 연속 쉐이크 강도만 업데이트 (쉐이크 자체는 ComboSystem에서 시작됨)
                    cameraShake.UpdateContinuousMultiplier(currentCombo);
                }
                else
                {
                    // 임계값 이전에는 콤보 기반 즉시 쉐이크를 사용
                    cameraShake.ShakeForCombo(currentCombo);
                }
            }

            // 기존 효과 유지
            obj.GetComponent<MeshRenderer>().material.color = Color.green;
            obj.transform.DOMoveY(-6, 0.75f).SetEase(Ease.InBack).OnComplete(() =>
            {
                obj.SetActive(false);
            });
        }
    }

    /// <summary>
    /// 표적 맞춘 처리: 정확도 계산 및 점수 부여
    /// 반환값: 획득한 점수
    /// </summary>
    private int ProcessTargetHit(GameObject target, Vector3 hitPosition)
    {
        // TargetDetector 컴포넌트 가져오기
        TargetDetector targetDetector = target.GetComponent<TargetDetector>();
        int earnedScore = 0;

        if (targetDetector != null)
        {
            // 정확도 계산 (0~1)
            float accuracy = targetDetector.CalculateAccuracy(hitPosition);

            // 기본 점수 계산
            earnedScore = CalculateBaseScore(accuracy);

            // 점수 매니저에 정확도 기반 점수 추가
            ScoreManager.Instance.AddScoreByAccuracy(accuracy, hitPosition);

            // 게임 통계 기록
            if (GameStatistics.Instance != null)
            {
                GameStatistics.Instance.RecordHit(earnedScore);
            }

            // 디버그 로그
            Debug.Log($"<color=cyan>💥 명중!</color> 정확도: {accuracy:P0} ({targetDetector.GetAccuracyGrade(accuracy)})");
        }
        else
        {
            Debug.LogWarning("TargetDetector 컴포넌트가 없습니다. Block 프리팹에 추가하세요.");
        }

        return earnedScore;
    }

    /// <summary>
    /// 정확도 기반 기본 점수 계산
    /// </summary>
    private int CalculateBaseScore(float accuracy)
    {
        accuracy = Mathf.Clamp01(accuracy);

        if (accuracy >= 0.6f)
            return 3;
        else if (accuracy >= 0.3f)
            return 2;
        else
            return 1;
    }
}
