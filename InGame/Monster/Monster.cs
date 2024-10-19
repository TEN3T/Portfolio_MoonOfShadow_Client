
using Cinemachine.Utility;
using SKILLCONSTANT;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [field: SerializeField] public MonsterData monsterData { get; private set; }

    private CapsuleCollider2D monsterCollider2;
    private CapsuleCollider2D monsterCollider;
    private Rigidbody2D monsterRigidbody;
    private Vector2 monsterDirection;

    private bool spineSwitch;
    private StatusEffect statusEffect;
    private BehaviorTreeManager btManager;

    private float weightX;
    private float weightY;
    private WaitForSeconds delay;
    private WaitForSeconds tick;

    private HpBar hpBar;
    private float hpBarVisibleTime;
  
    private WaitForFixedUpdate fixedFrame;

    private MonsterCollider attackCollider;
    private SpineManager spineManager;
    private SoundRequester soundRequester;
    private SoundSituation.SOUNDSITUATION situation;
    private int exState = 0;

    public bool isFriendly { get; private set; }
    public int monsterId { get; set; }
    public bool isHit { get; set; }
    public bool isAttack { get; private set; }
    public Transform target { get; private set; }
    public Vector2 lookDirection { get; private set; } //바라보는 방향

    [SerializeField]
    public String monsterID;

    #region Mono & Setting
    private void Awake()
    {
        statusEffect = new StatusEffect();
        attackCollider = GetComponentInChildren<MonsterCollider>();
        monsterCollider2 = transform.Find("Collision").GetComponent<CapsuleCollider2D>();
        monsterCollider = GetComponent<CapsuleCollider2D>();
        spineManager = GetComponent<SpineManager>();
        soundRequester = GetComponent<SoundRequester>();
        monsterRigidbody = GetComponent<Rigidbody2D>();
        tick = new WaitForSeconds(0.4f);
        hpBarVisibleTime = Convert.ToInt32(CSVReader.Read("BattleConfig", "HpBarVisibleTime", "ConfigValue")) / 1000.0f;
        fixedFrame = new WaitForFixedUpdate();

      
        Guid guid = Guid.NewGuid();
        monsterID = guid.ToString();
        DebugManager.Instance.PrintDebug("[Monster Spawn] "+ monsterID);
    }

    private void Start()
    {
        btManager = new BehaviorTreeManager(SetAI(monsterData.attackType));
        spineManager.SetAnimation("Idle", true);
        attackCollider.SetAttackDistance(monsterData.atkDistance);

        if (monsterData.atkDistance <= 1.0f)    //근거리
        {
            weightX = attackCollider.attackCollider.size.x * 0.3f;
            weightY = attackCollider.attackCollider.size.y * 0.3f;
        }
        else    //원거리
        {
            weightX = monsterData.atkDistance;
            weightY = monsterData.atkDistance;
        }
        weightX = Math.Abs(weightX);
        weightY = Math.Abs(weightY);
    }

    private void Update()
    {
        if (isFriendly)
        {
            if (target == null)
            {
                this.target = FindEnemy();
            }
            else
            {
                Transform enemy = FindEnemy();
                if (this.target != enemy)
                {
                    this.target = enemy;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (spineSwitch)
        {
            btManager.Active();
            monsterRigidbody.velocity = Vector2.zero;
        }
    }

    public void SpawnSet(float hpCoefficient, float attackCoefficient)
    {
        Physics2D.IgnoreCollision(monsterCollider, monsterCollider2);
        
        monsterCollider.enabled = true;
        monsterCollider.enabled = true;
        monsterDirection = Vector2.zero;
        lookDirection = Vector2.right;
        MonsterDataSetting(monsterId.ToString(), hpCoefficient, attackCoefficient);
        delay = new WaitForSeconds(1.0f / monsterData.atkSpeed);

        isAttack = false;
        isHit = false;
        spineSwitch = true;
        //isSlow = false;
    }

    public void StatusUpdate(int hp, int attack, float moveSpeed)
    {
        monsterData.SetHp(monsterData.hp + hp);
        monsterData.SetCurrentHp(monsterData.hp);
        monsterData.SetAttack(monsterData.attack + attack);
        monsterData.SetMoveSpeed(monsterData.moveSpeed + moveSpeed);
    }

    private void OnEnable()
    {

        if (soundRequester != null)
        {
            soundRequester.GetBackSpeakers();
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.SPAWN);
         
        }
    }
    public string GetMonsterID() { 
        return monsterID;
    }

    public void SetTarget(Transform target, bool isFriendly)
    {
        this.target = target;
        this.isFriendly = isFriendly;
    }

    public void MonsterDataSetting(string monsterId, float hpCoefficient, float attackCoefficient)
    {
        Dictionary<string, Dictionary<string, object>> monsterTable = CSVReader.Read("MonsterTable");
        if (monsterTable.ContainsKey(monsterId))
        {
            Dictionary<string, object> table = monsterTable[monsterId];
            monsterData.SetMonsterName(Convert.ToString(table["MonsterName"]));
            int hp = Convert.ToInt32(table["HP"]);
            hp += Mathf.FloorToInt(hp * hpCoefficient);
            monsterData.SetHp(hp);
            monsterData.SetCurrentHp(monsterData.hp);
            monsterData.SetSizeMultiple(float.Parse(Convert.ToString(table["SizeMultiple"])));
            int attack = Convert.ToInt32(table["Attack"]);
            attack += Mathf.FloorToInt(attack * attackCoefficient);
            monsterData.SetAttack(attack);
            monsterData.SetMoveSpeed(float.Parse(Convert.ToString(table["MoveSpeed"])));
            monsterData.SetAtkSpeed(float.Parse(Convert.ToString(table["AtkSpeed"])));
            monsterData.SetViewDistance(float.Parse(Convert.ToString(table["ViewDistance"])));
            monsterData.SetAtkDistance(float.Parse(Convert.ToString(table["AtkDistance"])));
            monsterData.SetSkillID(Convert.ToInt32(table["SkillID"]));
            monsterData.SetGroupSource(Convert.ToString(table["GroupSource"]));
            monsterData.SetGroupSourceRate(Convert.ToInt32(table["GroupSourceRate"]));
            monsterData.SetMonsterPrefabPath(Convert.ToString(table["MonsterPrefabPath"]));

            if (Enum.TryParse(Convert.ToString(table["AttackType"]), true, out AttackType attackType))
            {
                monsterData.SetAttackType(attackType);
            }
            else
            {
                DebugManager.Instance.PrintError("[Error: Monster] 테이블의 AttackType을 체크해 주세요 (MonsterID: {0})", monsterId);
                monsterData.SetAttackType(AttackType.Bold);
            }

            if (Enum.TryParse(Convert.ToString(table["MonsterType"]), true, out MonsterType monsterType))
            {
                monsterData.SetMonsterType(monsterType);
            }
            else
            {
                DebugManager.Instance.PrintError("[Error: Monster] 테이블의 MonsterType을 체크해 주세요 (MonsterID: {0})", monsterId);
                monsterData.SetMonsterType(MonsterType.NORMAL);
            }
        }
        spineSwitch = true;
    }
    #endregion

    #region AI
    private Node SetAI(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Bold:
                return BoldAI();
            case AttackType.Shy:
                return ShyAI();
            case AttackType.FRIENDLY:
                return FriendlyAI();
            default:
                return null;
        }
    }

    private Node BoldAI()
    {
        return new SelectorNode
                (new List<Node>()
                {
                    new SequenceNode
                    (new List<Node>()
                    {
                        new ActionNode(IsAttack),
                        new ActionNode(IsAttackable),
                        new ActionNode(Attack)
                    }),
                    new SequenceNode
                    (new List<Node>()
                    {
                        new ActionNode(IsVisible),
                        new ActionNode(Run)
                    }),
                    new ActionNode(Idle)
                });
    }

    private Node ShyAI()
    {
        return new SelectorNode
                (new List<Node>()
                {
                    new SequenceNode
                    (new List<Node>()
                    {
                        new ActionNode(IsHit),
                        new SelectorNode
                        (new List<Node>()
                        {
                            new SequenceNode
                            (new List<Node>()
                            {
                                new ActionNode(IsAttack),
                                new ActionNode(IsAttackable),
                                new ActionNode(Attack)
                            }),
                            new ActionNode(Run)
                        })
                    }),
                    new ActionNode(Idle),
                });
    }
    
    private Node FriendlyAI()
    {
        //gameObject.layer = (int)LayerConstant.HIT;
        this.SetTarget(Scanner.GetTargetTransform(SKILL_TARGET.MELEE, transform, 999, new List<Transform>() { transform, }), true);

        return BoldAI();
    }
    #endregion

    #region AI_Logic
    private NodeConstant IsAttack()
    {
        return spineManager.GetAnimationName().Equals("Attack") ? NodeConstant.RUNNING : NodeConstant.SUCCESS;
    }

    private NodeConstant IsAttackable()
    {
        if (target == null)
        {
            return NodeConstant.FAILURE;
        }

        Vector2 diff = (target.position - transform.position).Abs();
        
        if ((diff.x <= weightX) && (diff.y <= weightY))
        {
            return NodeConstant.SUCCESS;
        }
        return NodeConstant.FAILURE;
    }

    private NodeConstant Attack()
    {
        monsterRigidbody.velocity = Vector3.zero;
        if (soundRequester != null) { 
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.ATTACK);
        }
        exState = 2;
            
        if (!isAttack)
        {
            spineManager.SetAnimation("Attack", false);
            spineManager.AddAnimation("Idle", true);
            StartCoroutine("AttackDelay");
        }
        return NodeConstant.SUCCESS;
    }

    private NodeConstant IsVisible()
    {
        if (target == null)
        {
            return NodeConstant.FAILURE;
        }

        return (target.position - transform.position).magnitude <= monsterData.viewDistance ? NodeConstant.SUCCESS : NodeConstant.FAILURE;
    }

    private NodeConstant Run()
    {
        if (isAttack)
        {
            isAttack = false;
            StopCoroutine("AttackDelay");
        }

        Vector2 diff = target.position - transform.position;
        float distance = diff.magnitude;

        if (distance <= monsterData.atkDistance)
        {
            return NodeConstant.SUCCESS;
        }

        spineManager.SetAnimation("Run", true, 0, monsterData.moveSpeed);
        monsterDirection = diff.normalized;
        spineManager.SetDirection(transform, monsterDirection);
        monsterRigidbody.MovePosition(monsterRigidbody.position + (monsterDirection * monsterData.moveSpeed * Time.fixedDeltaTime));

        if ((soundRequester != null && exState != 1) || (soundRequester != null && !soundRequester.isPlaying(SoundSituation.SOUNDSITUATION.RUN)))
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.RUN);

        exState = 1;

        return NodeConstant.RUNNING;
    }

    private NodeConstant Idle()
    {
        if (isAttack)
        {
            isAttack = false;
            StopCoroutine("AttackDelay");
        }

        isAttack = false;
        spineManager.SetAnimation("Idle", true);
        if ((soundRequester != null && exState != 0) || (soundRequester != null && !soundRequester.isPlaying(SoundSituation.SOUNDSITUATION.IDLE)))
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.IDLE);

        exState = 0;
        return NodeConstant.SUCCESS;
    }

    private NodeConstant IsHit()
    {
        return isHit ? NodeConstant.SUCCESS : NodeConstant.FAILURE;
    }

    private IEnumerator AttackDelay()
    {
        yield return tick;
        isAttack = true;
        attackCollider.AttackColliderSwitch(true);
        //yield return tick;
        yield return delay;
        attackCollider.AttackColliderSwitch(false);
        isAttack = false;
    }

    private Transform FindEnemy()
    {
        Transform enemy = Scanner.GetTargetTransform(SKILL_TARGET.MELEE, transform, 999, new List<Transform>() { transform, });
        if (enemy != null && enemy.TryGetComponent(out Monster monster))
        {
            return monster.isFriendly == false ? enemy : null;
        }

        return null;
    }
    #endregion

    #region Logic
    private void DropItem()
    {
        if (UnityEngine.Random.Range(0, 10001) <= monsterData.groupSourceRate)
        {
            ItemManager.Instance.DropItems(monsterData.groupSource, transform);
        }
    }

    public void Hit(float totalDamage)
    {
        if (hpBar == null && gameObject.activeInHierarchy)
        {
            StartCoroutine(HpBarControl());
        }

        isHit = true;

        if (soundRequester != null && gameObject.activeInHierarchy)
        {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.HIT);
        }

        monsterData.SetCurrentHp(monsterData.currentHp - (int)(totalDamage * GameManager.Instance.player.playerManager.playerData.attack));
        if (monsterData.currentHp <= 0)
        {
            Die(true);
        }
    }

    private IEnumerator HpBarControl()
    {
        hpBar = (HpBar)UIPoolManager.Instance.SpawnUI("HpBar", PlayerUI.Instance.transform.Find("HpBarUI"), transform.position);
        float time = 0.0f;
        do
        {
            hpBar.HpBarSetting(transform.Find("HpBar").position, monsterData.currentHp, monsterData.hp);
            time += Time.fixedDeltaTime;
            yield return fixedFrame;
        } while (hpBar != null && time < hpBarVisibleTime && monsterData.currentHp > 0);

        if (hpBar != null)
        {
            UIPoolManager.Instance.DeSpawnUI("HpBar", hpBar);
            hpBar = null;
        }
    }

    public void Die(bool isDrop)
    {
        if (soundRequester != null) {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.DIE);
        }
        if (hpBar != null)
        {
            UIPoolManager.Instance.DeSpawnUI("HpBar", hpBar);
            hpBar = null;
        }

        monsterCollider.enabled = false;
        monsterCollider2.enabled = false;
        
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(DieAnimation());
        }

        if (isDrop)
        {
            DropItem();
        }
        
        GameManager.Instance.killCount++;
        StopAllCoroutines();
        MonsterSpawner.Instance.DeSpawnMonster(this);
    }

    private IEnumerator DieAnimation()
    {
        spineSwitch = false;
        try
        {
            spineManager.SetAnimation("Death", false);
        }
        catch
        {
            DebugManager.Instance.PrintDebug("[ERROR]: 스파인에 죽는 애니메이션이 없는 몬스터입니다");
        }
        yield return new WaitForSeconds(1.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster;
        if (isFriendly)
        {
            if (collision.transform.parent.TryGetComponent(out monster) || collision.TryGetComponent(out monster))
            {
                if (!monster.isFriendly)
                {
                    Hit(monster.monsterData.attack);
                }
            }
        }
        else
        {
            if (collision.transform.parent.TryGetComponent(out monster) || collision.TryGetComponent(out monster))
            {
                if (monster.isFriendly)
                {
                    Hit(monster.monsterData.attack);
                }
            }
        }
    }
    #endregion

    #region SKILL_EFFECT & STATUS_EFFECT
    public void SkillEffectActivation(SKILL_EFFECT effect, float param)
    {
        this.SkillEffectActivation(effect, param, 1.0f);
    }

    public void SkillEffectActivation(SKILL_EFFECT effect, float param, float sec)
    {
        isHit = true;
        if (gameObject.activeInHierarchy)
        {
            switch (effect)
            {
                case SKILL_EFFECT.STUN:
                    StartCoroutine(Stun(param));
                    break;
                case SKILL_EFFECT.SLOW:
                    StartCoroutine(Slow(param, sec));
                    break;
                case SKILL_EFFECT.KNOCKBACK:
                    StartCoroutine(KnockBack(param));
                    break;
                case SKILL_EFFECT.EXECUTE:
                    Execute(param);
                    break;
                case SKILL_EFFECT.RESTRAINT:
                    StartCoroutine(Restraint(param));
                    break;
                case SKILL_EFFECT.PULL:
                    StartCoroutine(Pull(param));
                    break;
                default:
                    DebugManager.Instance.PrintDebug("[ERROR]: 없는 스킬 효과입니다");
                    break;
            }
        }
    }

    private IEnumerator Stun(float n)
    {
        if (spineSwitch)
        {
            spineSwitch = false;
            float originSpeed = monsterData.moveSpeed;
            monsterData.SetMoveSpeed(0.0f);
            spineManager.SetAnimation("Idle", true);
            yield return new WaitForSeconds(n);
            monsterData.SetMoveSpeed(originSpeed);
            spineSwitch = true;
        }
    }

    private IEnumerator Slow(float n, float sec)
    {
        if (statusEffect.IsStatusEffect(STATUS_EFFECT.SLOW))
        {
            yield break;
        }

        statusEffect.AddStatusEffect(STATUS_EFFECT.SLOW);
        float originSpeed = monsterData.moveSpeed;
        monsterData.SetMoveSpeed(originSpeed * (1 - n * 0.01f));
        yield return new WaitForSeconds(sec);
        monsterData.SetMoveSpeed(originSpeed);
        statusEffect.RemoveStatusEffect(STATUS_EFFECT.SLOW);
    }

    private IEnumerator Slow(float n)
    {
        yield return this.Slow(n, 1.0f);
    }

    private IEnumerator KnockBack(float n)
    {
        if (spineSwitch)
        {
            spineSwitch = false;
            Vector2 diff = transform.position - GameManager.Instance.player.transform.position;
            monsterRigidbody.AddRelativeForce(diff.normalized * n * 0.0002f, ForceMode2D.Impulse);
            yield return KnockBackReset(0.5f);
            spineSwitch = true;
        }
    }

    private IEnumerator KnockBackReset(float time)
    {
        yield return new WaitForSeconds(time);
        monsterRigidbody.velocity = Vector3.zero;
    }

    private void Execute(float n)
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) < n)
        {
            Die(true);
        }
    }

    private IEnumerator Restraint(float n)
    {
        if (statusEffect.IsStatusEffect(STATUS_EFFECT.RESTRAINT))
        {
            yield break;
        }

        statusEffect.AddStatusEffect(STATUS_EFFECT.RESTRAINT);
        float originSpeed = monsterData.moveSpeed;
        monsterData.SetMoveSpeed(0.0f);
        yield return new WaitForSeconds(n);
        monsterData.SetMoveSpeed(originSpeed);
        statusEffect.RemoveStatusEffect(STATUS_EFFECT.RESTRAINT);
    }

    private IEnumerator Pull(float n)
    {
        if (spineSwitch)
        {
            spineSwitch = false;
            Vector2 diff = target.position - transform.position;
            monsterRigidbody.AddRelativeForce(diff.normalized * n * 0.0002f, ForceMode2D.Impulse);
            yield return tick;
            spineSwitch = true;
        }
    }

    public IEnumerator FireDot(int time, float dotDamage)
    {
        if (statusEffect.IsStatusEffect(STATUS_EFFECT.FIRE))
        {
            yield break;
        }

        statusEffect.AddStatusEffect(STATUS_EFFECT.FIRE);
        WaitForSeconds sec = new WaitForSeconds(1.0f);
        for (int i = 0; i < time; i++)
        {
            this.Hit(-(int)dotDamage);
            yield return sec;
        }
        statusEffect.RemoveStatusEffect(STATUS_EFFECT.FIRE);
    }

    public IEnumerator Transition(float time, int monsterId)
    {
        if (statusEffect.IsStatusEffect(STATUS_EFFECT.TRANSITION))
        {
            yield break;
        }

        if (monsterData.currentHp <= 0.0f)
        {
            yield break;
        }

        statusEffect.AddStatusEffect(STATUS_EFFECT.TRANSITION);

        SkeletonDataAsset asset = spineManager.GetSkeletonDataAsset();
        spineManager.SetSkeletonDataAsset(ResourcesManager.Load<Monster>(CSVReader.Read("MonsterTable", monsterId.ToString(), "MonsterPrefabPath").ToString()).transform.Find("Character").GetComponent<SkeletonAnimation>().skeletonDataAsset);
        spineManager.SetAnimation("Idle", true);
        //yield return new WaitForSeconds(time);
        float currentTime = 0.0f;
        while (currentTime < time)
        {
            currentTime += Time.fixedDeltaTime;
            yield return fixedFrame;
            if (monsterData.currentHp <= 0.0f)
            {
                statusEffect.RemoveStatusEffect(STATUS_EFFECT.TRANSITION);
                spineManager.SetSkeletonDataAsset(asset);
                spineManager.SetAnimation("Idle", true);
                if (Scanner.RangeTarget(transform, 2.0f, (int)LayerConstant.MONSTER)[0].TryGetComponent(out Monster monster))
                {
                    SkillManager.Instance.CoroutineStarter(monster.Transition(time, monsterId));
                }
                yield break;
            }
        }
        statusEffect.RemoveStatusEffect(STATUS_EFFECT.TRANSITION);
        spineManager.SetSkeletonDataAsset(asset);
        spineManager.SetAnimation("Idle", true);
    }
    #endregion
}