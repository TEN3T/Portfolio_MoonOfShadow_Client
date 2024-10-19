using BFM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SCALE_TYPE
{
    NONE,
    VERTICAL,
    HORIZON,
}

public class SkillManager : SingletonBehaviour<SkillManager>
{
    //public static readonly int SKILL_MAX_LEVEL = 8;
    public static readonly int ACTIVE_SKILL_MAX_COUNT = 6;
    public static readonly int PASSIVE_SKILL_MAX_COUNT = 5;
    public static readonly int SKILL_MAX_COUNT = ACTIVE_SKILL_MAX_COUNT + PASSIVE_SKILL_MAX_COUNT;

    [SerializeField] private int skillNum = 10801;

    private Dictionary<string, Dictionary<string, object>> skillTable;
    private Dictionary<string, Dictionary<string, object>> passiveTable;
    private Dictionary<int, ObjectPool<Projectile>> skillPools;
    private Dictionary<int, Vector3> projectileOriginalSize;
    private ObjectPool<SkillRangeCircle> rangeCirclePool;
    private Dictionary<int, IEnumerator> skillCoroutineList;

    //public Dictionary<int, SkillInfo> skillList { get; private set; } = new Dictionary<int, SkillInfo>();
    public Dictionary<int, Skill> skillList { get; private set; } = new Dictionary<int, Skill>();

    protected override void Awake()
    {
        skillTable = CSVReader.Read("SkillTable");
        passiveTable = CSVReader.Read("PassiveTable");
        skillPools = new Dictionary<int, ObjectPool<Projectile>>();
        projectileOriginalSize = new Dictionary<int, Vector3>();
        skillCoroutineList = new Dictionary<int, IEnumerator>();

        foreach (string skillId in skillTable.Keys)
        {
            if (int.TryParse(skillId, out int id))
            {
                id /= 100;
                if (!skillPools.ContainsKey(id))
                {
                    string prefabPath = skillTable[skillId]["SkillPrefabPath"].ToString();
                    skillPools.Add(id, new ObjectPool<Projectile>(ResourcesManager.Load<Projectile>(prefabPath), transform));
                }
            }
        }

        rangeCirclePool = new ObjectPool<SkillRangeCircle>(ResourcesManager.Load<SkillRangeCircle>("Prefabs/InGame/Skill/SkillRangeCircle"), transform);
    }

    #region Spawn Projectile
    //public SkillRangeCircle SpawnRangeCircle(float duration, Transform parent)
    //{
    //    return SpawnRangeCircle(duration, 1.0f, parent);
    //}

    //public SkillRangeCircle SpawnRangeCircle(float duration, float size, Transform parent)
    //{
    //    SkillRangeCircle circle = rangeCirclePool.GetObject();
    //    circle.transform.SetParent(parent);
    //    circle.Activation(duration, size);
    //    circle.transform.localPosition = Vector3.zero;
    //    circle.gameObject.SetActive(true);
    //    return circle;
    //}

    public void DeSpawnRangeCircle(SkillRangeCircle circle)
    {
        rangeCirclePool.ReleaseObject(circle);
        circle.transform.SetParent(transform);
    }

    public T SpawnProjectile<T>(ActiveData skillData) where T : Projectile
    {
        return SpawnProjectile<T>(skillData, transform);
    }

    public T SpawnProjectile<T>(ActiveData skillData, LayerConstant layer) where T : Projectile
    {
        return SpawnProjectile<T>(skillData, transform, layer);
    }

