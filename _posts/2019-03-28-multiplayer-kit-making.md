---
layout: post
title: 유니티 멀티플레이어 템플릿 개발중
tags:
  - Unity
---

## [multiplayer-unity](https://github.com/chickeningot/multiplayer=unity)

요즘 만들고 있는 유니티 멀티플레이어 템플릿 프로젝트이다.  
네트워킹 라이브러리를 만드는 김에 시작하게 된 프로젝트인데 목표는 다음과 같다.  

- 네트워킹 서비스 (완료)
- 스팀 연동 (완료)
- 서버 & 클라이언트 자동 빌드

완성하면 통째로 에셋 스토어에 올리고 싶지만  
네트워킹 서비스를 제외하면 나머지는 오픈소스를 가져다가 쓰기 때문에  
네트워킹 서비스만 나중에 분리해서 올릴 생각이다. 한글 주석도 다 영어로 고치고..

### 네트워킹 서비스 ChickenIngot.Networking
유니티 네트워킹 LLAPI 를 사용하여 만들었다. LLAPI는 2018.3 부터 obsolute 되어서  
2018.2.21 버전까지만 사용 가능하다.  
리플렉션으로 RPC를 구현했기 때문에 멋있게 **RMP(Reflection Message Protocol)**라는 이름도 붙여줬다. 나중엔 리플렉션으로 멤버필드에도 접근할 수 있는 기능도 만들 생각이다.  

### 스팀 연동 ChickenIngot.Steam
[Facepunch.Steamworks](https://github.com/Facepunch/Facepunch.Steamworks)를 사용했다.  
유니티에서 스팀 연동하는 법을 구글링하면 가장 먼저 나오는 게 [Steamworks.Net](https://steamworks.github.io/)인데  
Facepuch 것이 훨씬 좋다. 게리모드와 러스트 개발자인 Garry가 만들었다.  

### 서버 & 클라이언트 자동 빌드
유니티 프로젝트 하나에서 서버와 클라를 모두 빌드할 수 있는 시스템을 만들 예정이다.    
서버는 batch 옵션으로 콘솔창만 띠우는 식이고  
#if 전처리기로 서버와 클라이언트의 코드를 분리할 수 있다.  
그리고 개발자 콘솔같은 유틸리티도 포함시킬 생각이다.
