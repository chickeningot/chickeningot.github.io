---
layout: post
title: 유니티 멀티플레이어 템플릿
tags:
  - unity
  - csharp
---

## [multiplayer-unity](https://github.com/chickeningot/multiplayer-unity)

요즘 만들고 있는 유니티 멀티플레이어 템플릿 프로젝트이다.  
네트워킹 라이브러리를 만드는 김에 시작하게 된 프로젝트인데 기능은 다음과 같다.
- 네트워킹 서비스
- 스팀
- 개발자 콘솔
- 빌드 매니저

유니티 에셋스토어에 무료 에셋으로 올리고 싶은 마음이 있지만  
여기에 사용한 라이브러리를 재배포해도 될지 확신이 없어서 망설이고 있다.  

### 지원하는 유니티 버전
~2018.2.21 (2018.3 이전까지) 

## 네트워킹 서비스
![](/images/2019-04-03-multiplayer-unity/networking.PNG)

서버 열기, 서버 접속, RPC를 이용한 통신을 지원한다.  
리플렉션으로 RPC를 구현했기 때문에 멋있게 **RMP(Reflection Message Protocol)**라는 이름도 붙여줬다. 나중엔 멤버필드에도 접근할 수 있는 기능도 만들 생각이다. 

### 예제 코드
```csharp
public void SendChat(string msg)
{
  // 서버에게 채팅 메시지를 보낸다
  _view.RPC(RPCOption.ToServer, "svRPC_Chat", msg);
}

[RMP]
[ServerOnly]
private void svRPC_Chat(string msg)
{
  // 서버는 받은 채팅 메시지를 출력하고 모든 클라이언트에게 전송한다
  clRPC_Chat(msg);
  _view.RPC(RPCOption.Broadcast, "clRPC_Chat", msg);
}

[RMP]
[ClientOnly]
private void clRPC_Chat(string msg)
{
  // 서버로부터 받은 채팅 메시지를 출력한다
  Debug.Log(msg);
}
```

## 스팀
![](/images/2019-04-03-multiplayer-unity/steam1.PNG)
![](/images/2019-04-03-multiplayer-unity/steam2.PNG)

모든 스팀 기능들은 [Facepunch.Steamworks](https://github.com/Facepunch/Facepunch.Steamworks)를 통해서 사용할 수 있다.  
유니티에서 스팀 연동하는 법을 구글링하면 가장 먼저 나오는 게 [Steamworks.Net](https://steamworks.github.io/)인데  
Facepuch 것이 훨씬 좋다. 게리모드와 러스트 개발자인 Garry가 만들었다.  

이 템플릿은 스팀 클라이언트와 서버를 자동으로 관리해 준다.  
위의 네트워킹 서비스를 이용해 서버를 열거나 닫을 경우 스팀서버도 함께 열고 닫힌다.

## 개발자 콘솔
![](/images/2019-04-03-multiplayer-unity/console.PNG)

static 메소드에 어트리뷰트를 붙여놓으면 자동으로 명령어가 등록된다.  
명령어의 이름을 정하는 방법은 두 가지가 있다.
1. 어트리뷰트에 직접 전달
2. 예를들어, 메소드의 이름을 'CommandPrint' 라고 지으면 'print'라는 명령어가 등록됨

### 예제 코드
```csharp
[ConsoleCommand(Help = "Output message")]
static void CommandPrint(CommandArg[] args)
{
  // Print input message on terminal
}
```