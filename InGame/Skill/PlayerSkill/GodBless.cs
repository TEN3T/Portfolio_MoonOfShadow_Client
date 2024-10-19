using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GodBless : ActiveSkill
{
    public GodBless(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }

    public override IEnumerator Activation()
    {
        Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
        projectile.transform.position = shooter.position;
        projectile.transform.localScale = Vector3.one * 10.0f;

        yield return new WaitForSeconds(2.5f);

        List<Transform> targets = Scanner.RangeTarget(shooter, skillData.splashRange, (int)LayerConstant.MONSTER);
        DebugManager.Instance.PrintError(targets.Count);
        foreach (Transform target in targets)
        {
            if (target.TryGetComponent(out Monster monster))
            {
                monster.Hit(GameManager.Instance.player.playerManager.TotalDamage(skillData.damage));
            }
        }
        yield return new WaitForSeconds(1.5f);

        SkillManager.Instance.DeSpawnProjectile(projectile);
    }
}
