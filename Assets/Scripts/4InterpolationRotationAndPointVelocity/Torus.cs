using UnityEngine;

public class Torus : MonoBehaviour
{
    public int noOfTurns = 4;
    public float torusOverallRadius = 5f;
    public float height = 4f;
    public Color startColour = Color.red;
    public Color endColour = Color.cyan;

    private readonly int pointsPerTurn = 15;

    public void OnDrawGizmos()
    {
        int noOfPoints = pointsPerTurn * noOfTurns;
        Vector3[] points = new Vector3[noOfPoints];
        float innerCurlRadius = height / 2;

        for (int i = 0; i < noOfPoints; i++)
        {
            // Rotate once around to make the overall circle of the torus
            float torusAngleRad = i / (float)noOfPoints * Mathf.PI * 2;

            var torusY = transform.position.y;
            var torusX =
                transform.position.x
                + Mathf.Cos(torusAngleRad) * torusOverallRadius;
            var torusZ =
                transform.position.z
                + Mathf.Sin(torusAngleRad) * torusOverallRadius;
            var circlePoint = new Vector3(torusX, torusY, torusZ);

            // Make a rotation value that goes through fully per turn
            float curlAngleRad =
                i % pointsPerTurn / (float)pointsPerTurn * Mathf.PI * 2;

            // Use the tangent of the circle at the current point as the axis
            // for our rotation
            var circleTangent = Vector3.Cross(
                transform.up,
                circlePoint - transform.position
            );
            Gizmos.DrawRay(circlePoint, circleTangent.normalized);
            var rotation = Quaternion.AngleAxis(
                curlAngleRad * Mathf.Rad2Deg,
                circleTangent.normalized
            );

            // Rotate the up vector by our rotation and give it the correct radial distance to get the new point
            var point =
                circlePoint + (rotation * transform.up * innerCurlRadius);
            points[i] = point;
        }

        DrawShape(points);
    }

    public void DrawShape(Vector3[] points)
    {
        int noOfPoints = points.Length;
        for (int i = 0; i < noOfPoints; i++)
        {
            Vector3 firstPoint = points[i];
            Vector3 nextPoint = points[(i + 1) % noOfPoints];
            float t = i / (float)noOfPoints;
            Gizmos.color = LerpColour(startColour, endColour, t);
            Gizmos.DrawLine(firstPoint, nextPoint);
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
