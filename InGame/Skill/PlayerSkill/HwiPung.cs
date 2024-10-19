using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HwiPung : ActiveSkill
{
    private const float ROTATE_SPEED = 90.0f;

    public HwiPung(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum)
    {
    }

    //탕탕 부메랑
    public override IEnumerator Activation()
    {
        for (int i = 0; i < skillData.projectileCount; i++)
        {
            Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
            projectile.transform.position = shooter.position;
            SkillManager.Instance.StartCoroutine(Move(projectile));
            yield return intervalTime;
        }
    }

    private IEnumerator Move(Projectile projectile)
    {
        Vector2 direction = (Scanner.GetTarget(skillData.skillTarget, shooter, skillData.attackDistance) - (Vector2)shooter.position).normalized;

        float travel = skillData.attackDistance * 4.0f;
        float coefficient = skillData.attackDistance;
        float weight = 0.0f;
        do
        {
            float distance = Time.fixedDeltaTime * skillData.speed * coefficient / skillData.attackDistance;
            projectile.transform.GetChild(0).Rotate(Vector3.forward * Time.deltaTime * skillData.speed * ROTATE_SPEED);
            projectile.transform.Translate(direction * distance);
            travel -= distance;
            weight -= Time.fixedDeltaTime / skillData.attackDistance;
            coefficient += weight;
            yield return frame;
        } while (travel > 0.0f);

        //direction *= -1.0f;

        //travel = skillData.attackDistance * 3.0f;
        //do
        //{
        //    float distance = Time.fixedDeltaTime * skillData.speed;
        //    projectile.transform.Translate(direction * distance);
        //    travel -= distance;
        //    yield return frame;
        //} while (travel > 0.0f);

        SkillManager.Instance.DeSpawnProjectile(projectile);
    }
}
