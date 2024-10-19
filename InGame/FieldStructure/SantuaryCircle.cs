using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SantuaryCircle : FieldStructure
{
    private bool isHeal;
    private int hpRegen;
    private int duration;
    private WaitForSeconds tick;
    private WaitForSeconds sec;
    private SpriteRenderer sprite;

    private ParticleSystem[] particles;

    protected override void Awake()
    {
        base.Awake();

        isHeal = false;
        front.enabled = false;
        hpRegen = int.Parse(this.fieldStructureData.gimmickParam[0]);
        duration = int.Parse(this.fieldStructureData.gimmickParam[1]);
        tick = new WaitForSeconds(0.01f);
        sec = new WaitForSeconds(1.0f);

        sprite = front.GetComponent<SpriteRenderer>();
        sprite.enabled = false;

        particles = GetComponentsInChildren<ParticleSystem>();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.TryGetComponent(out Player player) && isHeal)
        {
            player.playerManager.playerData.CurrentHpModifier(hpRegen);
            isHeal = false;
        }
    }

    public IEnumerator Activation()
    {
        top.GetComponent<SpriteRenderer>().enabled = false;
        sprite.enabled = true;
        OnEffect();
        for (int i = 0; i < duration; i++)
        {
            front.enabled = true;
            isHeal = true;
            yield return tick;
            front.enabled = false;
            yield return sec;
        }

        top.GetComponent<SpriteRenderer>().enabled = true;
        sprite.enabled = false;
        OffEffect();
    }

    public void OnEffect()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }

    }

    public void OffEffect()
    {
        foreach (ParticleSystem particle in particles)
        {
            if (particle.isPlaying)
            {
                particle.Stop();
                particle.Clear();
            }
        }
    }
}
