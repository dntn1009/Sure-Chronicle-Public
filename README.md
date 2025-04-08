# 🧙‍♂️ 수레크로니클 (Surae Chronicle)

Unity로 개발한 방치형 RPG 게임 프로젝트입니다.  
Firebase, Google Cloud, Addressable, 인앱 결제, AdMob 등을 통합하여 실제 모바일 게임 서비스 수준의 구조를 구현하였습니다.  
이 저장소는 **코드 구조와 시스템 구성 예시를 보여주기 위한 Public 리포지토리**로, 실행보다는 참고용으로 구성되어 있습니다.



---

## 📆 개발 기간

> **2024.6 ~ 2025.03 (약 9개월)**  
> 개인 개발 (기획 / 구현 / Firebase 연동 포함 전체 시스템 직접 개발)
<details>
<summary>📆 개발 연대기 (전체 보기)</summary>

### 2024년 6월 ~ 7월  
- 게임 기획 및 기본 UI 레이아웃 설계  
- 2.5D 카메라 구조 및 맵 구현 준비  
- URP 및 커브 셰이더(Graph)를 적용하여 원통형 월드 구현  
- 타일맵 기반 맵 제작, 캐릭터 기본 컨트롤 및 이동 구현  
- SPUM 에셋 기반 캐릭터 및 몬스터 리소스 세팅  
- 캐릭터 카드 시스템 및 UI 구성 시작  

### 2024년 8월  
- 카드 시스템 본격 구현: 수집, 강화, 초월 등 데이터 구조 설계  
- HeroWindow, PullWindow UI 구현 및 소환 로직 개발  
- 디자인 패턴(Facade, Adapter, Composite 등) 적극 적용  
- Firebase 연동 시작, 유저 정보 구조 설계  
- Addressable 및 구조 최적화 기반 마련  

### 2024년 9월  
- 카드 장착 및 슬롯 기반 히어로 소환 시스템 구현  
- 히어로 FSM 적용 및 자동 전투 시스템 구현  
- 맵의 곡선 셰이더 최적화 및 타일맵 정리  
- 몬스터 FSM(State Machine) 구현 및 사냥 로직 개발  
- 카드 및 보상 UI 개선  
- Chapter, Stage, Boss 전투 구조 설계  
- Firebase RealtimeDatabase 및 Auth 기능 안정화  

### 2024년 10월  
- 히어로, 몬스터 FSM 완성 및 전투 시스템 통합  
- 각종 UI 개선 및 보스 처치 결과 처리 로직 추가  
- Raid 모드 및 티켓 시스템 구상  
- SPUM 기반 애니메이션 커스터마이징  
- Shader Graph를 활용한 배경 및 건물 투명화 구현  
- 구글 로그인 및 계정 인증 처리(GPGS 2.0)  

### 2024년 11월  
- Google Play Games 인증 및 Firebase 계정 연결 완료  
- 몬스터 스폰, 행동 로직, FSM 정교화  
- Hero Auto 전투, AI 동작 개선 및 NavMesh 최적화  
- 보스전 진행 후 챕터 결과 처리 및 재도전/다음 챕터 로직 구축  
- Firebase Functions를 통해 서버 사이드 로직 구축 준비  

### 2024년 12월  
- Firebase Functions로 결제 검증 및 보상 처리  
- 카드 뽑기, 강화 UI 및 데이터 흐름 정비  
- Hero/Monster 피해 연출 및 데미지 폰트 구현  
- 유저 데이터 클래스 구조로 정비, 구조화  
- 관리자용 AdminScene 구현 – 메시지/쿠폰 발행 기능  
- 클라우드 펑션 기반 티켓 스케줄 처리  

### 2025년 1월  
- 인앱 결제 시스템 구현 완료 (Unity IAP, 클라우드 검증 포함)  
- AdMob 연동 및 리워드 광고 보상 처리  
- Hero 총 30종 구성 및 카드 강화 확률 조정  
- Firebase Functions에서 새벽 티켓 자동 지급 스케줄 구현  
- 실 기기 테스트 및 Google Play Console 공개 테스트 준비  

