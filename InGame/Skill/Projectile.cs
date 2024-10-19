using SKILLCONSTANT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum PROJECTILE_DIRECTION
{
    LEFT,
    RIGHT,
}

public class Projectile : MonoBehaviour
{
    private const string BOUNCE_PATH = "Prefabs/InGame/Skill/Bounce";

    [SerializeField] AudioClip shootAudioClip;
    [SerializeField] AudioClip hitAudioClip;
    [SerializeField] AudioClip destroyAudioClip;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    protected Rigidbody2D rigid;
    protected Collider2D projectileCollider;
    protected int bounceCount;
    protected bool isMetastasis = false;
    protected int hitCount;

    //public float totalDamage { get; private set; }
    public ActiveData skillData { get; private set; }
    public Monster collisionMonster { get; private set; }
    public float particleSpeed { get; private set; }

    private void Awake()
    {
        if (!TryGetComponent(out projectileCollider))
        {
            projectileCollider = GetComponentInChildren<Collider2D>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        //transform.GetComponent<Renderer>().sortingLayerName = LayerConstant.SPAWNOBJECT.ToString();
        if (transform.TryGetComponent(out Renderer renderer))
        {
            renderer.sortingLayerName = LayerConstant.SPAWNOBJECT.ToString();
        }
        else
        {
            try
            {
                transform.GetComponentInChildren<Renderer>().sortingLayerName = LayerConstant.SPAWNOBJECT.ToString();
            }
            catch
            {
                DebugManager.Instance.PrintError("[Projectile: SortingLayer] Not Found Renderer Component.");
            }
        }

        SetSortingLayer(transform.Find("Effect"));

        if (TryGetComponent(out Animator animator))
        {
            this.animator = animator;
        }

        ParticleControll pc = GetComponentInChildren<ParticleControll>();
        if (pc != null)
        {
            particleSpeed = pc.GetParticleSpeed();
        }

    }

    private void SetSortingLayer(Transform trans)
    {
        if (trans == null)
        {
            return;
        }
        if (trans.TryGetComponent(out Renderer renderer))
        {
            renderer.sortingLayerName = LayerConstant.SPAWNOBJECT.ToString();
        }

        foreach (Transform child in trans)
        {
            SetSortingLayer(child);
        }
    }

    protected virtual void OnEnable()
    {
        collisionMonster = null;
        hitCount = 0;
        if (projectileCollider != null)
        {
            projectileCollider.enabled = true;
        }
        //발사 사운드
        if (shootAudioClip != null)
        {
            SoundManager.Instance.PlayAudioClip("Skill", shootAudioClip);
        }
    }

    public void SetDirection(PROJECTILE_DIRECTION direction)
    {
        spriteRenderer.flipX = direction == PROJECTILE_DIRECTION.LEFT ? true : false;
    }

    public void SetAnimation(Sprite sprite, RuntimeAnimatorController controller)
    {
        spriteRenderer.sprite = sprite;
        animator.runtimeAnimatorController = controller;
    }

    public virtual void SetProjectile(ActiveData skillData)
    {
        this.skillData = skillData;
        if (projectileCollider != null)
        {
            projectileCollider.isTrigger = true;
        }
        
        for (int i = 0; i < this.skillData.skillEffect.Count; i++)
        {
            if (this.skillData.skillEffect[i] == SKILLCONSTANT.SKILL_EFFECT.BOUNCE)
            {
                rigid = GetComponent<Rigidbody2D>();
                projectileCollider.sharedMaterial = ResourcesManager.Load<PhysicsMaterial2D>(BOUNCE_PATH);
                bounceCount = Convert.ToInt32(this.skillData.skillEffectParam[i]);
                projectileCollider.isTrigger = false;
            }

            if (this.skillData.skillEffect[i] == SKILLCONSTANT.SKILL_EFFECT.METASTASIS)
            {
                rigid = GetComponent<Rigidbody2D>();
                bounceCount = Convert.ToInt32(this.skillData.skillEffectParam[i]);
                isMetastasis = true;
                projectileCollider.isTrigger = false;
            }
        }

        SkillEffect(null);

        //if (skillData.skillType == SKILL_TYPE.RANGES)
        //{
        //    SkillManager.Instance.SpawnRangeCircle(skillData.duration, this.transform);
        //}
    }

    public void SetAlpha(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    public void CollisionRadius(float radius)
    {
        if (projectileCollider != null)
        {
            ((CircleCollider2D)projectileCollider).radius = radius;
        }
    }

    public void CollisionPower(bool flag)
    {
        if (projectileCollider != null)
        {
            projectileCollider.enabled = flag;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Monster monster))
        {
            collisionMonster = monster;

            //float totalDamage = skillData.damage;
            //hitCount++;
            //if (hitCount == 1)
            //{
            //    totalDamage += SoulManager.Instance.GetEffect(SoulEffect.SINGLEDAMAGE, skillData._damage);
            //}
            //else if (hitCount > 1)
            //{
            //    totalDamage += hitCount * SoulManager.Instance.GetEffect(SoulEffect.MULTIATTACK, skillData._damage);
            //}

            //if (monster.monsterData.monsterType == MonsterType.BOSS)
            //{
            //    totalDamage += SoulManager.Instance.GetEffect(SoulEffect.BOSSDAMAGE, skillData._damage);
            //}
            //else
            //{
            //    totalDamage += SoulManager.Instance.GetEffect(SoulEffect.NORMALDAMAGE, skillData._damage);
            //}

            //monster.Hit(totalDamage);
            monster.Hit(CalTotalDamage(monster));
            
            SkillEffect(monster);
            //충돌 사운드
            if (hitAudioClip != null)
            {
                SoundManager.Instance.PlayAudioClip("Skill", hitAudioClip);
            }

            if (!skillData.isPenetrate)
            {
                Remove();
            }
            DebugManager.Instance.PrintDebug("[TEST]: Hit");
        }
    }

    public float CalTotalDamage(Monster monster)
    {
        float totalDamage = skillData.damage;
        hitCount++;
        if (hitCount == 1)
        {
            totalDamage += SoulManager.Instance.GetEffect(SoulEffect.SINGLEDAMAGE, skillData._damage);
        }
        else if (hitCount > 1)
        {
            totalDamage += hitCount * SoulManager.Instance.GetEffect(SoulEffect.MULTIATTACK, skillData._damage);
        }

        if (monster.monsterData.monsterType == MonsterType.BOSS)
        {
            totalDamage += SoulManager.Instance.GetEffect(SoulEffect.BOSSDAMAGE, skillData._damage);
        }
        else
        {
            totalDamage += SoulManager.Instance.GetEffect(SoulEffect.NORMALDAMAGE, skillData._damage);
        }

        return totalDamage;
    }

    //private void OnBecameInvisible()
    //{
    //    Invoke("Remove", skillData.duration * 2);
    //}

    protected void Remove()
    {
        SkillManager.Instance.DeSpawnProjectile(this);
        //소멸 사운드
        if (destroyAudioClip != null)
        {
            SoundManager.Instance.PlayAudioClip("Skill", destroyAudioClip);
        }
    }

    protected void SkillEffect(Monster target)
    {
        int count = skillData.skillEffect.Count;
        for (int i = 0, j = 0; i < count; i++)
        {
            float param = float.Parse(skillData.skillEffectParam[j]);
            switch (skillData.skillEffect[i])
            {
                case SKILL_EFFECT.EXPLORE:
                    Explore(param);
                    break;
                case SKILL_EFFECT.MOVEUP:
                    StartCoroutine(MoveUp(param));
                    break;
                case SKILL_EFFECT.DRAIN:
                    Drain(param);
                    break;
                case SKILL_EFFECT.DELETE:
                    Delete(param, float.Parse(skillData.skillEffectParam[++j]));
                    break;
                case SKILL_EFFECT.TRANSITION:
                    if (target != null)
                    {
                        SkillManager.Instance.CoroutineStarter(target.Transition(param, int.Parse(skillData.skillEffectParam[++j])));
                    }
                    break;
                case SKILL_EFFECT.STUN:
                case SKILL_EFFECT.SLOW:
                case SKILL_EFFECT.KNOCKBACK:
                case SKILL_EFFECT.EXECUTE:
                case SKILL_EFFECT.RESTRAINT:
                case SKILL_EFFECT.PULL:
                    if (target != null)
                    {
                        target.SkillEffectActivation(skillData.skillEffect[i], param);
                    }
                    break;
                case SKILL_EFFECT.SPAWNMOB:
                    SkillManager.Instance.CoroutineStarter(SpawnMob(param));
                    break;
                case SKILL_EFFECT.CHANGEFORM:
                    SkillManager.Instance.CoroutineStarter(GameManager.Instance.player.ChangeForm(skillData.duration, param));
                    break;
                default:
                    DebugManager.Instance.PrintDebug("[ERROR]: 없는 스킬 효과입니다");
                    break;
            }
            j++;
        }
    }

    private void Explore(float n)
    {
        if (UnityEngine.Random.Range(0, 100) < n)
        {
            List<Transform> targets = Scanner.RangeTarget(transform, skillData.splashRange, (int)LayerConstant.MONSTER);
            foreach (Transform target in targets)
            {
                if (target.TryGetComponent(out Monster monster))
                {
                    monster.monsterData.SetCurrentHp(monster.monsterData.currentHp - (int)skillData.damage);
                }
            }
        }
    }

    private void Delete(float n, float m) //n: gimmick, m: item
    {
        List<Transform> targets;
        if (UnityEngine.Random.Range(0, 100) < n)
        {
            targets = Scanner.RangeTarget(transform, skillData.splashRange, (int)LayerConstant.MONSTER, (int)LayerConstant.GIMMICK);
            foreach (Transform target in targets)
            {
                if (target.TryGetComponent(out Monster monster))
                {
                    monster.Die(true);
                }
                else if (target.TryGetComponent(out BlueFlame blueFlame))
                {
                    blueFlame.Remove();
                }
                else if (target.parent.TryGetComponent(out MobStatue statue))
                {
                    statue.Remove();
                }
            }
        }

        if (UnityEngine.Random.Range(0, 100) < m)
        {
            targets = Scanner.RangeTarget(transform, skillData.splashRange, (int)LayerConstant.ITEM);
            foreach (Transform target in targets)
            {
                if (target.TryGetComponent(out Item item))
                {
                    ItemManager.Instance.DeSpawnItem(item);
                }
            }
        }
    }

    private IEnumerator MoveUp(float n)
    {
        GameManager.Instance.player.playerManager.playerData.MoveSpeedModifier(n * 0.01f);
        yield return new WaitForSeconds(skillData.duration);
        GameManager.Instance.player.playerManager.playerData.MoveSpeedModifier(-n * 0.01f);
    }

    private void Drain(float n)
    {
        float hp = skillData.damage * n * 0.01f;
        GameManager.Instance.player.playerManager.playerData.CurrentHpModifier((int)hp);
    }

    private IEnumerator SpawnMob(float n)
    {
        Monster summoner = MonsterSpawner.Instance.SpawnFriendlyMonster((int)n, Scanner.GetTarget(skillData.skillTarget, transform, skillData.attackDistance));
        summoner.SetTarget(Scanner.GetTargetTransform(SKILL_TARGET.MELEE, transform, 999, new List<Transform>() { summoner.transform, }), true);
        summoner.StatusUpdate(0, (int)skillData.damage, skillData.speed);
        yield return new WaitForSeconds(skillData.duration);

        if (summoner.gameObject.activeInHierarchy)
        {
            summoner.Die(false);
        }
    }
}
