using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GwiGi : ActiveSkill
{
    public GwiGi(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }

    public override IEnumerator Activation()
    {
        PROJECTILE_DIRECTION projectileDirection;

        Vector2 direction = (Scanner.GetTarget(skillData.skillTarget, shooter, skillData.attackDistance) - (Vector2)shooter.position).normalized;
        if (direction.x < 0)
        {
            projectileDirection = PROJECTILE_DIRECTION.LEFT;
        }
        else
        {
            projectileDirection = PROJECTILE_DIRECTION.RIGHT;
        }

        for (int i = 0; i < skillData.projectileCount; i++)
        {
            Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData, shooter, scaleType: SCALE_TYPE.HORIZON);
            projectile.SetDirection(projectileDirection);
            if (projectileDirection == PROJECTILE_DIRECTION.LEFT)
            {
                projectile.transform.localPosition = new Vector2(-projectile.transform.localScale.x * 2, projectile.transform.localScale.y);
                projectileDirection = PROJECTILE_DIRECTION.RIGHT;
            }
            else
            {
                projectile.transform.localPosition = new Vector2(projectile.transform.localScale.x * 2, projectile.transform.localScale.y);
                projectileDirection = PROJECTILE_DIRECTION.LEFT;
            }
            SkillManager.Instance.CoroutineStarter(Move(projectile));
            yield return intervalTime;
        }
    }

    private IEnumerator Move(Projectile projectile)
    {
        
        

        yield return duration;
        SkillManager.Instance.DeSpawnProjectile(projectile);
    }

}
