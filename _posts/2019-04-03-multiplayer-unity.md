---
layout: post
title: 유니티 멀티플레이어 템플릿
tags:
  - Unity
---

## [multiplayer-unity](https://github.com/chickeningot/multiplayer=unity)

스타터킷이라고 하면 뭔가 고급스럽고 돈받고 파는 것 같은 이미지가 느껴져서  
그냥 이런건 템플릿이라고 부르기로 했다.  

- 네트워킹 서비스
- 스팀 연동
- 개발자 콘솔
- 빌드 매니저

게임을 만들고 싶지만 구체적인 아이디어는 없어서  
언젠가는 쓰겠지 하는 생각으로 만들게 된 프로젝트이다.  
유니티 에셋스토어에 무료 에셋으로 올리고 싶은 마음이 있지만  
스팀 기능을 만들면서 사용한 Facepunch.Steamwork를 재배포해도 될지 확신이 없어서 망설이고 있다.  

MIT 라이센스이고 'Do whatever you want'라고 쿨하게 써놓기도 했지만  
직접 물어보고 답변을 얻기 전까진 역시 찝찝하다.  

### 네트워킹 서비스 ChickenIngot.Networking
#### 채팅 예제 코드
```csharp
using UnityEngine;

namespace ChickenIngot.Networking.Demo
{
	/// -----------------------------------------------------------------------------------
	/// [RCP Message]
	///		스크립트가 RPC 메시지와 기타 이벤트를 받으려면
	///		RMPNetworkView 의 'Message Receivers' 에 등록되어 있어야 한다.
	/// -----------------------------------------------------------------------------------

	public class DemoChatting : MonoBehaviour
	{
		[SerializeField]
		private RMPNetworkView _view;

		public void SendChat(string msg)
		{
			_view.RPC(RPCOption.ToServer, "svRPC_Chat", msg);
		}

		// 서버가 ToServer 옵션을 사용하면 자신의 메소드가 호출된다.
		// 덕분에 코드를 일관적으로 작성할 수 있다.
		// SendChat 메소드를 보면 굳이 자신이 서버인지 클라이언트인지 확인하지 않았다.
		[RMP]
		[ServerOnly]
		private void svRPC_Chat(string msg)
		{
			// 메소드가 어느 클라이언트에 의해 호출되었는지 확인하려면 RMPNetworkView.MessageSender 를 사용한다.
			// 서버 자신이 호출한 경우에는 null 이다.
			if (_view.MessageSender == null)
				msg = "Server : " + msg;
			else
				msg = "Client : " + msg;

			clRPC_Chat(msg);
			_view.RPC(RPCOption.Broadcast, "clRPC_Chat", msg);
		}

		// 사실 어트리뷰트들은 아무런 기능도 하지 않는다. 하지만 가독성을 위해 붙이는 것이 좋다.
		[RMP]
		[ClientOnly]
		private void clRPC_Chat(string msg)
		{
			Debug.Log(msg);
		}
	}
}
```

#### 데모 씬
![]("/images/2019-04-03-multiplayer-unity/networking.PNG")

### 스팀 연동 ChickenIngot.Steam

#### 아바타 불러오기 예제 코드
```csharp
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChickenIngot.Steam.Demo
{
	/// -----------------------------------------------------------------------------------
	/// [스팀 기능 사용 예 : Avatar 불러오기]
	///		Facepunch.Steamworks 는 다양한 스팀 기능을 지원한다.
	///		이 스크립트에는 스팀 프로필 이미지를 불러오는 예제가 작성되어 있다.
	/// -----------------------------------------------------------------------------------

	public class DemoAvatar : MonoBehaviour
	{
		[SerializeField]
		private RawImage _ui;
		[SerializeField]
		private Facepunch.Steamworks.Friends.AvatarSize _size;

		IEnumerator Start()
		{
			while (Steam.Client == null)
				yield return null;

			var me = Steam.Me;
			Steam.Client.Friends.GetAvatar(_size, me.SteamId, (image) => OnImage(image));
		}

		private void OnImage(Facepunch.Steamworks.Image image)
		{
			if (image == null)
			{
				Debug.LogWarning("Failed to get avatar.");
				return;
			}

			var texture = new Texture2D(image.Width, image.Height);

			for (int x = 0; x < image.Width; x++)
				for (int y = 0; y < image.Height; y++)
				{
					var p = image.GetPixel(x, y);

					texture.SetPixel(x, image.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
				}

			texture.Apply();

			ApplyTexture(texture);
		}

		private void ApplyTexture(Texture texture)
		{
			if (_ui != null)
				_ui.texture = texture;
		}
	}
}
```

#### 데모 씬
![]("/images/2019-04-03-multiplayer-unity/steam1.PNG")

![]("/images/2019-04-03-multiplayer-unity/steam2.PNG")

### 개발자 콘솔
#### 명령어 등록 예제
```csharp
[ConsoleCommand(Help = "Output message")]
static void CommandPrint(CommandArg[] args)
{
  // Print input string on display.
}
```

![]("/images/2019-04-03-multiplayer-unity/console.PNG")