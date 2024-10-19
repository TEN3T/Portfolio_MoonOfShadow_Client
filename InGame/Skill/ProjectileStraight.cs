using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStraight : Projectile
{
    private float shiftingDistance = 0.0f;
    private int bounced;
    private Vector2 target;
    private bool isFire;
    private List<Transform> exceptions;

    protected override void OnEnable()
    {
        base.OnEnable();
        shiftingDistance = 0.0f;
        bounced = 0;
        isFire = false;
        target = Vector2.right;
        exceptions = new List<Transform>();
    }

    private void FixedUpdate()
    {
        if (bounceCount != 0)
        {
            if (!isFire)
            {
                rigid.velocity = Quaternion.Normalize(transform.rotation) * Vector2.up * skillData.speed;
                isFire = true;
            }
            else
            {
                rigid.velocity = rigid.velocity.normalized * skillData.speed;
            }

            if (bounced >= bounceCount)
            {
                Remove();
            }
        }
        else
        {
            float speed = skillData.speed * Time.fixedDeltaTime;
            transform.Translate(Vector2.right * speed);
            shiftingDistance += speed;
            if (shiftingDistance > skillData.attackDistance)
            {
                Remove();
            }
        }

    }

    public void SetFireDirection(Vector2 target)
    {
        this.target = target;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (bounceCount == 0)
        {
            base.OnTriggerEnter2D(collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.TryGetComponent(out Monster monster))
        {
            monster.isHit = true;
            monster.Hit(skillData.damage);
            SkillEffect(monster);
            ++bounced;
            
            if (isMetastasis)
            {
                exceptions.Add(monster.transform);
                target = Scanner.GetTarget(SKILLCONSTANT.SKILL_TARGET.RANDOM, monster.transform, 100, exceptions) - (Vector2)monster.transform.position;
                rigid.velocity = target.normalized * skillData.speed;
            }

            DebugManager.Instance.PrintDebug("[TEST]: Hit");
        }
        else if (collision.gameObject.layer == (int)LayerConstant.MAP)
        {
            ++bounced;
        }
    }
}
