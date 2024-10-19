using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRangeCircle : MonoBehaviour
{
    private float duration;

    public void Activation(float duration, float size)
    {
        transform.GetComponent<Renderer>().sortingLayerName = LayerConstant.SPAWNOBJECT.ToString();
        this.duration = duration <= 0.0f ? 1.0f : duration;
        this.transform.localScale = Vector3.one * size;
        SkillManager.Instance.CoroutineStarter(Circle());
    }

    private IEnumerator Circle()
    {
        yield return new WaitForSeconds(duration);
        this.transform.localScale = Vector3.zero;
        SkillManager.Instance.DeSpawnRangeCircle(this);
    }
}
