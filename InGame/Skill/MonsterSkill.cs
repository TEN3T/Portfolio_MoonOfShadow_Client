using SKILLCONSTANT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterSkill : Skill
{
    protected MonsterSkillData skillData;
    protected WaitForSeconds coolTime;
    protected WaitForSeconds intervalTime;
    protected WaitForSeconds duration;
    protected WaitForFixedUpdate frame;

    public abstract IEnumerator Activation();

    public MonsterSkill(int skillId, Transform shooter)
    {
        skillData = new MonsterSkillData();
        frame = new WaitForFixedUpdate();
    }

    public IEnumerator SkillActivation()
    {
        throw new System.NotImplementedException();
    }

    public void SkillLevelUp()
    {
        SetSkillData(skillData.skillId + 1);
    }

    public void SetSkillData(int skillId)
    {
        Dictionary<string, object> table = CSVReader.Read("MonsterSkillTable")[skillId.ToString()];

        skillData.SetSkillId(skillId);
        skillData.SetName(table["Name"].ToString());

        if (int.TryParse(table["Cooltime"].ToString(), out int ct))
        {
            skillData.SetCoolTime(ct);
            coolTime = new WaitForSeconds(skillData.coolTime);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 CoolTime 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        skillData.SetAniName(table["Ani_name_01"].ToString());

        if (float.TryParse(table["AttackDist"].ToString(), out float attackDist))
        {
            skillData.SetAttackDistance(attackDist);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 AttackDistance 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }
        
        if (bool.TryParse(table["IsEffect"].ToString().ToLower(), out bool ie))
        {
            skillData.SetIsEffect(ie);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 IsEffect 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }
        
        if (int.TryParse(table["ProjectileCount"].ToString(), out int pc))
        {
            skillData.SetProjectileCount(pc);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 ProjectileCount 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        if (int.TryParse(table["IntervalTime"].ToString(), out int it))
        {
            skillData.SetIntervalTime(it);
            intervalTime = new WaitForSeconds(skillData.intervalTime);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 IntervalTime 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        if (int.TryParse(table["Duration"].ToString(), out int d))
        {
            skillData.SetDuration(d);
            duration = new WaitForSeconds(skillData.duration);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 Duration 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        if (int.TryParse(table["Damage"].ToString(), out int damage))
        {
            skillData.SetDamage(damage);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 Damage 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        if (float.TryParse(table["Speed"].ToString(), out float speed))
        {
            skillData.SetSpeed(speed);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 Speed 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        if (bool.TryParse(table["IsPenetrate"].ToString().ToLower(), out bool ip))
        {
            skillData.SetIsPenetrate(ip);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 IsPenetrate 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        if (float.TryParse(table["SplashRange"].ToString(), out float sr))
        {
            skillData.SetSplashRange(sr);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 SplashRange 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        if (float.TryParse(table["ProjectileSize"].ToString(), out float ps))
        {
            skillData.SetProjectileSize(ps);
        }
        else
        {
            DebugManager.Instance.PrintError("[Error: MonsterSkill] 테이블의 ProjectileSize 부분을 점검해 주세요. (SkillID: {0})", skillId);
        }

        try
        {
            List<SKILL_EFFECT> list2 = new List<SKILL_EFFECT>();
            foreach (string str in (table["SkillEffect"] as List<string>))
            {
                list2.Add((SKILL_EFFECT)Enum.Parse(typeof(SKILL_EFFECT), str, true));
            }
            skillData.SetSkillEffects(list2);
        }
        catch
        {
            try
            {
                List<SKILL_EFFECT> list2 = new List<SKILL_EFFECT>()
            {
                (SKILL_EFFECT)Enum.Parse(typeof(SKILL_EFFECT), Convert.ToString(table["SkillEffect"]), true),
            };
                skillData.SetSkillEffects(list2);
            }
            catch
            {
                skillData.SetSkillEffects(new List<SKILL_EFFECT>());
            }
        }

        List<string> list = table["SkillEffectParam"] as List<string>;
        List<float> effectParams = new List<float>();
        if (list == null)
        {
            effectParams.Add(float.Parse(table["SkillEffectParam"].ToString()));
            skillData.SetSkillEffectParams(effectParams);
        }
        else
        {
            foreach(string str in list)
            {
                effectParams.Add(float.Parse(str));
            }
            skillData.SetSkillEffectParams(effectParams);
        }

        skillData.SetPrefabPath(table["SkillPrefabPath"].ToString());
    }
}
