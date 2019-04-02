---
layout: post
title: 유니티 멀티플레이어 템플릿
tags:
  - Unity
---

## [multiplayer-unity](https://github.com/chickeningot/multiplayer=unity)

게임을 만들고 싶지만 구체적인 아이디어는 없어서 언젠가는 쓰겠지 하는 생각으로 만들게 된 프로젝트이다.  
유니티 에셋스토어에 무료 에셋으로 올리고 싶은 마음이 있지만  
스팀 기능을 만들면서 사용한 Facepunch.Steamwork를 재배포해도 될지 확신이 없어서 망설이고 있다.  

MIT 라이센스이고 'Do whatever you want'라고 쿨하게 써놓기도 했지만  
직접 물어보고 답변을 얻기 전까진 역시 찝찝하다. 

### 기능
- 네트워킹 서비스
- 스팀 연동
- 개발자 콘솔
- 빌드 매니저

### 지원하는 유니티 버전
~2018.2.21 (2018.3 이전까지) 

### 네트워킹 서비스 ChickenIngot.Networking
![](/images/2019-04-03-multiplayer-unity/networking.PNG)
서버 열기, 서버 접속, RPC를 이용한 통신을 지원한다.  
#### 예제 코드
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

### 스팀 연동 ChickenIngot.Steam
![](/images/2019-04-03-multiplayer-unity/steam1.PNG)
![](/images/2019-04-03-multiplayer-unity/steam2.PNG)
모든 스팀 기능들은 Facepunch.Steamwork를 통해서 사용할 수 있다.  
이 템플릿은 스팀 클라이언트와 서버를 자동으로 관리해 준다.
위의 네트워킹 서비스를 이용해 서버를 열거나 닫을 경우 스팀서버도 함께 열고 닫힌다.

### 개발자 콘솔
![](/images/2019-04-03-multiplayer-unity/console.PNG)
#### 예제 코드
```csharp
[ConsoleCommand(Help = "Output message")]
static void CommandPrint(CommandArg[] args)
{
  // Print input message on terminal
}
```