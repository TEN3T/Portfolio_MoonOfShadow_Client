using System.Collections;
using UnityEngine;

public class MobStatue : FieldStructure
{
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = top.GetComponent<SpriteRenderer>();
        this.hp = float.Parse(this.fieldStructureData.gimmickParam[0]);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!spriteRenderer.enabled)
        {
            return;
        }

        if (collision.gameObject.TryGetComponent(out Projectile projectile))
        {
     
            this.hp -= projectile.skillData.damage;
            if (this.hp <= 0)
            {
                StartCoroutine(Activation());
            }
            else {
                if (soundRequester != null)
                {
                    DebugManager.Instance.PrintDebug("[SoundRequest] MobStatue Hit");
                    soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.HIT);
                }
            }
        }
    }

    private IEnumerator Activation()
    {
        if (soundRequester != null)
        {
            DebugManager.Instance.PrintDebug("[SoundRequest] MobStatue DESTORY");
            soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.DESTORY);
        }

        ItemManager.Instance.DropItems(this.fieldStructureData.gimmickParam[1], transform);
        spriteRenderer.enabled = false;
        top.enabled = false;
        yield return new WaitForSeconds(this.fieldStructureData.coolTime);
        spriteRenderer.enabled = true;
        top.enabled = true;
        this.hp = 1;
    }

    public override void Remove()
    {

        StartCoroutine(Activation());
    }
}
