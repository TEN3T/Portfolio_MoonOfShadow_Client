using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuckJung : ActiveSkill
{
    public JuckJung(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum)
    {
    }

    public override IEnumerator Activation()
    {
        GameManager.Instance.skillTrigger = false;
        GameManager.Instance.playerTrigger = false;
        PassiveEffect.PassiveEffectActivation(float.Parse(skillData.skillEffectParam[0]), SKILLCONSTANT.SKILL_PASSIVE.ARMOR, SKILLCONSTANT.CALC_MODE.PLUS);
        
        float totalDamage = GameManager.Instance.player.playerManager.playerData.currentHp;
        SkillManager.Instance.CoroutineStarter(GameManager.Instance.player.Invincible(duration));

        yield return duration;

        GameManager.Instance.skillTrigger = true;
        GameManager.Instance.playerTrigger = true;
        PassiveEffect.PassiveEffectActivation(-float.Parse(skillData.skillEffectParam[0]), SKILLCONSTANT.SKILL_PASSIVE.ARMOR, SKILLCONSTANT.CALC_MODE.PLUS);

        totalDamage = 2 * Mathf.Abs(totalDamage - GameManager.Instance.player.playerManager.playerData.currentHp);
        foreach (Monster monster in MonsterSpawner.Instance.monsters)
        {
            if (CameraManager.Instance.IsTargetVisible(monster.transform.position))
            {
                monster.Hit(totalDamage);
            }
        }
    }
}
