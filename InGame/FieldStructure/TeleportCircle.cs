using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCircle : FieldStructure
{
    [SerializeField] private TeleportCircle connectedTeleport;
    [SerializeField] private float coolTime = 2.0f;
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private float delayTime = 1.5f;

    private SpriteRenderer topSpriteRenderer;
    private SpriteRenderer frontSpriteRenderer;
    private int teleportCount;
    private bool available;
    private bool isEffect;
    private SoundRequester soundRequester;

    private ParticleSystem[] particles;

    protected override void Awake()
    {
        base.Awake();

        teleportCount = int.Parse(this.fieldStructureData.gimmickParam[0]);
        available = true;
        topSpriteRenderer = top.GetComponent<SpriteRenderer>();
        frontSpriteRenderer = front.GetComponent<SpriteRenderer>();
        soundRequester = GetComponent<SoundRequester>();
        frontSpriteRenderer.enabled = false;
        isEffect = false;

        particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        transform.position -= new Vector3(0.0f, 0.00005f, 0.0f);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (available)
        {
            if (collision.transform.parent.TryGetComponent(out Player player))
            {
                if (soundRequester != null) { 
                    soundRequester.ChangeSituation(SoundSituation.SOUNDSITUATION.ACTIVE);
                }
                StartCoroutine(Activation(player));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isEffect)
        {
            if (collision.transform.parent.TryGetComponent(out Player player))
            {
                OffEffect();
            }
        }
    }

    private IEnumerator Activation(Player player)
    {
        available = false;
        OnEffect();
        connectedTeleport.OnEffect();

        float time = 0.0f;
        while (time < delayTime)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            if (!isEffect)
            {
                available = true;
                OffEffect();
                connectedTeleport.OffEffect();
                yield break;
            }
        }

        player.transform.localPosition = connectedTeleport.transform.position + new Vector3(0.0f, 0.25f, 0.0f);
        StartCoroutine(connectedTeleport.CoolTime());
        StartCoroutine(CoolTime());
    }

    public IEnumerator CoolTime()
    {
        available = false;
        teleportCount--;
        isEffect = false;
        yield return new WaitForSeconds(delayTime * 0.5f);
        topSpriteRenderer.enabled = false;
        frontSpriteRenderer.enabled = true;
        OffEffect();
        connectedTeleport.OffEffect();
        yield return new WaitForSeconds(coolTime);
        if (teleportCount > 0)
        {
            available = true;
            topSpriteRenderer.enabled = true;
            frontSpriteRenderer.enabled = false;
        }
    }

    public void OnEffect()
    {
        isEffect = true;
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
            StartCoroutine(RotateParticle(particle));
        }

    }

    private IEnumerator RotateParticle(ParticleSystem particle)
    {
        while (particle.isPlaying)
        {
            particle.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f) * Time.fixedDeltaTime * rotateSpeed);
            yield return new WaitForFixedUpdate();
        }
    }

    public void OffEffect()
    {
        isEffect = false;
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
