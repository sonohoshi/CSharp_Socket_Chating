## 본 markdown 파일은 제가 메모해두고 공부하기 위한 파일입니다.  

### Scoket.Bind(EndPoint);

```csharp
public void Bind (System.Net.EndPoint localEP);
```  

Socket을 로컬 엔드포인트와 연결합니다. - MSDN  
특정 로컬 EndPoint를 이용해야하는 경우 본 메소드를 이용한다.  
여기서 EndPoint란 네트워크 주소를 식별하기위해 만든 class라고 하고, abstract 클래스라고 한다. 지식의 출처는 MSDN이다.    
EndPoint 클래스는 원격 호스트에 연결할 때도 이용한다고 한다. 

어쨌거나 여기서 중요한건 Bind 메소드를 이용할 때엔 로컬 EndPoint를 넘겨줘야 한다는 것이다. 할당된 로컬 주소를 고려하지 않는 경우, IPAddress.Any를 주소 매개변수로 넘겨 IPEndPoint를 만들 수 있고, 기본 서비스 공급자가 가장 적합한 네트워크 주소를 할당한다... 고 MSDN에 적혀있다. 내가 직접 IP 주소를 적어넣지 않아도 적당한 주소를 할당해주는 방법이 있다는 이야기일 것이다.  

```csharp
try {
    aSocket.Bind(anEndPoint);
}
catch (Exception e) {
    Console.WriteLine("Winsock error: " + e.ToString());
}
```  

위와같은 방식으로 Bind함수를 이용한다. Bind함수가 throw 하는 Exception은 네가지가 있는데, 순서대로 다음과 같다.  
```csharp
ArgumentNullException e;
// 로컬 EndPoint가 null일 때 발생한다.
SocketException e;
// Socket에 액세스하는 동안 오류가 발생하면 뱉는다.
ObjectDisposedException e;
// Socket이 닫히면 발생한다.
SecurityException e;
// 호출 스택의 상위 호출자에게 요청된 작업을 수행할 권한이 없는 경우 발생한다.
```  

위에서 EndPoint 클래스에 대해 설명했는데, 직접 EndPoint 클래스를 상속받은 IPEndPoint 객체를 만들어 넘겨줄 때엔 이렇게 이용한다.  
```cs
Socket.Bind(new IPEndPoint(IPAddress.Any, /* 사용할 포트 번호 */ 1000));
```  

IPAddress.Any 혹은 사용할 IP Address와 사용할 Port Number를 생성자의 인자로 넘겨주면 된다. But, 보통 여러번 쓰는건 번거롭기 때문에 IPEndPoint 객체를 하나 만들어두는 것이 나을 것 같다고 생각한다.

---

Bind 메소드를 성공적으로 호출했다면 Listen 메소드를 호출한다고 합니다.  

```csharp
public void Listen (int backlog);
```  

위와 같은 형태를 지니고 있습니다. 본 메소드를 실행하면 해당 소켓을 수신 대기 상태로 만들어줍니다. 매개변수로는 대기 큐의 크기를 지정해주는데, 예를 들어 5를 넘겨주면 한번에 5개의 명령을 실행할 수 있고 그 다음부터는 큐에 쌓인다고 볼 수 있겠습니다.  

```csharp
// create the socket
Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

// bind the listening socket to the port
IPAddress hostIP = (Dns.Resolve(IPAddress.Any.ToString())).AddressList[0];
IPEndPoint ep = new IPEndPoint(hostIP, port);
listenSocket.Bind(ep);

// start listening
listenSocket.Listen(backlog);
```  

위는 MSDN에 나와있는 활용 예제이다. 위에서 이야기한 것처럼, Bind 메소드를 실행시킨 후 Listen 메소드를 부르는 것을 볼 수 있다.  
대강의 과정을 보면 Bind로 IP주소에 접속하고, Listen을 통해 수신 대기 상태로 만든다고 이해하면 될 것 같다.  

---

우리는 이제 비동기 작업을 수행할 때가 됐다. BiginAccept 콜백 메소드를 통해 작업을 시작할 것이다.  

```csharp
public IAsyncResult BeginAccept (System.Net.Sockets.Socket acceptSocket, int receiveSize, AsyncCallback callback, object state);
```  

위 코드가 바로 BeginAccept 메소드의 원형이다. 중간에 들어가있는 AsyncCallback은 대리자의 형태를 가진 델리게이트이다.  
이는 다음과 같은 형태를 가지고있다.  

```csharp
public delegate void AsyncCallback(IAsyncResult ar);
```  

내용이 조금 어렵다. 비동기식 스레드를 하나 직접 생성해서 쓴다는 듯 하다. 매개변수로 들어가는 IAsuncResult는 인터페이스로, 비동기 작업의 상태를 나타낸다고 한다. 스레딩을 한다는 의미가 이제 이해되네요.  

아무튼 BeginAccept 메소드를 통해 해당 콜백 메소드를 넘겨주고, 연결 요청을 처리하는 메소드를 AsyncCallback의 형태로 넘겨주면 되는 것이다. 해당 콜백 함수를 우리가 작성해야하는지는 아직 공부를 하지 않았다. 근데 지금 공부해보니까 우리가 작성해야한다. 아이고야.  