    public T SpawnProjectile<T>(ActiveData skillData, Transform shooter, LayerConstant layer = LayerConstant.SKILL, SCALE_TYPE scaleType = SCALE_TYPE.NONE) where T : Projectile
    {
        int poolId = skillData.skillId / 100;
        T projectile = (T)skillPools[poolId].GetObject();
        if (!projectileOriginalSize.ContainsKey(poolId))
        {
            projectileOriginalSize.Add(poolId, projectile.transform.localScale);
        }
        projectile.transform.parent = shooter;
        projectile.gameObject.layer = (int)layer;
        projectile.transform.localPosition = Vector2.zero;
        if (scaleType == SCALE_TYPE.NONE)
        {
            projectile.transform.localScale = projectileOriginalSize[poolId] * skillData.projectileSizeMulti;
        }
        else if (scaleType == SCALE_TYPE.HORIZON)
        {
            projectile.transform.localScale = new Vector2(projectileOriginalSize[poolId].x * skillData.projectileSizeMulti, projectileOriginalSize[poolId].y);
        }
        else if (scaleType == SCALE_TYPE.VERTICAL)
        {
            projectile.transform.localScale = new Vector2(projectileOriginalSize[poolId].x, projectileOriginalSize[poolId].y * skillData.projectileSizeMulti);
        }
        projectile.SetProjectile(skillData);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public T SpawnProjectile<T>(PassiveData skillData, LayerConstant layer = LayerConstant.SKILL) where T : Projectile
    {
        return SpawnProjectile<T>(skillData, transform, layer);
    }

    public T SpawnProjectile<T>(PassiveData skillData, Transform shooter, LayerConstant layer = LayerConstant.SKILL) where T : Projectile
    {
        int id = skillData.skillId / 100;
        if (!skillPools.ContainsKey(id))
        {
            skillPools.Add(id, new ObjectPool<Projectile>(ResourcesManager.Load<Projectile>(passiveTable[skillData.skillId.ToString()]["PassivePrefabPath"].ToString()), shooter));
        }

        T projectile = (T)skillPools[id].GetObject();
        projectile.transform.parent = shooter;
        projectile.gameObject.layer = (int)layer;
        projectile.transform.localPosition = Vector2.zero;
        //projectile.CollisionPower(false);
        projectile.gameObject.SetActive(true);
        return projectile;
    }

    public void DeSpawnProjectile(Projectile projectile)
    {
        skillPools[projectile.skillData.skillId / 100].ReleaseObject(projectile);
        projectile.transform.parent = transform;
    }

    public void DeSpawnProjectile(Projectile projectile, int skillId)
    {
        skillPools[skillId / 100].ReleaseObject(projectile);
        projectile.transform.parent = transform;
    }
    #endregion

    public void SkillAdd(int skillId, Transform shooter, int skillNum)
    {
        Skill skill;

        foreach (int id in skillList.Keys)
        {
            if (id / 100 == skillId / 100)
            {
                DebugManager.Instance.PrintDebug("[SkillManager]: Skill Level Up!");
                skillList[id].SkillLevelUp();
                skill = skillList[id];
                skillList.Add(id + 1, skill);
                skillList.Remove(id);
                return;
            }
        }

        skill = FindSkill(skillId, shooter, skillNum);

        if (skillId / 10000 == 1)
        {
            Dictionary<string, Dictionary<string, object>> skillTable = CSVReader.Read("SkillTable");
            PlayerUI.Instance.skillBoxUi.SkillIconInit(skillTable[skillId.ToString()]["Icon"].ToString(), skillNum);
            PlayerUI.Instance.activeSkillCount++;
        }
        else if (skillId / 10000 == 2)
        {
            Dictionary<string, Dictionary<string, object>> passiveTable = CSVReader.Read("PassiveTable");
            PlayerUI.Instance.skillBoxUi.SkillIconInit(passiveTable[skillId.ToString()]["Icon"].ToString(), skillNum + ACTIVE_SKILL_MAX_COUNT);
            PlayerUI.Instance.passiveSkillCount++;
        }

        IEnumerator enumerator = skill.SkillActivation();
        StartCoroutine(enumerator);
        skillList.Add(skillId, skill);
        skillCoroutineList.Add(skillId, enumerator);
    }

    public void CoroutineStarter(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public void SwapSkill(int prevId, int newId)
    {
        ActiveSkill prevSkill = (ActiveSkill)skillList[prevId];

        StopCoroutine(skillCoroutineList[prevId]);
        skillCoroutineList.Remove(prevId);
        Skill skill = FindSkill(newId, prevSkill.shooter, prevSkill.skillNum);
        skillList.Remove(prevId);

        Dictionary<string, Dictionary<string, object>> skillTable = CSVReader.Read("SkillTable");
        PlayerUI.Instance.skillBoxUi.SkillIconInit(skillTable[newId.ToString()]["Icon"].ToString(), prevSkill.skillNum);

        IEnumerator enumerator = skill.SkillActivation();
        StartCoroutine(enumerator);
        skillList.Add(newId, skill);
        skillCoroutineList.Add(newId, enumerator);
    }

    public Skill FindSkill(int skillId, Transform shooter, int skillNum)
    {
        switch (skillId / 100)
        {
            case 101:
                return new Juhon(skillId, shooter, skillNum);
            case 102:
                return new Bujung(skillId, shooter, skillNum);
            case 103:
                return new GangSin(skillId, shooter, skillNum);
            case 104:
                return new GodBless(skillId, shooter, skillNum);
            case 105:
                return new Possession(skillId, shooter, skillNum);
            case 106:
                return new Irons(skillId, shooter, skillNum);
            case 107:
                return new GwiGi(skillId, shooter, skillNum);
            case 108:
                return new JuHyung(skillId, shooter, skillNum);
            case 109:
                return new MyeongGyae(skillId, shooter, skillNum);
            case 110:
                return new Crepitus(skillId, shooter, skillNum);
            case 111:
                return new GyuGyu(skillId, shooter, skillNum);
            case 112:
                return new Aliento(skillId, shooter, skillNum);
            case 113:
                return new Pok(skillId, shooter, skillNum);
            case 114:
                return new JeRyeung(skillId, shooter, skillNum);
            case 115:
                return new ParkSung(skillId, shooter, skillNum);
            case 116:
                return new Inn(skillId, shooter, skillNum);
            case 117:
                return new Churk(skillId, shooter, skillNum);
            case 118:
                return new BunGye(skillId, shooter, skillNum);
            case 119:
                return new SunYang(skillId, shooter, skillNum);
            case 120:
                return new Horin(skillId, shooter, skillNum);
            case 121:
                return new JuckHwa(skillId, shooter, skillNum);
            case 122:
                return new KumJul(skillId, shooter, skillNum);
            case 123:
                return new ChyuRyung(skillId, shooter, skillNum);
            case 124:
                return new JuckJung(skillId, shooter, skillNum);
            case 125:
                return new HwiPung(skillId, shooter, skillNum);
            case 126:
                return new SeRae(skillId, shooter, skillNum);
            case 127:
                return new GamYum(skillId, shooter, skillNum);
            case 128:
                return new Ildo(skillId, shooter, skillNum);
            case 129:
                return new JuOrk(skillId, shooter, skillNum);
            case 130:
                return new BuGong(skillId, shooter, skillNum);
            case 301:
                return new Horin(skillId, shooter, skillNum);
            //아래로 패시브
            case 201:
                return new MyungSang(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 202:
                return new InnPassive(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 203:
                return new HyulPok(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 204:
                return new DaeHum(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 205:
                return new GaSok(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 206:
                return new Hyum(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 207:
                return new JaeSaeng(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 208:
                return new HwakSan(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 209:
                return new HwakHo(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 210:
                return new JuJuGaSork(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 211:
                return new JuJuJyungPok(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 212:
                return new GwangHwa(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 213:
                return new ChangAe(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            case 214:
                return new ChukDan(skillId, shooter, skillNum + ACTIVE_SKILL_MAX_COUNT);
            default:
                DebugManager.Instance.PrintError("[SkillManager] 미구현된 스킬입니다 (Skill ID: {0})", skillId);
                return null;
        }
    }

    

}
