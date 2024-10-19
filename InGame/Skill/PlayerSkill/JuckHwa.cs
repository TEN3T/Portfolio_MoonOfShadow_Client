using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuckHwa : ActiveSkill
{
    private List<Projectile> projectiles = new List<Projectile>();

    public JuckHwa(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }

    public override IEnumerator Activation()
    {
        //float ignitionTime = 0.1f;
        //float activateTime = 0.0f;
        //while (activateTime < skillData.duration)
        //{
        //    if (ignitionTime <= 0.0f)
        //    {
        //        Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
        //        projectile.transform.position = shooter.position;
        //        projectiles.Add(projectile);
        //        Debug.Log(projectiles.Count);
        //        ignitionTime = 0.2f;
        //    }
        //    else
        //    {
        //        ignitionTime -= Time.fixedDeltaTime;
        //    }

        //    activateTime += Time.fixedDeltaTime;
        //    yield return frame;
        //}
        //SkillManager.Instance.CoroutineStarter(Extinguish());
        //while (projectiles.Count > 0)
        //{
        //    yield return frame;
        //}
        float activeTime = skillData.duration;
        do
        {
            activeTime -= Time.fixedDeltaTime;
            if (((int)(activeTime * 1000)) % 13 == 0)
            {
                Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
                Vector3 pos = shooter.position;
                pos.y -= 0.5f;
                projectile.transform.position = pos;
                SkillManager.Instance.CoroutineStarter(Ignition(projectile));
            }
            yield return frame;
        } while (activeTime > 0);
    }

    private IEnumerator Ignition(Projectile projectile)
    {
        ParticleSystem.MainModule main = projectile.GetComponentInChildren<ParticleSystem>().main;
        main.startLifetime = skillData.duration;
        yield return duration;
        SkillManager.Instance.DeSpawnProjectile(projectile);
    }

    private IEnumerator Extinguish()
    {
        float extinguishTime = 1f;
        while (projectiles.Count > 0)
        {
            if (extinguishTime <= 0.0f)
            {
                if (projectiles.Count > 0)
                {
                    SkillManager.Instance.DeSpawnProjectile(projectiles[0]);
                    projectiles.RemoveAt(0);
                }
            }
            else
            {
                if (projectiles.Count > 0)
                {
                    extinguishTime -= Time.fixedDeltaTime;
                }
            }
            if(projectiles.Count<=0)
            {
                yield break;
            }
            yield return frame;
        }
    }
}

