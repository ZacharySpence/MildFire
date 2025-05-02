using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NodeVisualConnectorDrawer : MonoBehaviour
{
    public GameObject dashPrefab;
    public float dashLength = 10f;
    public float gapLength = 5f;

    private List<GameObject> activeDashes = new List<GameObject>();

    public void Clear()
    {
        foreach (var dash in activeDashes)
        {
            Destroy(dash);
        }
        activeDashes.Clear();
    }

    public void DrawConnection(RectTransform startPoint, RectTransform endPoint)
    {
        // Control points for Bezier
        float distance = Vector2.Distance(startPoint.position, endPoint.position);
        Vector2 offset = new Vector2(0, distance * Random.Range(0.05f, 0.4f));
        Vector2 controlPoint1 = (Vector2)startPoint.position + offset;
        Vector2 controlPoint2 = (Vector2)endPoint.position - offset;

        // Generate Bezier points
        List<Vector3> path = GenerateBezierCurve(startPoint.position, endPoint.position, controlPoint1, controlPoint2);

        float totalDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++)
            totalDistance += Vector3.Distance(path[i], path[i + 1]);

        float currentDistance = 0f;
        while (currentDistance < totalDistance)
        {
            Vector3 currentPosition = GetPositionAtDistance(path, currentDistance);
            Vector3 nextPosition = GetPositionAtDistance(path, currentDistance + 1f);
            Vector3 direction = (nextPosition - currentPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject dash = Instantiate(dashPrefab, transform);
            RectTransform dashRect = dash.GetComponent<RectTransform>();
            dashRect.position = currentPosition;
            dashRect.rotation = Quaternion.Euler(0f, 0f, angle);
            dashRect.sizeDelta = new Vector2(dashLength, dashRect.sizeDelta.y);

            activeDashes.Add(dash);
            currentDistance += dashLength + gapLength;
        }
    }

    private List<Vector3> GenerateBezierCurve(Vector3 start, Vector3 end, Vector2 cp1, Vector2 cp2)
    {
        List<Vector3> points = new List<Vector3>();
        int steps = 50;

            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector3 point = Mathf.Pow(1 - t, 3) * start +
                                3 * Mathf.Pow(1 - t, 2) * t * (Vector3)cp1 +
                                3 * (1 - t) * Mathf.Pow(t, 2) * (Vector3)cp2 +
                                Mathf.Pow(t, 3) * end;
                points.Add(point);
            }

        return points;
    }

    private Vector3 GetPositionAtDistance(List<Vector3> path, float distance)
    {
        float currentDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(path[i], path[i + 1]);
            currentDistance += segmentLength;

            if (currentDistance >= distance)
            {
                float segmentDistance = segmentLength - (currentDistance - distance);
                return Vector3.Lerp(path[i], path[i + 1], segmentDistance / segmentLength);
            }
        }

        return path[^1];
    }
}

