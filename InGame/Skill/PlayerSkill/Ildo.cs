using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Ildo : ActiveSkill
{
    public Ildo(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }
    public override IEnumerator Activation()
    {
        for (int i = 0; i < skillData.projectileCount; i++)
        {
            Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
            projectile.transform.position = shooter.position;
            projectile.CollisionPower(false);
            SkillManager.Instance.CoroutineStarter(Effect(projectile));
            yield return intervalTime;
        }

    }

    private IEnumerator Effect(Projectile projectile)
    {
        ParticleSystem[] effects = projectile.GetComponentsInChildren<ParticleSystem>();
        effects[0].Play();

        Vector2 targetPos = Scanner.GetTarget(skillData.skillTarget, shooter, skillData.attackDistance);
        Vector2 direction = (targetPos - (Vector2)shooter.position).normalized;
        Vector3 speed = direction * skillData.speed * Time.fixedDeltaTime;

        projectile.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

        float diff = 0.0f;
        float distance = Vector2.Distance(projectile.transform.position, targetPos);

        do
        {
            projectile.transform.position = Vector3.SmoothDamp(projectile.transform.position, targetPos, ref speed, 0.25f);
            yield return frame;
            diff = Vector2.Distance(projectile.transform.position, targetPos);
            if (diff < 0.75f)
            {
                projectile.SetAlpha(diff / distance);
            }
        } while (diff > 0.25f);

        effects[0].Clear();
        projectile.SetAlpha(1.0f);
        projectile.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
        projectile.transform.position = targetPos + new Vector2(0.0f, 1.5f);
        projectile.CollisionPower(true);

        effects[1].Play();
        do
        {
            projectile.transform.position = Vector3.Lerp(projectile.transform.position, targetPos, 15.0f * Time.fixedDeltaTime);
            diff = Vector2.Distance(projectile.transform.position, targetPos);
            yield return frame;
        } while (diff > 0.5f);

        yield return new WaitForSeconds(0.5f);
        SkillManager.Instance.DeSpawnProjectile(projectile);
    }
}
