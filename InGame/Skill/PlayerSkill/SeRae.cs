using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeRae : ActiveSkill
{
    private Vector2 speed = Vector2.down;
    private float angleSpeed = 1.0f;

    public SeRae(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum)
    {
    }

    public override IEnumerator Activation()
    {
        for (int i = 0; i < skillData.projectileCount; i++)
        {
            Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
            projectile.transform.localEulerAngles = Vector3.zero;
            SkillManager.Instance.CoroutineStarter(Effect(projectile));
            yield return intervalTime;
        }
    }

    private IEnumerator Effect(Projectile projectile)
    {
        projectile.CollisionPower(false);
        Vector2 pos = Scanner.GetTarget(SKILLCONSTANT.SKILL_TARGET.RANDOM, shooter, skillData.attackDistance);
        pos += UnityEngine.Random.insideUnitCircle * 2.0f;
        projectile.transform.position = pos + Vector2.up * 10.0f;

        do
        {
            projectile.transform.position = Vector2.SmoothDamp(projectile.transform.position, pos, ref speed, 0.25f);
            yield return frame;
        } while (Vector2.Distance(projectile.transform.position, pos) > 0.25f);

        speed = Vector3.up;
        pos = (Vector2)projectile.transform.position + Vector2.up;
        do
        {
            projectile.transform.position = Vector2.SmoothDamp(projectile.transform.position, pos, ref speed, 0.2f);
            projectile.transform.localEulerAngles = Vector3.forward * Mathf.SmoothDampAngle(projectile.transform.localEulerAngles.z, -45.0f, ref angleSpeed, 0.2f);
            yield return frame;
        } while (Vector2.Distance(projectile.transform.position, pos) > 0.01f);

        projectile.CollisionPower(true);
        yield return intervalTime;
        SkillManager.Instance.DeSpawnProjectile(projectile);
    }
}
