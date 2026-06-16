using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 강화된 카메라 쉐이크 효과
/// 콤보 달성 시 카메라를 강하게 흔들어 임팩트 표현
/// 결승선까지 지속적으로 강하게 흔들림
/// </summary>
public class CameraShake : MonoBehaviour
{
    [Header("쉐이크 설정")]
    [SerializeField] private float shakeIntensity = 1.5f;      // 흔들림 강도 (강화)
    [SerializeField] private float shakeDuration = 0.6f;       // 흔들림 지속 시간 (강화)
    [SerializeField] private float shakeSpeed = 15f;           // 흔들림 속도 (빠르게)

    private Vector3 originalPosition;
    private Camera mainCamera;
    private Coroutine shakeCoroutine;
    private Coroutine continuousCoroutine;
    private float continuousMultiplier = 2.5f;
    private Vector3 lastShakeOffset = Vector3.zero;
    private Vector3 continuousOffset = Vector3.zero;
    private Vector3 oneShotOffset = Vector3.zero;
    private Vector3 lastAppliedOffset = Vector3.zero;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// 카메라 쉐이크 시작 (약한 흔들림)
    /// </summary>
    public void ShakeLight()
    {
        Shake(shakeIntensity * 2.0f, shakeDuration * 1.0f);
    }

    /// <summary>
    /// 카메라 쉐이크 시작 (중간 흔들림)
    /// </summary>
    public void ShakeMedium()
    {
        Shake(shakeIntensity * 3.5f, shakeDuration * 1.3f);
    }

    /// <summary>
    /// 카메라 쉐이크 시작 (강한 흔들림) - 최대 강도
    /// </summary>
    public void ShakeStrong()
    {
        Shake(shakeIntensity * 6.0f, shakeDuration * 2.0f);
    }

    /// <summary>
    /// 공 충돌 시 흔들림 (즉각적 임팩트)
    /// </summary>
    public void ShakeOnHit()
    {
        Shake(shakeIntensity * 1.8f, shakeDuration * 0.4f);
    }

    /// <summary>
    /// 콤보 수에 따라 쉐이크 세기를 자동으로 계산하고 실행
    /// 콤보가 높을수록 더 강하고 오래 흔들린다.
    /// </summary>
    public void ShakeForCombo(int combo)
    {
        if (combo <= 0)
        {
            // 기본 히트 쉐이크
            ShakeOnHit();
            return;
        }

        // combo 기반 가중치 계산
        float intensityMultiplier = 1f + (combo * 0.35f); // 콤보 1당 0.35배 증가
        float durationMultiplier = 1f + (combo * 0.15f);

        Shake(shakeIntensity * intensityMultiplier, shakeDuration * durationMultiplier);
    }

    /// <summary>
    /// 연속 흔들림 (게임 진행 중 계속 흔들리게)
    /// </summary>
    public void ShakeContinuous()
    {
        // short continuous burst
        Shake(shakeIntensity * 2.5f, shakeDuration * 3f);
    }

    /// <summary>
    /// 시작하면 중지될 때까지 계속 카메라를 흔듭니다 (결승까지 지속 가능)
    /// </summary>
    public void StartContinuousShake()
    {
        if (continuousCoroutine != null) return; // 이미 실행 중
        continuousCoroutine = StartCoroutine(ContinuousShakeCoroutine());
    }

    /// <summary>
    /// 연속 쉐이크 중지
    /// </summary>
    public void StopContinuousShake()
    {
        if (continuousCoroutine != null)
        {
            StopCoroutine(continuousCoroutine);
            continuousCoroutine = null;
            // 이전에 적용한 오프셋이 있으면 제거해서 물리/이동 기반 위치로 복구
            transform.localPosition = transform.localPosition - lastShakeOffset;
            lastShakeOffset = Vector3.zero;
        }
    }

    private IEnumerator ContinuousShakeCoroutine()
    {
        float noiseOffsetX = Random.Range(0f, 100f);
        float noiseOffsetY = Random.Range(0f, 100f);
        while (true)
        {
            float xShake = (Mathf.PerlinNoise(noiseOffsetX + Time.time * shakeSpeed, 0) - 0.5f) * 2f;
            float yShake = (Mathf.PerlinNoise(noiseOffsetY + Time.time * shakeSpeed, 1) - 0.5f) * 2f;
            Vector3 newOffset = new Vector3(xShake, yShake, 0) * shakeIntensity * continuousMultiplier;

            // 코루틴은 오프셋을 직접 적용하지 않고 별도 변수에 저장
            continuousOffset = newOffset;

            yield return null;
        }
    }

    /// <summary>
    /// 연속 쉐이크의 강도를 콤보에 따라 업데이트
    /// </summary>
    public void UpdateContinuousMultiplier(int combo)
    {
        // combo 가 커질수록 multiplier 증가
        continuousMultiplier = 1f + (combo * 0.35f);
    }

    /// <summary>
    /// 커스텀 쉐이크
    /// </summary>
    public void Shake(float intensity, float duration)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            oneShotOffset = Vector3.zero;
            // 즉시 적용된 마지막 오프셋 제거는 LateUpdate에서 처리
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    /// <summary>
    /// 쉐이크 코루틴 (Perlin 노이즈로 부드러운 진동)
    /// </summary>
    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsedTime = 0f;
        float noiseOffsetX = Random.Range(0f, 100f);
        float noiseOffsetY = Random.Range(0f, 100f);
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Perlin 노이즈를 사용한 부드러운 진동
            float xShake = (Mathf.PerlinNoise(noiseOffsetX + Time.time * shakeSpeed, 0) - 0.5f) * 2f;
            float yShake = (Mathf.PerlinNoise(noiseOffsetY + Time.time * shakeSpeed, 1) - 0.5f) * 2f;

            Vector3 newOffset = new Vector3(xShake, yShake, 0) * intensity;

            // one-shot 오프셋을 설정 (LateUpdate에서 합성)
            oneShotOffset = newOffset;

            yield return null;
        }

        // 끝나면 부드럽게 oneShotOffset을 0으로 감쇠시켜 제거
        float returnTime = 0.1f;
        float returnElapsed = 0f;
        Vector3 startOffset = oneShotOffset;

        while (returnElapsed < returnTime)
        {
            returnElapsed += Time.deltaTime;
            oneShotOffset = Vector3.Lerp(startOffset, Vector3.zero, returnElapsed / returnTime);
            yield return null;
        }

        oneShotOffset = Vector3.zero;
    }

    private void LateUpdate()
    {
        // physics나 다른 로직으로 이미 변경된 transform.localPosition을 기준으로 오프셋을 적용
        Vector3 totalOffset = continuousOffset + oneShotOffset;

        // basePos는 현재 transform에서 이전에 적용한 오프셋을 제거한 위치
        Vector3 basePos = transform.localPosition - lastAppliedOffset;
        Vector3 newPos = basePos + totalOffset;

        transform.localPosition = newPos;
        lastAppliedOffset = totalOffset;
    }
}
