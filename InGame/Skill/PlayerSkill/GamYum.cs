using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamYum : ActiveSkill
{
    public GamYum(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum)
    {
    }

    public override IEnumerator Activation()
    {
        for (int i = 0; i < skillData.projectileCount; i++)
        {
            ProjectileStraight projectile = SkillManager.Instance.SpawnProjectile<ProjectileStraight>(skillData);
            projectile.transform.localPosition = shooter.position;
            Vector2 pos = Scanner.GetTarget(skillData.skillTarget, shooter, skillData.attackDistance);
            pos -= (Vector2)shooter.position;
            projectile.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - 90.0f);
            yield return intervalTime;
        }
    }
}
