using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangAe : PassiveSkill
{
    private float speed = 20.0f;

    public ChangAe(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum)
    {
    }

    public override IEnumerator Activation()
    {
        for (int i = 0; i < skillData.skillEffect.Count; i++)
        {
            PassiveEffect.PassiveEffectActivation(skillData.skillEffectParam[i], skillData.skillEffect[i], skillData.calcMode[i]);
        }
        yield return Move();
    }

    private IEnumerator Move()
    {
        projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData, shooter);
        SortingLayer(projectile.transform);

        while (true)
        {
            projectile.transform.Rotate(Vector3.up * Time.fixedDeltaTime * speed);
            yield return frame;
        }
    }

    private void SortingLayer(Transform trans)
    {
        foreach (Transform t in trans)
        {
            if (t.TryGetComponent(out Renderer renderer))
            {
                renderer.sortingLayerName = LayerConstant.SPAWNOBJECT.ToString();
            }
        }
    }
}
