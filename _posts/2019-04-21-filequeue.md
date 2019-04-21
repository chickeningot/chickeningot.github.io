---
layout: post
title: C# 파일 비동기 입출력 알아서 해주는 코드
tags:
  - csharp
---

## <a href="assets/FileQueue.cs" download>FileQueue.cs</a>

같은 디렉토리를 하나의 스레드에서는 접근하려 하고, 다른 스레드에서는 제거하려고 한다면 문제가 발생할 것이다. 이러한 상황을 방지하고 싶을때, 즉 큐잉을 하고싶을 때 쓸 수 있는 클래스이다.  

처음 이걸 작성할 당시엔 async/await 를 몰라서 원래는 비동기 처리를 편하게 하려고 만들었던 것이었다. 그냥 비동기 입출력을 하고싶으면 이것보다는 [System.IO 의 다양한 Async 메소드](https://docs.microsoft.com/en-us/dotnet/standard/io/asynchronous-file-i-o)를 활용하는 게 더 낫다.