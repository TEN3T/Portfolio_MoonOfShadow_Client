using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChyuRyung : ActiveSkill
{
    private List<Vector2> pathList = new List<Vector2>();
    private LineRenderer lineRenderer;
    public ChyuRyung(int skillId, Transform shooter, int skillNum) : base(skillId, shooter, skillNum) { }
    public override IEnumerator Activation()
    {
        Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData, shooter);
        projectile.transform.position = (Vector2)shooter.position;

        lineRenderer = projectile.gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.red;
        float elapsedTime = 0.0f;
        while (true)
        {
            if (elapsedTime > 0.2f)
            {
                Vector2 path = shooter.transform.position;
                pathList.Add(path);
                elapsedTime = 0.0f;
                SkillManager.Instance.CoroutineStarter(Despawn());
            }
            if (pathList.Count > 3 && Vector2.Distance(pathList[0], shooter.transform.position) < 0.3f)
            {
                List<Vector2> fourPoints = new List<Vector2>
                {
                    pathList[0],
                    pathList[pathList.Count / 4],
                    pathList[(2 * pathList.Count) / 4],
                    pathList[pathList.Count - 1]
                };
                float area = CalculateMinimumArea(fourPoints.ToArray());
                Vector2 center = CalculateCentroid(fourPoints.ToArray());
                if (area > 0.1f)
                {
                    Debug.Log("소환!");
                    pathList.Clear();
                    if (area < 0.5f)
                    {
                        yield return Active(area + 0.5f, center);
                    }
                    else
                    {
                        yield return Active(area, center);
                    }
                }
            }
            UpdateLineRenderer();

            elapsedTime += Time.fixedDeltaTime;
            yield return frame;
        }
    }
    private void UpdateLineRenderer()
    {
        // 경로를 LineRenderer에 적용
        lineRenderer.positionCount = pathList.Count;
        for (int i = 0; i < pathList.Count; i++)
        {
            lineRenderer.SetPosition(i, pathList[i]);
        }
    }
    private float CalculateMinimumArea(Vector2[] polygon)
    {
        int n = polygon.Length;

        float area = 0f;
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            area += polygon[i].x * polygon[j].y;
            area -= polygon[j].x * polygon[i].y;
        }
        area = Mathf.Abs(area) * 0.5f;

        return area;
    }
    Vector2 CalculateCentroid(Vector2[] points)
    {
        if (points == null || points.Length == 0)
        {
            return Vector2.zero;
        }

        Vector2 centroid = Vector2.zero;

        foreach (Vector2 point in points)
        {
            centroid += point;
        }

        centroid /= points.Length;

        return centroid;
    }
    private IEnumerator Active(float area, Vector2 center)
    {
        Projectile projectile = SkillManager.Instance.SpawnProjectile<Projectile>(skillData);
        projectile.transform.localScale = Vector2.one * area;
        projectile.transform.localPosition = center;
        lineRenderer.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        SkillManager.Instance.DeSpawnProjectile(projectile);
        lineRenderer.gameObject.SetActive(true);
    }
    private IEnumerator Despawn()
    {
        if (pathList.Count > skillData.duration * 1000)
        {
            pathList.RemoveAt(0);
        }
        yield return frame;
    }
}