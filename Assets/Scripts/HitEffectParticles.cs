using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 공이 표적을 맞출 때 나타나는 파티클 이펙트
/// 색상, 크기, 회전 등을 동적으로 표시
/// </summary>
public class HitEffectParticles : MonoBehaviour
{
    [Header("파티클 설정")]
    [SerializeField] private int particleCount = 15;           // 파티클 개수
    [SerializeField] private float particleLifetime = 0.8f;    // 파티클 지속 시간
    [SerializeField] private float particleSpeed = 8f;         // 파티클 속도
    [SerializeField] private float particleSize = 0.3f;        // 파티클 크기

    /// <summary>
    /// 충돌 이펙트 표시 (정확도별 색상)
    /// </summary>
    public static void ShowHitEffect(Vector3 position, int points)
    {
        // 점수에 따른 색상 결정
        Color effectColor = points switch
        {
            3 => Color.yellow,                      // 중앙: 노란색
            2 => new Color(1f, 0.65f, 0f),         // 중간: 주황색
            1 => Color.red,                         // 외곽: 빨간색
            _ => Color.white
        };

        CreateParticleEffect(position, effectColor, points);
    }

    /// <summary>
    /// 파티클 이펙트 생성
    /// </summary>
    private static void CreateParticleEffect(Vector3 position, Color color, int points)
    {
        int particleCount = 15 + (points * 3);  // 점수가 높을수록 파티클 많음

        // 중심에 큰 빛 이펙트
        GameObject lightEffect = new GameObject("HitLight");
        lightEffect.transform.position = position;
        
        // 라이트 추가 (빠르게 페이드 아웃)
        Light light = lightEffect.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 15f;
        light.intensity = 3f;
        light.color = color;

        // 라이트 점진적 사라지기
        var lightSequence = DOTween.Sequence();
        lightSequence.Append(DOTween.To(() => light.intensity, x => light.intensity = x, 0f, 0.3f))
            .OnComplete(() => Destroy(lightEffect));

        // 파티클들 생성
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(particle.GetComponent<Collider>());
            Destroy(particle.GetComponent<Rigidbody>());
            
            particle.name = "HitParticle";
            particle.transform.position = position;
            particle.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            // 재질 설정
            Material mat = particle.GetComponent<MeshRenderer>().material;
            mat.color = color;
            mat.SetFloat("_Glossiness", 0.8f);

            // 랜덤 방향으로 분산
            Vector3 direction = Random.insideUnitSphere.normalized;
            float speed = Random.Range(5f, 12f);

            // Rigidbody 추가 (물리 적용)
            Rigidbody rb = particle.AddComponent<Rigidbody>();
            rb.linearVelocity = direction * speed;
            rb.useGravity = true;
            rb.linearDamping = 0.5f;

            // 페이드 아웃 애니메이션
            Color startColor = color;
            Color endColor = new Color(color.r, color.g, color.b, 0);
            
            var sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(() => mat.color, x => mat.color = x, endColor, 0.7f))
                .OnComplete(() => Destroy(particle));

            // 회전 애니메이션
            particle.transform.DORotate(new Vector3(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f)), 0.7f);
        }

        // 임팩트 텍스트 ("HIT!")
        CreateImpactText(position, points);
    }

    /// <summary>
    /// 임팩트 텍스트 표시 ("HIT!")
    /// </summary>
    private static void CreateImpactText(Vector3 position, int points)
    {
        GameObject textObj = new GameObject("ImpactText");
        textObj.transform.position = position + Vector3.up * 2f;

        // TextMeshPro 임시 UI 텍스트 (월드 스페이스)
        // 대신 간단한 표시로 처리
        
        Debug.Log($"💥 HIT! 위치: {position}, 점수: {points}");
    }
}
