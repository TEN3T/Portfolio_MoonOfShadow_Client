using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Possession : ActiveSkill
{
    private GameObject effect;

    public Possession(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }

    public override IEnumerator Activation()
    {
        effect = ResourcesManager.Load<GameObject>("Prefabs/InGame/Skill/Possession_Effect");
        if (effect.transform.TryGetComponent(out Renderer renderer))
        {
            renderer.sortingLayerName = LayerConstant.SPAWNOBJECT.ToString();
        }
        yield return Effect();
        Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData, shooter);
        SkillManager.Instance.DeSpawnProjectile(projectile);
    }

    private IEnumerator Effect()
    {
        GameObject projectile = Object.Instantiate(effect, shooter);
        if (projectile.TryGetComponent(out Animator animator))
        {
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }
        
        Object.Destroy(projectile);
    }
}
