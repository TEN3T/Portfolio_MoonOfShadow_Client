using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFlame : FieldStructure
{
    [SerializeField][Min (0.0f)] private float damage = 5.0f;      //N
    [SerializeField][Min (0)] private int burnTime = 3;    //M
    [SerializeField][Min (0.0f)] private float burnDamage = 3.0f;  //X
    [SerializeField][Range (0.0f, 100.0f)] private float burnSlow = 5.0f;    //T
    [SerializeField][Min (1.0f)] private float duration = 3.0f;    //Z

    private WaitForFixedUpdate frame;
    private float burnDotTime;
    private float currentBurnTime;
    private float currentDuration;
    //private Vector3 originSize;
    private bool isTeleport;
    private float speed = 2.5f;

    protected override void Awake()
    {
        base.Awake();

        burnDotTime = 1.0f;
        currentBurnTime = burnDotTime;
        currentDuration = duration;
        //originSize = transform.localScale;
        isTeleport = false;
        frame = new WaitForFixedUpdate();
    }

    private void Update()
    {
        if (currentBurnTime > 0.0f)
        {
            currentBurnTime -= Time.deltaTime;
        }
        else if (currentBurnTime <= -0.1f)
        {
            currentBurnTime = burnDotTime;
        }

        if (currentDuration > 0.0f && !isTeleport)
        {
            currentDuration -= Time.deltaTime;
        }
        else if (!isTeleport)
        {
            isTeleport = true;
            StartCoroutine(Teleport());
        }
    }

    private IEnumerator Teleport()
    {
        float offSet = 0f;
        if (soundRequester != null)
        {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.DEMISE);
        }
        while (offSet > -1.6f)
        {
            offSet -= Time.deltaTime * speed;
            top.transform.localPosition = new Vector2(0.15f, offSet);
            //transform.localScale = originSize * offSet;
            yield return frame;
        }

        transform.localPosition = CameraManager.Instance.GetRandomPosition(transform.position);

        if (soundRequester != null)
        {
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.SPAWN);
        }

        while (offSet < 0.0f)
        {
            offSet += Time.deltaTime * speed;
            if (offSet >= 0.0f)
            {
                offSet = 0.0f;
            }
            //transform.localScale = originSize * offSet;
            top.transform.localPosition = new Vector2(0.15f, offSet);
            yield return frame;
        }

        currentDuration = duration;
        isTeleport = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (currentBurnTime <= 0.0f)
        {
            if (collision.TryGetComponent(out Monster monster))
            {
                DebugManager.Instance.PrintDebug("[DamgeLog] BlueFrame Damaged To " + monster.GetMonsterID());
                monster.Hit((int)damage);
                monster.SkillEffectActivation(SKILLCONSTANT.SKILL_EFFECT.SLOW, burnSlow, 1.0f);
                return;
            }
            if (collision.gameObject.layer != (int)LayerConstant.SKILL && collision.transform.parent.TryGetComponent(out Player player))
            {
                StartCoroutine(player.FireDot(1, damage));
                StartCoroutine(player.Slow(1.0f, burnSlow));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Monster monster))
        {
            StartCoroutine(monster.FireDot(burnTime, damage));
            return;
        }
        if (collision.gameObject.layer != (int)LayerConstant.SKILL && collision.transform.parent.TryGetComponent(out Player player))
        {
            if (Vector2.Distance(player.transform.position, transform.position) >= transform.localScale.x * 0.5f)
            {
                player.RemoveStatusEffect(STATUS_EFFECT.FIRE);
                StartCoroutine(player.FireDot(burnTime, damage));
            }
        }
    }
}
