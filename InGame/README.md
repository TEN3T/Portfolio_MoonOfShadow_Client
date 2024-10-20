# InGame
```
※ 목차의 하이퍼 링크를 누를 시 해당 항목에 대한 부분으로 이동됩니다.

※ .cs 확장자의 하이퍼 링크를 누를 시 해당 스크립트로 이동합니다.

※ 스크립트 세부 사항은 별도의 pdf 파일에 서술되어 있습니다.
```
## 목차
* [Camera](#camera)

* [FieldStructure](#fieldStructure)

* [Item](#item)

* [Monster](#monster)

* [Player](#player)

* [Skill](#skill)

* [GameManager](#gamemanager)

* [LayerConstant](#layerconstant)


## Camera
### [CameraManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Camera/CameraManager.cs)
* 플레이어를 비추는 카메라를 제어하는 스크립트입니다.
* Cinemachine 사용

### [SpawnMobLocation.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Camera/SpawnMobLocation.cs)
* 몬스터의 스폰 위치에 대한 Enum

## FieldStructure
### [FieldStructure.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/FieldStructure/FieldStructure.cs)  [FieldStructureData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/FieldStructure/FieldStructureData.cs)
* 필드에 생성되는 오브젝트들의 기본 기능 및 데이터 스크립트입니다.
* 이 스크립트를 상속받아 특정 오브젝트의 기능을 추가 구현합니다.

### Other Script
* 나머지 스크립트들은 FieldStructure.cs를 상속해 만든 특정 FieldStructure에 대한 스크립트입니다.

## Item
### [Item.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Item/Item.cs)  [ItemData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Item/ItemData.cs)
* 아이템의 기본 기능 및 데이터 스크립트입니다.

### [ItemManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Item/ItemManager.cs)
* 아이템 드롭 확률을 제어하는 스크립트입니다.
* 드롭되는 아이템을 그룹별로 묶어 두 개의 테이블을 만든 후 조인하여 각 아이템별로 특정 확률을 제어할 수 있게 설정했습니다.

### [ItemConstant.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Item/ItemConstant.cs)
* 아이템 타입에 대한 Enum입니다.

### [ItemEffect.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Item/ItemEffect.cs)
* 아이템 타입별로 기능을 구현한 스크립트입니다.

### Other Script
* 나머지 스크립트들은 Item.cs를 상속해 만든 특정 아이템에 대한 스크립트입니다.

## Monster
### [Monster.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Monster/Monster.cs)  [MonsterData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Monster/MonsterData.cs)
* 몬스터의 기본 기능 및 데이터 스크립트입니다.

### [MonsterCollider.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Monster/MonsterCollider.cs)
* 몬스터의 충돌 문제 해결 및 공격 범위를 제어하는 스크립트입니다.
* 충돌과 공격의 콜라이더를 분리하여 설계했습니다.

### [MonsterSpawner.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Monster/MonsterSpawner.cs)  [MonsterSpawnData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Monster/MonsterSpawnData.cs)
* 몬스터 소환 제어 및 데이터 스크립트입니다.

## Player
### [Player.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Player/Player.cs)  [PlayerData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Player/PlayerData.cs)
* 플레이어 캐릭터의 기본 기능 및 데이터 스크립트입니다.

### [PlayerItem.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Player/PlayerItem.cs)
* 아이템 획득용 콜라이더를 제어하는 스크립트입니다.

### [PlayerManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Player/PlayerManager.cs)
* 플레이어 캐릭터의 전반적인 데이터들을 관리하는 스크립트입니다.
* 공격, 피격, 충돌 등을 제어합니다.

## Skill
### [Skill.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/Skill.cs)
* 액티브/패시브 스킬이 공통적으로 가지는 속성을 정의한 인터페이스입니다.

### [ActiveSkill.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/ActiveSkill.cs)  [ActiveData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/ActiveData.cs)
* 액티브 스킬 관련 스크립트입니다.
* 쿨타임마다 자동으로 발동되는 시스템입니다.

### [PassiveSkill.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/PassiveSkill.cs)  [PassiveData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/PassiveData.cs)  [PassiveEffect.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/PassiveEffect.cs)  [PassiveSet.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/PassiveSet.cs)
* 패시브 스킬 관련 스크립트입니다.

### [MonsterSkill.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/MonsterSkill.cs)  [MonsterSkillData.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/MonsterSkillData.cs)
* 몬스터 스킬 관련 스크립트입니다.
* 현재 미구현 상태이며 추후 업데이트 예정입니다.

### [Projectile.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/Projectile.cs)  [ProjectileStraight.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/ProjectileStraight.cs)
* 액티브 스킬 발동 시 발사되는 투사체에 관한 스크립트입니다.
* 투사체의 로직에 따라 움직임을 직접 구현하는 방식과 단순 발사되는 방식 두 가지를 사용했습니다.

### [SkillManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/SkillManager.cs)
* 스킬 획득 및 발동 등 스킬과 전반적으로 관련된 데이터들을 제어하는 스크립트입니다.

### [StatusEffect.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/StatusEffect.cs)
* 상태이상을 제어하는 스크립트입니다.
* 비트 연산을 이용해 구현했습니다.

### [Scanner.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/Scanner.cs)
* 스킬이 발동될 때 명중해야하는 몬스터들이나 좌표를 서칭하는 스크립트입니다.

### [ParticleControll.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/ParticleControll.cs)
* 스킬의 비주얼 요소를 제어하는 스크립트입니다.

### [SkillConstant.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/Skill/SkillConstant.cs)
* 스킬과 관련된 Enum Set입니다.

## Other
### [GameManager.cs](https://github.com/TEN3T/Portfolio_MoonOfShadow_Client/blob/main/InGame/GameManager.cs)
* 게임 플레이 전체를 제어하는 스크립트입니다.
* 게임의 시작, 종료, 일시정지 및 각종 키 입력을 담당합니다.
* 맵 로드, 플레이어 캐릭터 배치, Layer 값 제어를 합니다.
* InGame UI와 상호작용합니다.

### [LayerConstant.cs]()
* 충돌 및 Layer 순서로 인한 비주얼 요소 순서를 제어하기 위해 사용하는 Enum입니다.

