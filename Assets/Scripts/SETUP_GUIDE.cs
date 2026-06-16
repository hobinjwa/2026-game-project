using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ========================================
/// 📊 통합 점수 시스템 + 추가 기능 설정 가이드
/// ========================================
/// 
/// 이 스크립트 패키지를 사용하기 위한 완전한 설정 방법입니다.
/// 
/// 
/// [포함된 스크립트]
/// ========================================
/// 
/// 핵심 시스템:
/// 1. ScoreManager.cs - 점수 관리 및 배수 시스템
/// 2. TargetDetector.cs - 표적 정확도 판단
/// 3. FloatingTextEffect.cs - 점수 이펙트
/// 4. BallManager.cs - 공 충돌 처리
/// 
/// 추가 시스템:
/// 5. ComboSystem.cs ⭐ - 연속 명중 콤보 (1.25x ~ 2배 보너스)
/// 6. SoundEffectManager.cs ⭐ - 점수/콤보 사운드 효과
/// 7. ScoreHistory.cs ⭐ - 점수 기록 및 최고 기록 저장
/// 
/// 
/// [STEP 1] 기본 씬 설정 (이미 설명됨)
/// ========================================
/// 
/// • ScoreManager GameObject 생성
/// • Block 프리팹에 TargetDetector 추가
/// • FloatingTextEffect 프리팹 생성 및 할당
/// → SETUP_GUIDE.cs의 이전 내용 참고
/// 
/// 
/// [STEP 2] 콤보 시스템 설정 ⭐ NEW
/// ========================================
/// 
/// 1) ComboSystem GameObject 생성:
///    - Hierarchy에서 빈 GameObject 생성
///    - 이름: "ComboSystem"
///    - ComboSystem.cs 스크립트 추가
/// 
/// 2) 인스펙터 설정:
///    - Combo Text: Combo 표시 UI (TextMeshProUGUI)
///      예: Canvas > ComboText
///    - Combo Reset Time: 3초 (연타 끊길 때까지의 시간)
///    
///    배수 설정 (기본값으로 충분함):
///    - Combo 5 Threshold: 5연타 시작
///    - Combo 5 Multiplier: 1.25배
///    - Combo 10 Threshold: 10연타 시작
///    - Combo 10 Multiplier: 1.5배
///    - Combo 20 Threshold: 20연타 시작
///    - Combo 20 Multiplier: 2배
/// 
/// 3) 작동 방식:
///    - 연타 성공: 점수 1.25배 → 1.5배 → 2배 증가
///    - 3초 이상 못 맞추면 콤보 끊김
///    - 콤보 달성 시 콘솔에 🔥 이모지와 함께 로그 표시
/// 
/// 
/// [STEP 3] 사운드 시스템 설정 ⭐ NEW
/// ========================================
/// 
/// 1) SoundEffectManager GameObject 생성:
///    - Hierarchy에서 빈 GameObject 생성
///    - 이름: "SoundEffectManager"
///    - SoundEffectManager.cs 스크립트 추가
///    - AudioSource 컴포넌트 자동으로 추가됨
/// 
/// 2) 오디오 클립 할당 (Assets > Audio 폴더에서):
///    - Center Hit Sound: 중앙 명중음 (높은 음)
///    - Middle Hit Sound: 중간 명중음 (중간 음)
///    - Edge Hit Sound: 외곽 명중음 (낮은 음)
///    - Combo 5 Sound: 5연타 효과음
///    - Combo 10 Sound: 10연타 효과음
///    - Combo 20 Sound: 20연타 효과음
/// 
///    💡 팁: 없으면 표준 UI 사운드를 사용해도 됩니다.
///           Unity에서 기본 제공하는 사운드 참고
/// 
/// 3) 볼륨 설정:
///    - Master Volume: 0~1 (기본 0.5)
///    - Hit Volume: 0~1 (기본 0.7)
///    - Combo Volume: 0~1 (기본 1.0)
/// 
/// 
/// [STEP 4] 점수 기록 시스템 설정 ⭐ NEW
/// ========================================
/// 
/// 1) ScoreHistory GameObject 생성:
///    - Hierarchy에서 빈 GameObject 생성
///    - 이름: "ScoreHistory"
///    - ScoreHistory.cs 스크립트 추가
/// 
/// 2) 인스펙터 설정:
///    - Score History Text: 점수 기록 표시 UI
///      예: Canvas > ScoreHistoryPanel > HistoryText
///    - Max History Count: 5 (표시할 최근 기록 개수)
/// 
/// 3) 작동 방식:
///    - 게임 종료 시 점수 자동 저장
///    - 최고 기록은 PlayerPrefs에 저장됨 (게임 재시작 후에도 유지)
///    - UI에 최고 기록 + 최근 5개 점수 표시
/// 
///    💾 저장 위치:
///       Windows: C:\\Users\\USERNAME\\AppData\\LocalLow\\YourCompany\\YourGame
///       Mac: ~/Library/Preferences/com.YourCompany.YourGame
///       Android: 기기 내부 저장소
/// 
/// 
/// [완성된 씬 구조]
/// ========================================
/// 
/// Hierarchy:
/// ├── Camera
/// │   └── (CameraCharacter 스크립트)
/// ├── Canvas
/// │   ├── Score (TextMeshProUGUI) - ScoreManager "Score Text"
/// │   ├── Combo (TextMeshProUGUI) - ComboSystem "Combo Text"
/// │   ├── BallCount (TextMeshProUGUI) - CameraCharacter "ballCountText"
/// │   ├── MainText (TextMeshProUGUI) - CameraCharacter "mainText"
/// │   └── ScoreHistoryPanel
/// │       └── HistoryText - ScoreHistory "Score History Text"
/// ├── ScoreManager (GameObject)
/// │   └── ScoreManager.cs 스크립트
/// ├── ComboSystem (GameObject)
/// │   └── ComboSystem.cs 스크립트
/// ├── SoundEffectManager (GameObject)
/// │   ├── SoundEffectManager.cs 스크립트
/// │   └── AudioSource (자동 추가)
/// ├── ScoreHistory (GameObject)
/// │   └── ScoreHistory.cs 스크립트
/// ├── Block (프리팹)
/// │   └── TargetDetector.cs 스크립트
/// ├── Obstacles...
/// └── ...
/// 
/// 
/// [점수 시스템 작동 흐름]
/// ========================================
/// 
/// 1. 공 발사 → 표적 맞춤
/// 2. BallManager.OnCollisionEnter() 호출
/// 3. TargetDetector.CalculateAccuracy() 정확도 계산
/// 4. ScoreManager.AddScoreByAccuracy() 점수 계산
/// 5. ComboSystem.AddCombo() 콤보 증가
/// 6. 배수 적용 (예: 3점 × 1.5배 = 4점)
/// 7. FloatingTextEffect 표시 (+4, 핑크색)
/// 8. SoundEffectManager 사운드 재생
/// 
/// 콤보 리셋 (3초 이상 못 맞출 경우):
/// - ComboSystem.ResetCombo() 호출
/// - 콤보 카운트 초기화
/// - UI 업데이트
/// 
/// 
/// [커스터마이징 예제]
/// ========================================
/// 
/// 1. 콤보 배수 변경:
///    ComboSystem 인스펙터에서 Combo Multiplier 값 조정
///    예: 1.5배 → 2배로 변경
/// 
/// 2. 콤보 시간 연장:
///    ComboSystem의 "Combo Reset Time" 증가
///    예: 3초 → 5초로 변경하면 더 오래 콤보 유지
/// 
/// 3. 정확도 기준 변경:
///    ScoreManager.cs의 CalculateScoreByAccuracy() 메서드 수정
///    ```csharp
///    if (accuracy >= 0.7f)  // 60% → 70%로 변경
///        return centerScore;
///    ```
/// 
/// 4. 사운드 비활성화:
///    SoundEffectManager 인스펙터의 Master Volume을 0으로 설정
/// 
/// 
/// [디버그 팁]
/// ========================================
/// 
/// 콘솔 로그 확인:
/// - <color=cyan>명중!</color> - 기본 명중 로그
/// - <color=yellow>🔥 5연타!</color> - 콤보 달성
/// - <color=orange>🔥🔥 10연타!</color> - 콤보 달성
/// - <color=red>🔥🔥🔥 20연타!!!</color> - 최대 콤보
/// - <color=red>콤보 끊김!</color> - 콤보 초기화
/// - <color=gold>🏆 새 최고 기록!</color> - 새로운 최고 점수
/// 
/// 
/// [예상 게임플레이]
/// ========================================
/// 
/// 예 1. 기본 플레이:
/// • 표적 1~10개 맞춤
/// • 각 명중: 1~3점 (정확도에 따라)
/// • 총 점수: 약 15~30점
/// 
/// 예 2. 콤보 유지 플레이:
/// • 연속 5개 맞춤: 기본 12점 × 1.25배 = 15점
/// • 연속 10개 맞춤: 기본 12점 × 1.5배 = 18점
/// • 연속 20개 맞춤: 기본 12점 × 2배 = 24점
/// • 총 점수 획득: 훨씬 높음!
/// 
/// 
/// [주의사항]
/// ========================================
/// 
/// 1. 각 매니저는 Singleton으로 구현됨
///    → 씬에 하나씩만 존재해야 함
/// 
/// 2. 사운드 할당 미필수
///    → 없으면 경고 없이 무시됨
/// 
/// 3. DoTween 라이브러리 필수
///    → Assets > Plugins > DoTween이 있는지 확인
/// 
/// 4. TextMeshPro 필수
///    → UI 텍스트는 반드시 TextMeshPro 사용
/// 
/// 
/// [최종 확인 리스트]
/// ========================================
/// 
/// ✓ ScoreManager 생성 및 설정
/// ✓ ComboSystem 생성 및 설정
/// ✓ SoundEffectManager 생성 및 설정
/// ✓ ScoreHistory 생성 및 설정
/// ✓ Block 프리팹에 TargetDetector 추가
/// ✓ FloatingTextEffect 프리팹 생성 및 할당
/// ✓ Canvas UI 완성 (Score, Combo, History 등)
/// ✓ 모든 매니저의 UI 필드 할당
/// ✓ 게임 실행 및 테스트
/// 
/// 모두 확인되면 완성입니다! 🎉
/// 
/// ========================================
/// </summary>
public class SETUP_GUIDE { }
