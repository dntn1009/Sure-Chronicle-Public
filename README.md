# 🧙‍♂️ 수레크로니클 (Surae Chronicle)

Unity로 개발한 방치형 RPG 게임 프로젝트입니다.  
Firebase, Google Cloud, Addressable, 인앱 결제, AdMob 등을 통합하여 실제 모바일 게임 서비스 수준의 구조를 구현하였습니다.  
이 저장소는 민감 정보가 제거된 **Public 버전**으로, 코드와 기능 구조를 공유하기 위한 용도로 구성되어 있습니다.

---

## 📱 주요 기능

- 히어로 수집 및 자동 사냥 기반의 **방치형 RPG 시스템**
- **Firebase Realtime Database**를 통한 유저 데이터 실시간 저장
- **Firebase Functions**를 통한 인앱 결제 검증 및 자정마다 티켓 지급 자동 처리
- **Addressables**로 구현된 동적 리소스 로딩 구조
- **AdMob** 배너 광고 및 보상형 광고 기능 연동
- **Google Play Billing (인앱 결제)** 기능 적용 및 서버 검증
- 카드 장착 → 히어로 소환 → 슬롯 따라다니는 시스템
- 쿠폰 입력 / 메시지함을 통한 보상 수령 기능

---

## 🛠 사용 기술 스택

- Unity 2022
- Firebase (Auth / Database / Hosting / Functions)
- Google Cloud Platform
- Google Play Console
- Google Play Billing (IAP)
- AdMob
- Addressable Asset System
- C#, ScriptableObject, PlayerPrefs 등

---

## 🚫 포함되지 않은 항목

> 민감한 정보 보호를 위해 일부 파일은 제외되어 있습니다.

- `google-services.json`
- `play-billing-service.json` (서비스 계정 키)
- `.keystore`, `.jks` 등 Android 서명 키
- Firebase Functions 내 `.env`, `node_modules/` 등

이 프로젝트를 실행하려면, 본인 Firebase 및 GCP 설정에 맞는 구성 파일이 필요합니다.

---

## 📝 실행 방법

1. Unity 2022.X 버전으로 프로젝트 열기
2. Firebase Unity SDK 설치 (Analytics, Auth, Database, Functions 등)
3. 필요한 `.json` 파일 및 SDK 키는 로컬에서 수동 설정
4. Addressable 빌드 및 리소스 할당 후 실행

---

## 📂 프로젝트 구조 요약

```plaintext
Assets/
├── Scripts/                  # 게임 로직 및 시스템
├── Firebase/                # (제외됨)
├── SPUM/                    # 캐릭터 리소스
├── GooglePlayGames/         # (제외됨)
├── AddressableAssetsData/   # Addressables 설정
├── StreamingAssets/         # 환경 설정 / 광고 관련 리소스
