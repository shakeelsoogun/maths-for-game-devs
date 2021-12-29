using System.Linq;
using UnityEditor;
using UnityEngine;

public class TheTurret : MonoBehaviour
{
    public float gunHeight = 1.3f;
    public float barrelSeparation = 0.3f;
    public float barrelLength = 0.8f;

    public void OnDrawGizmos()
    {
        var isFacingSurface = Physics.Raycast(
            transform.position,
            transform.forward,
            out RaycastHit hit
        );
        if (!isFacingSurface)
        {
            DrawRelativeLine(transform.position, transform.forward, Color.red);
            return;
        }

        // Assignment part a: Draw the basis vectors for placing the turret on a surface
        DrawLine(transform.position, hit.point, Color.white);
        DrawRelativeLine(hit.point, hit.normal, Color.green);
        var vecFromPlayerToTurret = hit.point - transform.position;
        var tangentOfTurret =
            Vector3.Cross(
                vecFromPlayerToTurret.normalized,
                hit.normal
            ).normalized;
        DrawRelativeLine(hit.point, tangentOfTurret, Color.red);
        var correctedTurretDirection =
            Vector3.Cross(hit.normal, tangentOfTurret).normalized;
        DrawRelativeLine(hit.point, correctedTurretDirection, Color.cyan);

        // Assignment part b: Create a worldToLocalMatrix for the turret's placement so
        // that we can draw a cube wire mesh of its bounding box using local coords
        Vector4 xBasis = new Vector4(
            tangentOfTurret.x,
            tangentOfTurret.y,
            tangentOfTurret.z
        );
        Vector4 yBasis = new Vector4(hit.normal.x, hit.normal.y, hit.normal.z);
        Vector4 zBasis = new Vector4(
            correctedTurretDirection.x,
            correctedTurretDirection.y,
            correctedTurretDirection.z,
            0
        );
        Vector4 position = new Vector4(
            hit.point.x,
            hit.point.y,
            hit.point.z,
            1
        );
        Matrix4x4 transformMatrix = new Matrix4x4(
            xBasis,
            yBasis,
            zBasis,
            position
        );

        // Now draw the cube
        Vector3[] corners = new Vector3[]
        {
            // bottom 4 positions:
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1),
            new Vector3(-1, 0, -1),
            new Vector3(1, 0, -1),
            // top 4 positions:
            new Vector3(1, 2, 1),
            new Vector3(-1, 2, 1),
            new Vector3(-1, 2, -1),
            new Vector3(1, 2, -1),
        };
        Vector3[] cornersInWorldPositions = corners
            .Select(corner => transformMatrix.MultiplyPoint3x4(corner))
            .ToArray();
        DrawCube(cornersInWorldPositions);

        // Assignment part c: Draw lines representing the "guns" of the turret!
        Vector3[] barrelPoints = new Vector3[]
        {
            new Vector3(-barrelSeparation / 2, gunHeight, 0),
            new Vector3(-barrelSeparation / 2, gunHeight, barrelLength),
            new Vector3(barrelSeparation / 2, gunHeight, 0),
            new Vector3(barrelSeparation / 2, gunHeight, barrelLength),
        };
        Vector3[] barrelPointsInWorldPositions = barrelPoints
            .Select(corner => transformMatrix.MultiplyPoint3x4(corner))
            .ToArray();
        DrawLine(
            barrelPointsInWorldPositions[0],
            barrelPointsInWorldPositions[2],
            Color.magenta
        );
        DrawLine(
            barrelPointsInWorldPositions[0],
            barrelPointsInWorldPositions[1],
            Color.magenta
        );
        DrawLine(
            barrelPointsInWorldPositions[2],
            barrelPointsInWorldPositions[3],
            Color.magenta
        );
        DrawLine(hit.point, hit.point + hit.normal * gunHeight, Color.magenta);
    }

    void DrawRelativeLine(Vector3 from, Vector3 direction, Color color)
    {
        DrawLine(from, from + direction, color);
    }

    void DrawCube(Vector3[] corners)
    {
        var originalColor = Handles.color;
        Handles.color = Color.yellow;
        var halfPoint = corners.Length / 2;
        DrawFace(corners.Take(halfPoint).ToArray());
        DrawFace(corners.Skip(halfPoint).ToArray());

        for (int i = 0; i < halfPoint; i++)
        {
            var lineStart = corners[i];
            var lineEnd = corners[i + halfPoint];
            Handles.DrawLine(lineStart, lineEnd);
        }
        Handles.color = originalColor;
    }

    void DrawFace(Vector3[] faceVertices)
    {
        for (int i = 0; i < faceVertices.Length; i++)
        {
            var secondIndex = i + 1 >= faceVertices.Length ? 0 : i + 1;
            var lineStart = faceVertices[i];
            var lineEnd = faceVertices[secondIndex];
            Handles.DrawLine(lineStart, lineEnd);
        }
    }

    void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        var originalColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawLine(from, to);
        Gizmos.color = originalColor;
    }
}
