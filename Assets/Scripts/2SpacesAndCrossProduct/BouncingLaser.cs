using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class BouncingLaser : MonoBehaviour
{
    public float maxDistance = 200f;

    public void OnDrawGizmos()
    {
        // The cheating version using Vector3.Reflect, used to verify correctness
        // Draws a green laser showing the correct version, gets overridden by the red version
        // later where that version is correct
        var correctLaserStart = transform.position;
        var correctLaserDirection = transform.forward;
        var hitCorrectFound = true;
        while (hitCorrectFound)
        {
            hitCorrectFound = Physics.Raycast(
                correctLaserStart,
                correctLaserDirection,
                out RaycastHit hit,
                maxDistance
            );
            if (!hitCorrectFound)
            {
                Gizmos.DrawLine(
                    correctLaserStart,
                    correctLaserStart + correctLaserDirection * maxDistance
                );
                break;
            }
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(hit.point, hit.point + hit.normal);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(correctLaserStart, hit.point);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;
            Handles.Label(
                hit.point + Vector3.up * 0.3f,
                $"Hit point: {hit.point}",
                style
            );
            Handles.Label(
                hit.point + Vector3.up * 0.6f,
                $"Diff Vector: {correctLaserStart - hit.point}",
                style
            );
            Handles.Label(
                hit.point + Vector3.up * 0.9f,
                $"Direction: {(correctLaserStart - hit.point).normalized}",
                style
            );

            correctLaserStart = hit.point;
            correctLaserDirection =
                Vector3.Reflect(correctLaserDirection, hit.normal).normalized;
            Handles.Label(
                hit.point + Vector3.up * 1.2f,
                $"Reflecting Dir: {correctLaserDirection}",
                style
            );
        }

        Gizmos.color = Color.red;
        // Start at the object facing forward
        var currentLaserStart = transform.position;
        var currentLaserDirection = transform.forward;

        // Loop through and keep checking if the laser hits something.
        // If it does, then figure out the bouncing-off direction and use that as
        // the next `centre` and `direction`
        var hitFound = true;
        while (hitFound)
        {
            hitFound = Physics.Raycast(
                currentLaserStart,
                currentLaserDirection,
                out RaycastHit hit,
                maxDistance
            );
            if (!hitFound)
            {
                DrawLine(
                    currentLaserStart,
                    currentLaserStart + currentLaserDirection * maxDistance,
                    Color.red
                );
                break;
            }
            DrawLine(currentLaserStart, hit.point, Color.red);
            DrawLine(hit.point, hit.point + hit.normal, Color.cyan);

            // This is one way of working it out pretty quickly in 2 lines; the rest is just visual explanation.
            // We essentially find the vector to 2 * the dot product with the normal,
            // then just "displace" the vector to its correct starting point (at hit.point)
            // (look at the dotted line in the visual)
            // Reference: https://math.stackexchange.com/questions/13261/how-to-get-a-reflection-vector
            var theDot = Vector3.Dot(currentLaserDirection, hit.normal);
            // var reflectingVector = currentLaserDirection - 2 * theDot * hit.normal;


            // Some visualisation hopefully trying to explain what's going on (kinda)
            var backwardsVec = currentLaserStart - hit.point;
            var backwardsDirection = backwardsVec.normalized;
            var positionOfDotOnCurrentDir = hit.point + backwardsDirection;
            Gizmos.DrawSphere(positionOfDotOnCurrentDir, 0.05f);

            var positionOf2DotOnNormal = hit.point - 2 * theDot * hit.normal;
            Gizmos.DrawSphere(positionOf2DotOnNormal, 0.05f);
            // This vector is essentially our reflecting line! It just starts in the wrong place;
            // after we just move it to start from the hit point instead when actually getting it to work.
            Handles.DrawDottedLine(
                positionOfDotOnCurrentDir,
                positionOf2DotOnNormal,
                2f
            );

            // The second way of working it out. Instead of a point at 2 * dot * normal, we work out a
            // perpendicular line to the normal and double it to get to a point on the reflecting line,
            // then just work out the vector from hit.point to this line.
            // It does weirdly have some small margin of error that builds up with more reflections (after 3
            // you can see the lines are a little different). The first way doesn't have this margin of error.
            // (look at the magenta line for visual)
            // Reference diagram: http://paulbourke.net/geometry/reflected/
            Gizmos.DrawSphere(hit.point - theDot * hit.normal, 0.05f);
            DrawLine(hit.point, positionOfDotOnCurrentDir, Color.blue);

            var positionOfDotOnNormal = hit.point - theDot * hit.normal;
            var perpendicularVec =
                positionOfDotOnNormal - positionOfDotOnCurrentDir;
            var lengthOfPerpendicular = perpendicularVec.magnitude;
            var pointOnReflectingLine = (
                positionOfDotOnCurrentDir
                + perpendicularVec.normalized * 2 * lengthOfPerpendicular
            );
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pointOnReflectingLine, 0.05f);
            Gizmos.color = Color.red;
            var reflectingVector = pointOnReflectingLine - hit.point;

            DrawLine(
                positionOfDotOnCurrentDir,
                pointOnReflectingLine,
                Color.magenta
            );

            currentLaserDirection = reflectingVector.normalized;
            currentLaserStart = hit.point;
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
