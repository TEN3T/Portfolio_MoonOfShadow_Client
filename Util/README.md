# Util
```
※ 목차의 하이퍼 링크를 누를 시 해당 항목에 대한 부분으로 이동됩니다.

※ .cs 확장자의 하이퍼 링크를 누를 시 해당 스크립트로 이동합니다.

※ 스크립트 세부 사항은 별도의 pdf 파일에 서술되어 있습니다.
```
## Util
서버와의 통신, 데이터 로드, 게임 서브 로직, 디자인 패턴, 오브젝트 풀
로컬라이즈, 디버깅을 담당하는 스크립트들입니다.

### [APIAddressManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/APIAddressManager.cs)  [APIManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/APIManager.cs)  [WebRequestManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/WebRequestManager.cs)
* 서버와 통신하기 위한 스크립트들입니다.
* 인게임과 관련된 코드를 작업했습니다.

### [CSVReader.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/CSVReader.cs)
* CSV 파일로 게임 데이터를 제어하기 때문에 이를 위한 CSV 파일을 읽는 스크립트입니다.

### [ResourcesManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/ResourcesManager.cs)
* 게임 성능 향상을 위해 인게임에서 불러온 파일들을 캐싱하는 스크립트입니다.

### [DebugManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/DebugManager.cs)
* 디버깅 메시지를 일괄적으로 제어하기 위한 스크립트입니다.

### [LocalizeManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/LocalizeManager.cs)
* 3가지 언어를 사용하기 때문에 이를 위한 로컬라이징 스크립트입니다.

### [GameAlgorithm.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/GameAlgorithm.cs)
* 자료구조를 다룰 때 필요한 수학적 알고리즘을 여러 스크립트에서 손쉽게 사용하기 위해 구현한 스크립트입니다.\
* Extension Method 방식으로 구현했습니다.

### [ObjectPool.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/ObjectPool.cs)
* 오브젝트의 생성과 파괴에 대한 cost를 감소시켜 게임 성능을 높이기 위한 스크립트입니다.
* ObjectPool 디자인 패턴을 사용했습니다.

### [SingleTon.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/SingleTon.cs)  [SingleTonBehaviour.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/Util/SingleTonBehaviour.cs)
* Scene이 바뀌는 경우 파괴되지 말아야 할 오브젝트를 위한 스크립트입니다.
* SingleTon 디자인 패턴을 사용했습니다.
* Mono를 상속받는지 여부에 따라 여러 버전으로 구현했습니다.
