using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Santuary : FieldStructure
{
    private const string SANTUARY_CIRCLE_PATH = "Prefabs/InGame/FieldStructure/FieldStructure_SantuaryCircle";

    [SerializeField] float speed = 5.0f;

    private bool isActive;
    private float diffCount;
    private int needKillCount;
    private int currentKillCount;
    private float killCountRatio;
    private Vector3 beadSize;

    private SantuaryCircle santuaryCircle;

    protected override void Awake()
    {
        base.Awake();

        isActive = false;
        needKillCount = int.Parse(this.fieldStructureData.gimmickParam[0]);
        beadSize = top.transform.localScale;
        santuaryCircle = Instantiate(ResourcesManager.Load<SantuaryCircle>(SANTUARY_CIRCLE_PATH), transform);
        santuaryCircle.transform.localPosition = new Vector3(0.0f, -7.5f, 0.0f);
    }

    private void Update()
    {
        if (!isActive)
        {
            StartCoroutine(Activation());
        }

        //top.transform.localScale = Vector2.one * (diffCount / needKillCount);
        killCountRatio = diffCount / needKillCount;
        if (killCountRatio > 1.0f)
        {
            killCountRatio = 1.0f;
        }
        top.transform.localScale = Vector2.Lerp(top.transform.localScale, beadSize * killCountRatio, Time.deltaTime * speed);
    }

    private IEnumerator Activation()
    {
        isActive = true;
        WaitForFixedUpdate tick = new WaitForFixedUpdate();
        currentKillCount = GameManager.Instance.killCount;
        //while (GameManager.Instance.killCount - currentKillCount < needKillCount)
        //{
        //    yield return tick;
        //}
        do
        {
            diffCount = GameManager.Instance.killCount - currentKillCount;
            yield return tick;
        } while (diffCount < needKillCount);

        yield return santuaryCircle.Activation();

        isActive = false;
    }
}
