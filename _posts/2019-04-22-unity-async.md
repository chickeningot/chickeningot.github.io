---
layout: post
title: 유니티에서 async, await 사용하기
tags:
  - unity
  - csharp
---

유니티는 C# 6.0을 지원한다. 2018 버전부터는 7.0도 지원한다고 들었다. async, await 키워드는 C# 5.0부터 추가된 기능이므로 유니티에서 사용 가능하다. 이 사실을 얼마 전에 깨달았다.  

굉장히 오버헤드가 큰 작업을 해야하는데 실시간성을 확보하고 싶을때가 있다. 맵을 로딩하는 동안 로딩 아이콘이 끊김 없이 뱅글뱅글 돌게 하고 싶은 경우가 좋은 예시. 보통 이럴 땐 일회용 스레드를 생성하고 그 안에서 작업을 수행한다. 이런 작업을 위해서 있는 편의 기능이 Task인데 aysnc와 await는 이 Task와 함께 사용하는 키워드이다.  

async, await의 사용법을 모를 때는 (다시 말하면 이 글을 쓰기 직전까지 ㅋㅋ..) 유니티에서 일회용 스레드를 만들고 싶으면 아래와 같이 쓰곤 했었다.

```csharp
using UnityEngine;
using System.Collections;
using System.Threading;

public class AsyncTest_Coroutine : MonoBehaviour
{
	void Start()
	{
		StartCoroutine(Run(10));
	}

	IEnumerator Run(int count)
	{
		int result = 0;
		bool isDone = false;

		// 람다식을 사용하면 변수 스코프를 공유할 수 있는 장점이 있다.
		// 스레드 내에서 result와 isDone 변수에 접근할 수 있다.
		(new Thread(() =>
		{
			for (int i = 0; i < count; ++i)
			{
				Debug.Log(i);
				result += i;
				Thread.Sleep(1000);
			}

			// 작업이 끝났음을 알린다.
			isDone = true;
		}))
		.Start();

		// isDone == true 가 될 때까지 대기한다.
		while (!isDone) yield return null;

		Debug.Log("Result : " + result);
	}
}
```

프로그램을 block 시키지 않고 10초간 비동기 작업이 실행되는 코드이다.  

이렇게 스레드 객체를 만들고 람다식으로 함수를 정의하면 나름 보기 좋은 코드가 나온다. 새로운 스레드를 팠지만 그냥 위에서 아래로 순차적으로 읽으면 동작을 이해할 수 있어서 개인적으로 굉장히 좋은 코드라고 생각한다. **하지만 매번 코루틴을 사용해야 하는 단점이 있고 그마저도 유니티이기 때문에 가능한 것이었다.**

그런데 async, await 키워드를 이용하면 더 간단하고 유니티 코루틴의 도움 없이도 동일한 동작을 하는 코드를 작성할 수 있다. 다음은 async, await를 사용하는 예제이다.

```csharp
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class AsyncTest : MonoBehaviour
{
	void Start()
	{
		Debug.Log("Run() invoked in Start()");
		Run(10);
		Debug.Log("Run() returns");
	}

	void Update()
	{
		Debug.Log("Update()");
	}

	async void Run(int count)
	{
		// 새로 만들어진 태스크 스레드에서 CountAsync()를 실행한다.
		var task = Task.Run(() => CountAsync(10));

		// 함수를 리턴하고 태스크가 종료될 때까지 기다린다.
		// 따라서 바로 "Run() returns" 로그가 출력된다.
		// 태스크가 끝나면 result 에는 CountAsync() 함수의 리턴값이 저장된다.
		int result = await task;

		// 태스크가 끝나면 await 바로 다음 줄로 돌아와서 나머지가 실행되고 함수가 종료된다.
		Debug.Log("Result : " + result);
	}

	int CountAsync(int count)
	{
		int result = 0;

		for (int i = 0; i < count; ++i)
		{
			Debug.Log(i);
			result += i;
			Thread.Sleep(1000);
		}

		return result;
	}
}
```

await 키워드가 들어있는 메소드는 반드시 async 키워드를 붙여야 한다. Task 객체 앞에 await를 붙이면 해당 태스크가 종료될 때까지 기다리게 된다. await를 만나면 즉시 함수를 리턴하고 해당 스레드는 다음 작업을 계속 수행할 수 있다. 태스크가 종료되면 다시 await가 있던 곳으로 돌아와 나머지가 실행된다. **물론 함수가 호출됐던 동일한 스레드에서 실행된다.**

이제 람다식을 사용해서 첫 번째 예제처럼 수정하면 다음과 같다.

```csharp
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class AsyncTest_Lambda : MonoBehaviour
{
	void Start()
	{
		Debug.Log("Run() invoked in Start()");
		Run(10);
		Debug.Log("Run() returns");
	}

	void Update()
	{
		Debug.Log("Update()");
	}

	async void Run(int count)
	{
		int result = 0;

		await Task.Run(() =>
		{
			for (int i = 0; i < count; ++i)
			{
				Debug.Log(i);
				result += i;
				Thread.Sleep(1000);
			}
		});

		Debug.Log("Result : " + result);
	}
}
```

코드가 훨씬 간단해졌다. 코루틴을 쓸 필요도 없고 무엇보다 더 멋있다. ㅎ

## [unity-example-async](https://github.com/chickeningot/unity-example-async)
이거는 위의 예제들을 다 모아놓은 프로젝트.