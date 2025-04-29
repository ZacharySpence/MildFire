using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NodeVisualConnectorDrawer : MonoBehaviour
{
  
    public GameObject dashPrefab;        // Prefab of the dash (Image)
    public float dashLength = 10f;       // Length of each dash
    public float gapLength = 5f;         // Length of the gap between dashes

    // Bezier curve control points (for snaking effect)
    public Vector2 controlPoint1;        // First control point for the curve
    public Vector2 controlPoint2;        // Second control point for the curve


    public void Setup(RectTransform startPoint, RectTransform endPoint)
    {
      
        float distance = Vector2.Distance(startPoint.position, endPoint.position);
        Vector2 offset = new Vector2(0, distance * Random.Range(0.05f, 0.4f)); // 20% curve strength


        controlPoint1 = (Vector2)startPoint.position + offset;
        controlPoint2 = (Vector2)endPoint.position - offset;
        CreateDashedLine(startPoint,endPoint);
    }
    void CreateDashedLine(RectTransform startPoint, RectTransform endPoint)
    {
        // Get the distance and direction between the start and end points
        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        // Generate the Bezier curve positions
        List<Vector3> path = GenerateBezierCurve(start, end, controlPoint1, controlPoint2);

        // Create the dashes along the curve
        CreateDashesAlongPath(path);
    }

    // Generate points along a cubic Bezier curve
    List<Vector3> GenerateBezierCurve(Vector3 start, Vector3 end, Vector2 controlPoint1, Vector2 controlPoint2)
    {
        List<Vector3> curvePoints = new List<Vector3>();

        // Number of points to generate (the more points, the smoother the curve)
        int numPoints = 50;
        for (int i = 0; i <= numPoints; i++)
        {
            float t = i / (float)numPoints;

            // Calculate the Bezier curve position using the formula
            Vector3 point = Mathf.Pow(1 - t, 3) * start +
                            3 * Mathf.Pow(1 - t, 2) * t * (Vector3)controlPoint1 +
                            3 * (1 - t) * Mathf.Pow(t, 2) * (Vector3)controlPoint2 +
                            Mathf.Pow(t, 3) * end;

            curvePoints.Add(point);
        }

        return curvePoints;
    }

    // Create the dashes along the generated curve
    void CreateDashesAlongPath(List<Vector3> path)
    {
        float totalDistance = 0f;
        // Calculate the total path distance
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalDistance += Vector3.Distance(path[i], path[i + 1]);
        }

        float currentDistance = 0f;

        while (currentDistance < totalDistance)
        {
            Vector3 currentPosition = GetPositionAtDistance(path, currentDistance);
            Vector3 nextPosition = GetPositionAtDistance(path, currentDistance + 1f); // Small lookahead for direction

            Vector3 direction = (nextPosition - currentPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Instantiate the dash prefab
            GameObject dash = Instantiate(dashPrefab, transform);
            RectTransform dashRect = dash.GetComponent<RectTransform>();

            dashRect.position = currentPosition;
            dashRect.rotation = Quaternion.Euler(0f, 0f, angle);
            dashRect.sizeDelta = new Vector2(dashLength, dashRect.sizeDelta.y);

            currentDistance += dashLength + gapLength;
        }
    }

    // Get the position on the path at a specific distance
    Vector3 GetPositionAtDistance(List<Vector3> path, float distance)
    {
        float currentDistance = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(path[i], path[i + 1]);
            currentDistance += segmentLength;

            if (currentDistance >= distance)
            {
                // Return the point at the distance
                float segmentDistance = segmentLength - (currentDistance - distance);
                return Vector3.Lerp(path[i], path[i + 1], segmentDistance / segmentLength);
            }
        }

        return path[path.Count - 1]; // Return the last point if distance exceeds
    }
}

