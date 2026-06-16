using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ╔══════════════════════════════════════════════════════════════════════════════╗
/// ║           🎮 점수 시스템 + 콤보 + 사운드 + 통계 완전 패키지 가이드 🎮          ║
/// ╚══════════════════════════════════════════════════════════════════════════════╝
/// 
/// 
/// 📦 포함된 모든 스크립트
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// 1️⃣ 핵심 점수 시스템
///    ├─ ScoreManager.cs            : 점수 관리, 차등 점수제 (1~3점)
///    ├─ TargetDetector.cs          : 정확도 판단 (중앙/중간/외곽)
///    └─ FloatingTextEffect.cs      : +3, +2, +1 이펙트 텍스트
/// 
/// 2️⃣ 콤보 시스템 ⭐
///    ├─ ComboSystem.cs             : 연속 명중 콤보 (1.25x ~ 2배 보너스)
///    ├─ CameraShake.cs             : 콤보 달성 시 카메라 흔들림
///    └─ SoundEffectManager.cs      : 콤보 사운드 이펙트
/// 
/// 3️⃣ 게임 기록 및 통계
///    ├─ ScoreHistory.cs            : 점수 기록 및 최고 점수 저장
///    ├─ GameStatistics.cs          : 명중률, 최고콤보 등 실시간 통계
///    └─ BallManager.cs (수정)     : 공 충돌 처리
/// 
/// 4️⃣ UI 통합
///    └─ CameraCharacter.cs (수정) : 게임 매니저 역할
/// 
/// 
/// 🎯 시스템 작동 흐름
/// ─────────────────────────────────────────────────────────────────────────────
/// 
///  공 발사
///    ↓
///  공 발사 기록 (GameStatistics.RecordShot)
///    ↓
///  표적 충돌 (Block과 충돌)
///    ↓
///  정확도 계산 (TargetDetector.CalculateAccuracy)
///    ↓
///  점수 계산 (1~3점 기본값)
///    ↓
///  콤보 배수 적용 (1.0배 ~ 2.0배)
///    ↓
///  최종 점수 = 기본점수 × 콤보배수
///    ↓
///  점수 획득 (ScoreManager.AddScore)
///    ├─ UI 업데이트
///    ├─ 플로팅 텍스트 표시 (위치: 충돌점)
///    ├─ 사운드 재생 (점수별 음색)
///    └─ 통계 기록 (GameStatistics.RecordHit)
///    ↓
///  콤보 증가 (ComboSystem.AddCombo)
///    ├─ 5연타: 카메라 Light 쉐이크 + 사운드
///    ├─ 10연타: 카메라 Medium 쉐이크 + 사운드
///    └─ 20연타: 카메라 Strong 쉐이크 + 사운드
///    ↓
///  게임 계속...
///    
///  (3초간 못 맞추면)
///    ↓
///  콤보 끊김 (ComboSystem.ResetCombo)
///
///  게임 종료 (Finish 트리거)
///    ↓
///  최종 점수 저장 (ScoreHistory.SaveScore)
///    ├─ 최고 기록 업데이트
///    └─ 점수 히스토리 추가
///    ↓
///  게임 재시작 또는 메뉴로 이동
/// 
/// 
/// ⚙️ 완성된 씬 구조 (최종 버전)
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// Hierarchy:
/// ├── Camera (Main)
/// │   ├── Camera 컴포넌트
/// │   ├── CameraCharacter.cs
/// │   ├── CameraShake.cs ⭐
/// │   └── Rigidbody
/// │
/// ├── Canvas (Screen Space)
/// │   ├── Score (TextMeshPro)              ← ScoreManager "Score Text"
/// │   ├── Combo (TextMeshPro)              ← ComboSystem "Combo Text"
/// │   ├── Statistics (TextMeshPro)         ← GameStatistics "Stats Text"
/// │   ├── BallCount (TextMeshPro)          ← CameraCharacter "ballCountText"
/// │   ├── MainText (TextMeshPro)           ← CameraCharacter "mainText"
/// │   ├── ScoreHistoryPanel (Panel)
/// │   │   └── HistoryText (TextMeshPro)    ← ScoreHistory "Score History Text"
/// │   └── (FloatingScore 프리팹들이 동적으로 생성됨)
/// │
/// ├── ScoreManager (GameObject)
/// │   ├── ScoreManager.cs
/// │   └── [Inspector]
/// │       ├── Score Text: Canvas > Score
/// │       ├── Floating Text Prefab: FloatingScoreText (프리팹)
/// │       └── (Center/Middle/Edge Score: 3, 2, 1)
/// │
/// ├── ComboSystem (GameObject) ⭐
/// │   ├── ComboSystem.cs
/// │   └── [Inspector]
/// │       ├── Combo Text: Canvas > Combo
/// │       └── (기본 배수 설정: 1.25x, 1.5x, 2x)
/// │
/// ├── SoundEffectManager (GameObject) ⭐
/// │   ├── SoundEffectManager.cs
/// │   ├── AudioSource (자동 추가)
/// │   └── [Inspector]
/// │       ├── Center Hit Sound: (오디오 클립)
/// │       ├── Middle Hit Sound: (오디오 클립)
/// │       ├── Edge Hit Sound: (오디오 클립)
/// │       ├── Combo 5 Sound: (오디오 클립)
/// │       ├── Combo 10 Sound: (오디오 클립)
/// │       └── Combo 20 Sound: (오디오 클립)
/// │
/// ├── ScoreHistory (GameObject) ⭐
/// │   ├── ScoreHistory.cs
/// │   └── [Inspector]
/// │       └── Score History Text: Canvas > ScoreHistoryPanel > HistoryText
/// │
/// ├── GameStatistics (GameObject) ⭐
/// │   ├── GameStatistics.cs
/// │   └── [Inspector]
/// │       └── Stats Text: Canvas > Statistics
/// │
/// ├── Block (프리팹 with TargetDetector)
/// │   ├── MeshRenderer
/// │   ├── Collider
/// │   ├── Rigidbody
/// │   └── TargetDetector.cs ⭐
/// │       └── Target Radius: 5 (블록별로 조정 가능)
/// │
/// ├── ObjectSpawner (GameObject)
/// │   ├── ObjectSpawner.cs
/// │   └── (Block 프리팹을 자동 생성)
/// │
/// ├── 물, 장애물, 기타 게임 오브젝트들...
/// │
/// └── BallManager (GameObject)
///     └── BallManager.cs (수정됨)
/// 
/// 
/// 📋 설정 체크리스트 (이것을 따라하면 완벽!)
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// ✅ 기본 설정 (Step 1~2)
///    ☐ ScoreManager GameObject 생성 + ScoreManager.cs 추가
///    ☐ FloatingScoreText 프리팹 생성 (TextMeshPro + FloatingTextEffect.cs)
///    ☐ Block 프리팹에 TargetDetector.cs 추가
///    ☐ Canvas에 Score, BallCount, MainText UI 추가
/// 
/// ✅ 콤보 시스템 (Step 3)
///    ☐ ComboSystem GameObject 생성 + ComboSystem.cs 추가
///    ☐ Canvas에 Combo 텍스트 추가
///    ☐ ComboSystem 인스펙터에 Canvas > Combo 할당
/// 
/// ✅ 사운드 시스템 (Step 4)
///    ☐ SoundEffectManager GameObject 생성 + SoundEffectManager.cs 추가
///    ☐ AudioSource 컴포넌트 확인 (자동 추가됨)
///    ☐ (선택) 오디오 클립 할당 (없어도 작동)
/// 
/// ✅ 카메라 쉐이크 (Step 5)
///    ☐ Main Camera에 CameraShake.cs 추가
///    ☐ (자동으로 ComboSystem과 연동)
/// 
/// ✅ 통계 시스템 (Step 6)
///    ☐ GameStatistics GameObject 생성 + GameStatistics.cs 추가
///    ☐ Canvas에 Statistics 텍스트 추가
///    ☐ GameStatistics 인스펙터에 Canvas > Statistics 할당
/// 
/// ✅ 점수 기록 (Step 7)
///    ☐ ScoreHistory GameObject 생성 + ScoreHistory.cs 추가
///    ☐ Canvas에 ScoreHistoryPanel > HistoryText UI 추가
///    ☐ ScoreHistory 인스펙터에 Canvas > ScoreHistoryPanel > HistoryText 할당
/// 
/// ✅ UI 정리 (Step 8)
///    ☐ Canvas > Score 할당 (ScoreManager "Score Text")
///    ☐ Canvas > Combo 할당 (ComboSystem "Combo Text")
///    ☐ Canvas > Statistics 할당 (GameStatistics "Stats Text")
///    ☐ Canvas > ScoreHistoryPanel > HistoryText 할당 (ScoreHistory)
/// 
/// ✅ 프리팹 최종 할당 (Step 9)
///    ☐ FloatingScoreText 프리팹을 ScoreManager "Floating Text Prefab"에 할당
/// 
/// ✅ 게임 테스트 (Step 10)
///    ☐ 공 발사 시 ballCount 감소
///    ☐ 표적 맞춤 → 점수 증가
///    ☐ 플로팅 텍스트 나타남 → 사라짐
///    ☐ 점수 UI 업데이트
///    ☐ 5연타 → 콤보 텍스트 + 카메라 쉐이크 + 사운드
///    ☐ 통계 UI 업데이트 (명중률, 최고콤보 등)
///    ☐ 게임 종료 → 점수 기록 저장
///    ☐ 게임 재시작 → 최고 기록 유지
/// 
/// 
/// 🎮 게임플레이 예상 점수
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// 시나리오 1: 일반 플레이 (콤보 없음)
///   • 표적 1개 맞춤 (중앙) → +3점
///   • 표적 1개 맞춤 (중간) → +2점
///   • 표적 1개 맞춤 (외곽) → +1점
///   • 3개 명중 시 총: 6점
/// 
/// 시나리오 2: 콤보 플레이 (5연타)
///   • 표적 1개 맞춤 (중앙) → 3 × 1.25배 = 3점
///   • (5번 연속 명중)
///   • 5번째 → 3 × 1.25배 = 3점 + 콤보 텍스트 + 카메라 쉐이크
///   • 5연타 시 총: 약 15점
/// 
/// 시나리오 3: 완벽한 콤보 플레이 (20연타)
///   • 표적 20개 연속 명중 (중앙)
///   • 5번째: 3 × 1.25배 = 3점
///   • 10번째: 3 × 1.5배 = 4점
///   • 20번째: 3 × 2.0배 = 6점
///   • 20연타 시 총: 약 80점
/// 
/// 
/// 💡 커스터마이징 예제
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// 1. 콤보 배수 너무 크다면:
///    ComboSystem 인스펙터 → Combo 10 Multiplier: 1.5 → 1.3 변경
/// 
/// 2. 콤보가 너무 빨리 끊긴다면:
///    ComboSystem 인스펙터 → Combo Reset Time: 3 → 5 변경
/// 
/// 3. 정확도 판정 기준 변경:
///    ScoreManager.cs → CalculateScoreByAccuracy() 메서드 수정
/// 
/// 4. 카메라 쉐이크 강도 조정:
///    CameraShake 인스펙터 → Shake Intensity: 0.2 → 0.5 변경
/// 
/// 5. 사운드 비활성화:
///    SoundEffectManager 인스펙터 → Master Volume: 0.5 → 0 변경
/// 
/// 
/// 🐛 문제 해결
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// Q. FloatingText가 안 보인다
/// A. Canvas가 Screen Space인지 확인
///    또는 ScoreManager의 "Floating Text Prefab" 필드가 비어있는지 확인
/// 
/// Q. 콤보가 작동하지 않는다
/// A. ComboSystem GameObject가 있는지 확인
///    Canvas > Combo 텍스트가 할당되었는지 확인
/// 
/// Q. 카메라가 흔들리지 않는다
/// A. Main Camera에 CameraShake.cs가 추가되었는지 확인
///    콤보 조건 (5, 10, 20연타)을 충족했는지 확인
/// 
/// Q. 사운드가 안 나온다
/// A. SoundEffectManager GameObject가 있는지 확인
///    오디오 클립이 할당되었는지 확인 (없어도 경고만 나고 작동함)
/// 
/// Q. 점수가 저장되지 않는다
/// A. ScoreHistory GameObject가 있는지 확인
///    PlayerPrefs가 비활성화되지 않았는지 확인
/// 
/// Q. 통계 UI가 안 보인다
/// A. GameStatistics GameObject가 있는지 확인
///    Canvas > Statistics 텍스트가 할당되었는지 확인
/// 
/// 
/// 📊 성과 측정
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// 게임플레이 후 메인 화면에 표시되는 정보:
/// 
///  명중률: 75% | 최고콤보: 12 | 총점수: 85
///  │           │               │
///  발사한 공의  연속 명중 최고  전체 얻은
///  몇 %가 맞았나 기록           최종 점수
/// 
/// 최고 기록: 250
/// • 85점
/// • 72점
/// • 61점
/// • 58점
/// • 43점
/// 
/// 
/// 🎯 다음 추가 기능 아이디어
/// ─────────────────────────────────────────────────────────────────────────────
/// 
/// 1. ⭐ 파티클 이펙트
///    점수 획득 시 파티클 이펙트 표시
/// 
/// 2. ⭐ 슬로우 모션 이펙트
///    콤보 달성 시 잠깐 게임 속도 감소
/// 
/// 3. ⭐ 순위표 (랭킹)
///    최고 점수 TOP 10 표시
/// 
/// 4. ⭐ 난이도 선택
///    EASY, NORMAL, HARD 선택 시 점수 배수 변경
/// 
/// 5. ⭐ 도전 과제
///    "5연타 달성", "100점 이상" 등 목표 달성
/// 
/// 
/// ═══════════════════════════════════════════════════════════════════════════════
/// 축하합니다! 이 가이드를 따라하면 완벽한 점수 시스템이 완성됩니다! 🎉
/// ═══════════════════════════════════════════════════════════════════════════════
/// </summary>
public class COMPLETE_GUIDE { }
