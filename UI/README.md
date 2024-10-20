# InGame
```
※ 목차의 하이퍼 링크를 누를 시 해당 항목에 대한 부분으로 이동됩니다.

※ .cs 확장자의 하이퍼 링크를 누를 시 해당 스크립트로 이동합니다.

※ 스크립트 세부 사항은 별도의 pdf 파일에 서술되어 있습니다.
```
## 목차
* [InGameUI](#ingameui)
* [OutGameUI](#outgameui)


## InGameUI
### [UIPoolManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/UIPoolManager.cs)
* UI 요소에 대한 ObjectPool을 관리하는 스크립트입니다.
* 몬스터가 피격 시 HpBar가 보이게 구현했기에 오브젝트의 생성 및 파괴에 관한 cost를 줄이는 용도로 ObjectPool을 적용했습니다.

### [PlayerUI.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/PlayerUI.cs)
* 플레이어 캐릭터와 관련된 UI들을 전반적으로 관리하는 스크립트입니다.

### [SkillUI.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/SkillUI.cs)  [SkillBoxUI.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/SkillBoxUI.cs)  [SkillBoxIcon.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/SkillBoxIcon.cs)
* 획득한 스킬의 비주얼 요소를 관리하는 스크립트입니다.
* 각 스킬의 쿨타임, 획득한 스킬들 등을 표현합니다.

### [HpBar.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/HpBar.cs)
* 플레이어 체력의 비주얼 요소를 관리하는 스크립트입니다.

### [ExpBar.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/ExpBar.cs)
* 경험치량에 따라 제어되는 경험치 바를 제어하는 스크립트입니다.

### [LevelUpUI.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/LevelUpUI.cs)
* 플레이어 캐릭터가 레벨업을 할 경우 스킬 획득을 제어하는 스크립트입니다.

### [PlayerStatusUI.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/PlayerStatusUI.cs)
* 플레이어 캐릭터의 데이터와 관련된 비주얼 요소를 관리하는 스크립트입니다.

### [Timer.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/Timer.cs)
* 현재 스테이지의 진행 시간을 계산하는 스크립트입니다.
* MonsterSpawner를 제어하는 데 사용합니다.

### [GameOverUI.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/InGameUI/GameOverUI.cs)
* 스테이지를 클리어하거나 실패할 경우 표시되는 비주얼 요소를 다룬 스크립트입니다.

## OutGameUI
### [UI_Hon.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/OutGameUI/UI_Hon.cs)
* 대분류 혼을 제어하기 위한 UI 스크립트입니다.
* 해금 여부에 대한 데이터를 서버에서 받아옵니다.

### [UI_Hon_Under](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/UI/OutGameUI/UI_Hon_Under.cs)
* 하위 혼을 제어하기 위한 UI 스크립트입니다.
* 해금 여부 및 선택된 혼에 대한 서버와의 데이터 송수신을 제어합니다.
