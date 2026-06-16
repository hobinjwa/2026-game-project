using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class CameraCharacter : MonoBehaviour
{
    public float spawnHelper = 4.5f;
    public GameObject objPrefab;
    public float ballForce = 700;
    public float ballCount;
    private Camera cam;
    public float camMove;
    
    public Image cursor;
    public bool playState;
    Rigidbody rb;
    BallManager ballManager;
    public TextMeshProUGUI ballCountText;
    public TextMeshProUGUI mainText;
    public int hitCount;

    void Start()
    {
        ballManager = FindObjectOfType<BallManager>();
        cam = GetComponent<Camera>();
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;

        // 게임 시작 시 점수 및 콤보 초기화
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }

        ComboSystem comboSystem = FindObjectOfType<ComboSystem>();
        if (comboSystem != null)
        {
            comboSystem.ResetForNewGame();
        }

        playState = true;
        if (ballCountText != null)
            ballCountText.text = ballCount.ToString();
        else
            Debug.LogWarning("CameraCharacter: 'ballCountText' is not assigned in the Inspector.");
    }

    void Update()
    {
        if (playState == true)
        {
            cursor.transform.position = Input.mousePosition;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Quaternion targetRotation = Quaternion.LookRotation(ray.direction);

            rb.linearVelocity = new Vector3(0, 0, camMove);

            float mousePosx = Input.mousePosition.x;
            float mousePosy = Input.mousePosition.y;
            Vector3 BallInstantiatePoint = cam.ScreenToWorldPoint(new Vector3(mousePosx, mousePosy, cam.nearClipPlane + spawnHelper));

            if (Input.GetMouseButtonDown(0) && ballCount > 0)
            {
                Vector3 targetloc = ray.direction;
                ballCount -= 1;

                // 게임 통계 기록
                if (GameStatistics.Instance != null)
                {
                    GameStatistics.Instance.RecordShot();
                }

                GameObject ballRigid;
                if (objPrefab == null)
                {
                    Debug.LogError("CameraCharacter: 'objPrefab' is null. Assign a prefab in the Inspector to instantiate.");
                    return;
                }

                ballRigid = Instantiate(objPrefab, BallInstantiatePoint, transform.rotation) as GameObject;
                ballRigid.GetComponent<Rigidbody>().AddForce(targetloc * ballForce);
                ballCountText.text = ballCount.ToString();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            playState = false;

            // 점수 가져오기
            int finalScore = ScoreManager.Instance.GetTotalScore();

            // 점수 기록 저장
            if (ScoreHistory.Instance != null)
            {
                ScoreHistory.Instance.SaveScore(finalScore);
            }

            if (hitCount >= 10)
            {
                mainText.text = $"Success!\nScore: {finalScore}";
                mainText.transform.DOScale(1, 1).SetEase(Ease.OutBack);
            }
            else
            {
                mainText.text = $"You Failed!\nScore: {finalScore}";
                mainText.transform.DOScale(1, 1).SetEase(Ease.OutBack);
            }

            rb.isKinematic = true;
            // 연속 쉐이크가 실행 중이면 중지
            CameraShake camShake = FindObjectOfType<CameraShake>();
            if (camShake != null)
            {
                camShake.StopContinuousShake();
            }
            Invoke(nameof(RestartLevel), 4);
        }
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}