순서대로 정리해보면, 소켓을 생성해준 뒤 -> Bind -> Listen -> BiginAccept의 과정으로 시작한다. 이 다음부턴 연결 요청이 들어온 후 이다. 여기까지가 시작 부분이라고 보며 될 것 같다.

---  

이제부터는 데이터를 수신할 차례이다. EndAccept와 BeginReceive 메소드를 이용할 것이다.  
EndAccept 메소드는 다음과 같은 형태를 가지고있다.  

```csharp
public System.Net.Sockets.Socket EndAccept (out byte[] buffer, out int bytesTransferred, IAsyncResult asyncResult);
```  

연결 요청을 스레딩을 통해 비동기로 처리하고, 통신을 처리할 새로운 객체를 반환한다. 우리가 쓸 때는 IAsyncResult 객체만 넘겨줄거다. 왜냐? 그걸 설명하는건 그다지 sexy하지 않군요.  

```csharp
private void someAsyncAcceptHandlerProc(IAsyncResult ar) {
    Socket sockClient = sockServer.EndAccept(ar);
}
```  

다음과 같은 형태로 이용한다고 한다. 클라이언트 소켓 객체를 처리할 때 쓰면 되는 것 같다. 연결 요청을 '받아서' 처리하는 애니까, 당연히 연결요청이 들어온 후 실행되야겠죠. 그러니까 말하자면, 위에서 이야기했던 AsyncCallback 에서 실행시키면 된다 대충 이렇게 이해하면 될 것 같아요.  

이번에 알아볼 것은 BeginReceive 메소드이다. BeginAccept처럼 비동기식 스레드를 하나 파서 소켓에서 받고있는 데이터를 받아온다. 매개변수가 좀 많다. 메소드의 원형을 보며 알아보도록 하자.  

```csharp
public IAsyncResult BeginReceive (
    byte[] buffer, 
    int offset, 
    int size, 
    System.Net.Sockets.SocketFlags socketFlags, 
    AsyncCallback callback, 
    object state);
```  

매개변수가 너무 많아 엔터로 구분을 좀 했다.  

1. 바이트 배열
2. 배이트 배열의 n번째 인덱스부터 받을것인지
3. 수신받을 자료의 크기
4. 소켓 옵션
5. 수신하면 호출되는 비동기 대리자, 즉 콜백 델리게이트
6. 추가적으로 필요한 데이터를 받을 객체  

이와 같이 매개변수를 넣어주면 된다. 적당한 클래스를 하나 만들어주도록 하자. 클래스를 넘겨주고나면 5번의 콜백 메소드에서 접근할 수 있을 것이다.  

```csharp
public class AsyncObject{
    public byte[] buffer;
    public Socket workingSocket;
    public AsyncObject(int size){
        buffer = new byte[size];
    }
}
```  

1번째 매개변수인 바이트 배열에 해당 클래스 객체를 만들고 객체에 있는 바이트 배열을 넘겨줄 생각이다.  
덕분에 BeginReceive 메소드를 적절하게 써서 코드를 짜줬다.  
이제, EndReceive. 데이터를 수신받을 차례다. 이 역시 '비동기적' 으로 처리한다.

```csharp
public int EndReceive (IAsyncResult asyncResult);
```

위와 같은 형태를 가지고 있다. 역시 매개변수로 콜백 메소드를 넘겨준다. 반환값이 int인 이유? 수신받은 자료의 바이트 수를 반환한다. 즉, 반환값이 0 이상이면 데이터가 있다는 뜻이겠지.  

우리는 BeginReceive에 넘겨준 콜백 메소드에서 해당 메소드, 즉 EndReceive를 호출할 예정이다. int값으로 받고나서 값이 0 이상이라면 데이터를 잘 처리해주면 되고, 그런 후에 다시 BeginReceive를 통해 데이터를 받기 시작하면 될것이다.

---

서버쪽 작업은 끝났다. 나는 이제부터 클라이언트 작업을 할 것이다.  

| 서버 | 클라이언트 |
| :---: | :---: |
| 1. Bind -> Listen -> BeginAccept | 2. Connect or BeginConnect|
| 3. Receive Request -> EndAccept -> BeginReceive | 4. Success Connect -> BeginReceive or Send/BeginSend|
| 5. Data is not null -> EndReceive -> BeginReceive| 5. Data is not null -> EndReceive -> BeginReceive|

위 표의 순서대로 서버와 클라이언트간의 통신이 진행된다.  

Connect부터 시작해보자.  
```csharp
public void Connect (string host, int port);
```

메소드의 원형이시다. host 매개변수는 말 그대로, 서버 호스트의 이름을 넣어야 한다. 오버로딩된 형태가 많아서 우리가 지금까지 알고있던 EndPoint를 매개변수로 넘겨주거나, IP adress를 넘겨주는 방법도 있다. 나는 EndPoint를 사용할 예정이다. string을 어떤 형태로 넘기는지 아직 잘 이해가 안돼서.