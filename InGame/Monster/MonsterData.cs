using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    NORMAL,
    ELITE,
    BOSS,
}

public enum AttackType
{
    Shy,
    Bold,
    FRIENDLY,
}

[System.Serializable]
public class MonsterData
{
    [field: SerializeField] public string monsterName { get; private set; }
    [field: SerializeField] public float sizeMultiple { get; private set; }
    [field: SerializeField] public int hp { get; private set; }
    [field: SerializeField] public int currentHp { get; private set; }
    [field: SerializeField] public float attack { get; private set; }
    [field: SerializeField] public float moveSpeed { get; private set; }
    [field: SerializeField] public float atkSpeed { get; private set; }
    [field: SerializeField] public float viewDistance { get; private set; }
    [field: SerializeField] public float atkDistance { get; private set; }
    [field: SerializeField] public int skillID { get; private set; }
    [field: SerializeField] public string groupSource { get; private set; }
    [field: SerializeField] public int groupSourceRate { get; private set; }
    [field: SerializeField] public string monsterPrefabPath { get; private set; }
    [field: SerializeField] public AttackType attackType { get; private set; }
    [field: SerializeField] public MonsterType monsterType { get; private set; }

    public void SetMonsterName(string monsterName) { this.monsterName = monsterName; }
    public void SetSizeMultiple(float sizeMultiple) { this.sizeMultiple = sizeMultiple; }
    public void SetHp(int hp) { this.hp = hp; }
    public void SetCurrentHp(int currentHp) { this.currentHp = currentHp; }
    public void SetAttack(float attack) { this.attack = attack; }
    public void SetMoveSpeed(float moveSpeed) { this.moveSpeed = moveSpeed; }
    public void SetAtkSpeed(float atkSpeed) { this.atkSpeed = atkSpeed; }
    public void SetViewDistance(float viewDistance) { this.viewDistance = viewDistance; }
    public void SetAtkDistance(float atkDistance) { this.atkDistance = atkDistance; }
    public void SetSkillID(int skillID) { this.skillID = skillID; }
    public void SetGroupSource(string groupSource) { this.groupSource = groupSource; }
    public void SetGroupSourceRate(int groupSourceRate) { this.groupSourceRate = groupSourceRate; }
    public void SetMonsterPrefabPath(string monsterPrefabPath) { this.monsterPrefabPath = monsterPrefabPath; }
    public void SetAttackType(AttackType attackType) { this.attackType = attackType; }
    public void SetMonsterType(MonsterType monsterType) { this.monsterType = monsterType; }
}