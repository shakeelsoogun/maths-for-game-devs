using UnityEngine;

public class Coil : MonoBehaviour
{
    public int noOfTurns = 4;
    public float height = 12f;
    public float radius = 5f;
    public Color startColour = Color.red;
    public Color endColour = Color.cyan;

    private readonly int pointsPerTurn = 360;

    public void OnDrawGizmos()
    {
        int noOfPoints = pointsPerTurn * noOfTurns;
        Vector3[] points = new Vector3[noOfPoints];
        float heightIncreasePerPoint = height / noOfPoints;
        float startHeight = transform.position.y - height / 2;

        for (int i = 0; i < noOfPoints; i++)
        {
            float currentHeight = i * heightIncreasePerPoint;
            float rotationAngleRad =
                i % pointsPerTurn / (float)pointsPerTurn * Mathf.PI * 2;
            var y = startHeight + currentHeight;
            var x = transform.position.x + Mathf.Cos(rotationAngleRad) * radius;
            var z = transform.position.z + Mathf.Sin(rotationAngleRad) * radius;
            points[i] = new Vector3(x, y, z);
        }

        DrawShape(points);
    }

    public void DrawShape(Vector3[] points)
    {
        int noOfPoints = points.Length;
        for (int i = 0; i < noOfPoints - 1; i++)
        {
            Vector3 startPoint = points[i];
            Vector3 endPoint = points[i + 1];
            float t = i / (float)noOfPoints;
            Gizmos.color = LerpColour(startColour, endColour, t);
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }

    public Color LerpColour(Color startColour, Color endColour, float t)
    {
        float r = Mathf.Lerp(startColour.r, endColour.r, t);
        float g = Mathf.Lerp(startColour.g, endColour.g, t);
        float b = Mathf.Lerp(startColour.b, endColour.b, t);
        float a = Mathf.Lerp(startColour.a, endColour.a, t);

        return new Color(r, g, b, a);
    }
}
