using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyeongGyae : ActiveSkill
{
    public MyeongGyae(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }

    public override IEnumerator Activation()
    {
        for (int i = 0; i < skillData.projectileCount; i++)
        {
            Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
            yield return intervalTime;
            SkillManager.Instance.DeSpawnProjectile(projectile);
        }
    }
}
