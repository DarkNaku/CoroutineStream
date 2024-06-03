# CoroutineStream

## 소개
코루틴들(Coroutines)의 순차 또는 병렬 실행을 도와주는 라이브러리

## 설치방법
1. 패키지 관리자의 툴바에서 좌측 상단에 플러스 메뉴를 클릭합니다.
2. 추가 메뉴에서 Add package from git URL을 선택하면 텍스트 상자와 Add 버튼이 나타납니다.
3. https://github.com/DarkNaku/CoroutineStream.git 입력하고 Add를 클릭합니다.

## 사용방법

```csharp
    ...
        CSPlayer.CoroutineStream()
            .Append(CoroutineA())
            .Callback(() => { Debug.Log(string.Format("{0} : Hello World !", Time.time)); })
            .Append(CoroutineB(), CoroutineC())
            .Interval(2f)
            .Append(CoroutineD(), CoroutineE(), CoroutineF())
            .Until(() => _flag)
            .While(() => Input.GetKey(KeyCode.Z))
            .OnComplete(() => { Debug.Log(string.Format("{0} : Completed !!!", Time.time)); });
    ...
```

## 클래스 

### class CSPlayer

- 게임 오브젝트를 갖고 있는 싱글턴 객체로 코루틴을 실행하는 역활하는 클래스.



#### 함수

##### public static CoroutineStream();

- 작업 목록을 등록하고 실행할 객체를 얻어옵니다.

```csharp
  var cs = CSPlayer.CoroutineStream();
```

### class CoroutineStream

- 작업 목록을 관리하는 역활을 하는 클래스.

#### 속성

##### public bool Stoped;

* 작업이 중지 완료 되지 못하고 도중에 중지 된 경우 참이 됩니다.

##### public bool Paused;

* 작업이 일시 정지된 경우 참이 됩니다.

##### public bool Completed;

* 작업 목록이 모두 완료된 경우 참이 됩니다.

#### 함수

##### public CoroutineStream Pause();

- 진행 중인 작업을 멈춥니다.

```csharp
    CSPlayer.CoroutineStream().Pause();
```

##### public CoroutineStream Resume();

- 작업을 다시 계속 진행합니다.

```csharp
    CSPlayer.CoroutineStream().Pause();
```

##### public CoroutineStream Stop();

- 작업을 중지합니다.

```csharp
    CSPlayer.CoroutineStream().Stop();
```

##### public CoroutineStream Append(params IEnumerator[] coroutine);

- 동시에 실행할 한개 이상의 코루틴을 추가합니다.

```csharp
  CSPlayer.CoroutineStream().Append(CoroutineA());
    
  CSPlayer.CoroutineStream().Append(CoroutineA(), CoroutineB());
```

##### public CoroutineStream Interval(float seconds);

- 지연시간을 추가합니다.

```csharp
  CSPlayer.CoroutineStream().Interval(10f);
```

##### public CoroutineStream Until(System.Func\<bool> predicate);

- 조건식이 참이 될 때까지 대기합니다.

```csharp
  CSPlayer.CoroutineStream().Until(() => Input.GetKeyDown(KeyCode.Space));
```

##### public CoroutineStream While(System.Func\<bool> predicate);

- 조건식이 참인 동안 대기합니다.

```csharp
  CSPlayer.CoroutineStream().While(() => Input.GetKey(KeyCode.Space));
```

##### public CoroutineStream Callback(System.Action callback);

- 콜백함수를 추가합니다.

```csharp
  CSPlayer.CoroutineStream().Callback(() => Debug.Log("Hello World !!!));
```

##### public CoroutineStream OnComplete(System.Action callback);

- 등록된 모든 작업이 완료된 후에 호출될 함수를 등록합니다. 여러번 등록하는 경우 마지막에 등록한 함수만 호출 됩니다.

```csharp
  CSPlayer.CoroutineStream().OnComplete(() => Debug.Log("Completed !!!));
```