### 2025년 2월  
- 게임 씬 최종 구성 및 최적화  
- Addressable 리소스 다운로드 구현 (Firebase Hosting 연동)  
- AudioManager 및 사운드 어드레서블로 구성  
- 게임 보호정책 UI, 추가 설치 UI, 로딩 UI 등 배포 전 마무리  
- 계정 신규 여부 자동 판단, 데이터 생성 로직 정비  
- Google Play 공개 테스트 등록 및 확률표 표기 등 정책 대응  

### 2025년 3월  
- 게임의 전체 흐름 안정화, 버그 수정  
- 슬롯 장착 시 히어로 생성 및 동기화 버그 해결  
- 챕터-스테이지 전환 처리 완성  
- 테스트 배포 완료 및 마켓 대응 준비

</details>

---



---

## 📱 주요 기능

- 히어로 수집 및 자동 사냥 기반의 **방치형 RPG 시스템**
- **Tilemap 기반** 건축물 **자동 투명 처리 구현** (플레이어 근접 시 시야 확보)
- **Firebase Realtime Database**를 통한 실시간 유저 데이터 저장
- **Firebase Functions**를 활용한 인앱 결제 검증 및 티켓 자동 지급 로직
- **Google Play Games Services (GPGS) V2.0**을 이용한 로그인 및 업적 시스템 구현
- **AdMob** 배너 광고 및 보상형 광고 연동
- **Addressable** 기반 리소스 관리 및 최적화 구조
- **URP + Shader Graph** 기반 2D Sprite 곡률 변형 시각 효과
- 카드 장착 → 히어로 소환 → 슬롯 추적 시스템
- 쿠폰 입력 / 메시지함을 통한 보상 수령 시스템

---

## 🛠 사용 기술 스택

- Unity 2022.3.55f1
- Firebase (Auth / Realtime DB / Functions / Hosting)
- Google Cloud Platform
- Google Play Console / Billing (IAP)
- AdMob
- Addressable Asset System
- C# / ScriptableObject / PlayerPrefs 등

---

## 📎 참고

> 🔐 이 리포지토리는 실행보다는 **구조 참고용으로 제공되는 Public 버전**입니다.  
> Firebase 키, 서비스 계정, 인앱결제 연동 정보, 외부 에셋 등은 모두 제거되어 있으며, 민감 정보는 포함되어 있지 않습니다.

---

## 📂 프로젝트 구조 예시

```plaintext
Assets/
├── 1.Scenes/                 # 게임 씬 (로비, 전투, 결과 등)
├── 2.Scripts/               # 전체 게임 로직과 시스템 코드
├── 3.Sprites/               # 게임 및 UI 리소스 (아이콘, 배경, 오브젝트 등)
├── 4.Prefabs/               # 프리팹 오브젝트 모음
├── 5.Animations/            # 애니메이션 클립 및 애니메이터 컨트롤러
├── 6.Material/              # 머티리얼 및 셰이더 관련 리소스
├── 7.URP/                   # URP 설정 및 렌더 파이프라인 관련 데이터
├── 8.Sound/                 # 효과음, 배경음 등 오디오 리소스
├── 998.DevelopMentLog/      # 일정별 개발 기록, 기능 진행 내용
├── 999.ExternalAssets/      # 외부 에셋 스토어 리소스
├── AddressableAssetsData/   # Addressables 설정 파일
├── Editor/                  # 커스텀 에디터 스크립트 (툴 편의성 향상 목적)
├── SPUM/                    # 외부 에셋 스토어 리소스 (캐릭터 제작)

functions/
├── index.js                    # Firebase Functions 진입점 (인앱 결제 검증 등)
├── package.json                # 의존성 목록
├── package-lock.json           # 의존성 버전 고정 파일
├── play-billing-service.json   # GCP 서비스 계정 키 (민감 정보로 리포 제외)
├── node_modules/               # 자동 설치되는 라이브러리 폴더 (리포 제외)
