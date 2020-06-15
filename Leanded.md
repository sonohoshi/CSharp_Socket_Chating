## 본 markdown 파일은 제가 메모해두고 공부하기 위한 파일입니다.  

### Scoket.Bind(EndPoint);

```cs
public void Bind (System.Net.EndPoint localEP);
```  

Socket을 로컬 엔드포인트와 연결합니다. - MSDN  
특정 로컬 EndPoint를 이용해야하는 경우 본 메소드를 이용한다.  
여기서 EndPoint란 네트워크 주소를 식별하기위해 만든 class라고 하고, abstract 클래스라고 한다. 지식의 출처는 MSDN이다.    
EndPoint 클래스는 원격 호스트에 연결할 때도 이용한다고 한다. 

어쨌거나 여기서 중요한건 Bind 메소드를 이용할 때엔 로컬 EndPoint를 넘겨줘야 한다는 것이다. 할당된 로컬 주소를 고려하지 않는 경우, IPAddress.Any를 주소 매개변수로 넘겨 IPEndPoint를 만들 수 있고, 기본 서비스 공급자가 가장 적합한 네트워크 주소를 할당한다... 고 MSDN에 적혀있다. 내가 직접 IP 주소를 적어넣지 않아도 적당한 주소를 할당해주는 방법이 있다는 이야기일 것이다.  

```cs
try {
    aSocket.Bind(anEndPoint);
}
catch (Exception e) {
    Console.WriteLine("Winsock error: " + e.ToString());
}
```  

위와같은 방식으로 Bind함수를 이용한다. Bind함수가 throw 하는 Exception은 네가지가 있는데, 순서대로 다음과 같다.  
```cs
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

IPAddress.Any 혹은 사용할 IP Address와 사용할 Port Number를 생성자의 인자로 넘겨주면 된다. But, 보통 여러번 번거롭기 때문에 IPEndPoint 객체를 하나 만들어두는 것이 나을 것 같다고 생각한다.

---

