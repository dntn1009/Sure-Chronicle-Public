# 🧙‍♂️ 수레크로니클 (Surae Chronicle)

Unity로 개발한 방치형 RPG 게임 프로젝트입니다.  
Firebase, Google Cloud, Addressable, 인앱 결제, AdMob 등을 통합하여 실제 모바일 게임 서비스 수준의 구조를 구현하였습니다.  
이 저장소는 **코드 구조와 시스템 구성 예시를 보여주기 위한 Public 리포지토리**로, 실행보다는 참고용으로 구성되어 있습니다.

---

## 📱 주요 기능

- 히어로 수집 및 자동 사냥 기반의 **방치형 RPG 시스템**
- **Firebase Realtime Database**를 통한 실시간 유저 데이터 저장
- **Firebase Functions**를 활용한 인앱 결제 검증 및 티켓 자동 지급 로직
- **AdMob** 배너 광고 및 보상형 광고 연동
- **Addressable** 기반 리소스 관리 및 최적화 구조
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
> Firebase 키, 서비스 계정, 인앱결제 연동 정보 등은 모두 제거되어 있으며, 민감 정보는 포함되어 있지 않습니다.

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
