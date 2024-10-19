using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JuHyung : ActiveSkill
{
    private LineRenderer effect;

    public JuHyung(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }

    public override IEnumerator Activation()
    {
        Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
        effect = projectile.GetComponent<LineRenderer>();
        effect.enabled = false;
        effect.positionCount = 0;
        projectile.transform.position = shooter.position;
        SkillManager.Instance.CoroutineStarter(Chaining(projectile));
        yield return intervalTime;
    }

    //ProjectileCount: 전이 개수
    private IEnumerator Chaining(Projectile projectile)
    {
        List<Vector3> effectPos = new List<Vector3>()
        {
            shooter.position,
        };

        //첫 전이 대상 몬스터
        Transform firstTarget = Scanner.GetTargetTransform(skillData.skillTarget, shooter, skillData.attackDistance);
        if (firstTarget == null)
        {
            SkillManager.Instance.DeSpawnProjectile(projectile);
            yield break;
        }
        effectPos.Add(firstTarget.position);

        List<Transform> aeTarget = Scanner.RangeTarget(firstTarget, skillData.splashRange, (int)LayerConstant.MONSTER);
        if (aeTarget.Count < 1)
        {
            yield break;
        }

        Dictionary<Transform, float> map = new Dictionary<Transform, float>();      //key: 몬스터, value: 거리
        foreach (Transform target in aeTarget)
        {
            map.Add(target, Vector2.Distance(firstTarget.position, target.position));
        }
        aeTarget = map.OrderBy(entry => entry.Value)
                      .Select(entry => entry.Key)
                      .Take(skillData.projectileCount)
                      .ToList();

        int count = aeTarget.Count < skillData.projectileCount ? aeTarget.Count : skillData.projectileCount;
        for (int i = 0; i < count; i++)
        {
            if (aeTarget[i].TryGetComponent(out Monster monster))
            {
                effectPos.Add(monster.transform.position);
                monster.Hit(projectile.CalTotalDamage(monster));
            }
        }

        effect.positionCount = effectPos.Count;
        effect.SetPositions(effectPos.ToArray());
        effect.enabled = true;

        yield return new WaitForSeconds(1.0f);
        SkillManager.Instance.DeSpawnProjectile(projectile);
    }

    //private IEnumerator Move(Projectile projectile, Transform target, float speed = 15.0f)
    //{
    //    projectile.GetComponent<TrailRenderer>().enabled = true;
    //    Vector2 direction = (target.position - projectile.transform.position).normalized;
    //    float distance = 0.0f;
    //    while (Vector2.Distance(projectile.transform.position, target.position) > 0.2f)
    //    {
    //        float shift = Time.fixedDeltaTime * skillData.speed * speed;
    //        distance += shift;
    //        if (distance >= skillData.attackDistance * 1.1f || !target.gameObject.activeInHierarchy)
    //        {
    //            break;
    //        }
    //        projectile.transform.Translate(direction * shift);
    //        yield return frame;
    //    }

    //    SkillManager.Instance.DeSpawnProjectile(projectile);
    //}

}
