using SKILLCONSTANT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSkillData
{
    public int skillId { get; private set; }
    public string name { get; private set; }
    public int coolTime { get; private set; }
    public string aniName { get; private set; }
    public float attackDistance { get; private set; }
    public bool isEffect { get; private set; }
    public int projectileCount { get; private set; }
    public int intervalTime { get; private set; }
    public int duration { get; private set; }
    public int damage { get; private set; }
    public float speed { get; private set; }
    public bool isPenetrate { get; private set; }
    public float splashRange { get; private set; }
    public float projectileSize { get; private set; }
    public List<SKILL_EFFECT> skillEffects { get; private set; }
    public List<float> skillEffectParams { get; private set; }
    public string prefabPath { get; private set; }

    public void SetSkillId(int skillId) { this.skillId = skillId; }
    public void SetName(string name) { this.name = name; }
    public void SetCoolTime(int coolTime) {  this.coolTime = coolTime; }
    public void SetAniName(string aniName) {  this.aniName = aniName; }
    public void SetAttackDistance(float attackDistance) {  this.attackDistance = attackDistance; }
    public void SetIsEffect(bool isEffect) {  this.isEffect = isEffect; }
    public void SetProjectileCount(int projectileCount) {  this.projectileCount = projectileCount; }
    public void SetIntervalTime(int intervalTime) {  this.intervalTime = intervalTime; }
    public void SetDuration(int duration) { this.duration = duration; }
    public void SetDamage(int damage) { this.damage = damage; }
    public void SetSpeed(float speed) { this.speed = speed; }
    public void SetIsPenetrate(bool isPenetrate) { this.isPenetrate = isPenetrate; }
    public void SetSplashRange(float splashRange) {  this.splashRange = splashRange; }
    public void SetProjectileSize(float projectileSize) { this.projectileSize =  projectileSize; }
    public void SetSkillEffects(List<SKILL_EFFECT> skillEffects) { this.skillEffects = skillEffects; }
    public void SetSkillEffectParams(List<float> skillEffectParams) { this.skillEffectParams = skillEffectParams; }
    public void SetPrefabPath(string prefabPath) {  this.prefabPath = prefabPath; }
}
