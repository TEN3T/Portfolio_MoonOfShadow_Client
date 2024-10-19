using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyungSang : PassiveSkill
{
    public MyungSang(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum)
    {
    }

    public override IEnumerator Activation()
    {
        GameManager.Instance.playerTrigger = false;
        yield return new WaitForSeconds(skillData.skillEffectParam[0] / 1000.0f);
        GameManager.Instance.playerTrigger = true;

        for (int i = 1; i < skillData.skillEffect.Count; i++)
        {
            PassiveEffect.PassiveEffectActivation(skillData.skillEffectParam[i], skillData.skillEffect[i], skillData.calcMode[i]);
        }
        yield return new WaitForFixedUpdate();
    }
}
