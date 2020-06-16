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

순서대로 정리해보면, 소켓을 생성해준 뒤 -> Bind -> Listen -> BiginAccept의 과정으로 시작한다. 이 다음부턴 연결 요청이 들어온 후 이다.