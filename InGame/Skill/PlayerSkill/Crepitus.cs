using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crepitus : ActiveSkill
{
    //private float size;

    public Crepitus(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }

    public override IEnumerator Activation()
    {
        //size = skillData.attackDistance >= 10 ? 10 : skillData.attackDistance;

        for (int i = 0; i < skillData.projectileCount; i++)
        {
            Vector2 pos = Scanner.GetTarget(skillData.skillTarget, shooter, skillData.attackDistance);
            if (pos == Vector2.one)
            {
                continue;
            }
            if (CameraManager.Instance.IsTargetVisible(pos))
            {
                Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
                projectile.transform.localScale = Vector3.one * skillData.splashRange;
                projectile.transform.position = pos;
                SkillManager.Instance.CoroutineStarter(Boom(projectile));
            }
            
            yield return intervalTime;
        }
    }

    private IEnumerator Boom(Projectile projectile)
    {
        yield return new WaitForSeconds(3.0f / projectile.particleSpeed);
        foreach(Transform t in Scanner.RangeTarget(projectile.transform, skillData.splashRange, (int)LayerConstant.MONSTER))
        {
            if (t.TryGetComponent(out Monster monster))
            {
                monster.Hit(GameManager.Instance.player.playerManager.TotalDamage(skillData.damage));
            }
        }
        SkillManager.Instance.DeSpawnProjectile(projectile);
    }
}
