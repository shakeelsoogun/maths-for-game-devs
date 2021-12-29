using System.Linq;
using UnityEditor;
using UnityEngine;

public class AdaptiveFieldOfView : MonoBehaviour
{
    public GameObject pointsContainer;

    public void OnDrawGizmos()
    {
        Camera camera = GetComponent<Camera>();
        if (pointsContainer == null)
        {
            return;
        }
        var spheres = pointsContainer
            .GetComponentsInChildren<SphereCollider>()
            .Where(
                transform =>
                    !transform.gameObject.Equals(pointsContainer.gameObject)
            );

        var cameraDir = camera.transform.forward.normalized;
        var cameraPosition = camera.transform.position;

        // Use dot product to work out an approximation of the angle of points.
        // Lowest dot product => closest to 90deg => most out of focus
        var mostOutOfViewPoint = Vector3.zero;
        float currentMinDotProduct = 1f;
        foreach (var point in spheres)
        {
            var cameraToPointVec = point.transform.position - cameraPosition;
            // if (Mathf.Abs(cameraToPointVec.x) > Mathf.Abs(cameraToPointVec.y))
            // {
            //     var sign = cameraToPointVec.x >= 0 ? 1 : -1;
            //     cameraToPointVec.x +=
            //         point.radius * point.transform.localScale.x * sign;
            // }
            // else
            // {
            //     var sign = cameraToPointVec.y >= 0 ? 1 : -1;
            //     cameraToPointVec.y +=
            //         point.radius * point.transform.localScale.y * sign;
            // }
            Handles.Label(
                point.transform.position + Vector3.down,
                $"{cameraToPointVec}"
            );
            Gizmos.DrawSphere(cameraPosition + cameraToPointVec, 0.1f);
            var cross = Vector3.Cross(cameraToPointVec.normalized, cameraDir);
            var recross = Vector3.Cross(cross, cameraToPointVec.normalized);
            Gizmos.DrawRay(cameraPosition + cameraToPointVec, cross.normalized);
            Gizmos.color = Color.green;
            var zeFurthestPoint =
                cameraPosition + cameraToPointVec
                - (
                    recross.normalized
                    * point.transform.localScale.x
                    * point.radius
                );
            var dot = Vector3.Dot(
                (zeFurthestPoint - cameraPosition).normalized,
                cameraDir
            );
            Gizmos.DrawLine(cameraPosition + cameraToPointVec, zeFurthestPoint);
            Gizmos.color = Color.white;

            // If the dot product is <= 0, the point is behind the camera (or
            // completely perpendicular) and so would be impossible to see it
            // purely just by changing the camera FOV, so we ignore these
            if (
                mostOutOfViewPoint == null
                || (dot < currentMinDotProduct && dot > 0)
            )
            {
                mostOutOfViewPoint = zeFurthestPoint;
                currentMinDotProduct = dot;
            }
            Handles.Label(
                point.transform.position + Vector3.down * 2,
                $"{dot}"
            );
        }

        // If we don't have any viewable points, let's just opt out
        if (mostOutOfViewPoint == Vector3.zero)
        {
            camera.fieldOfView = 0;
            return;
        }

        // Make a triangle out of the camera position, the outermost point and
        // the point at which those form a right angle and then use that to
        // work out the angle
        var cameraToOutermostPointVec = mostOutOfViewPoint - cameraPosition;
        var hypotenuseLength = cameraToOutermostPointVec.magnitude;
        var adjacentLength = Vector3.Dot(cameraToOutermostPointVec, cameraDir);
        var angle = Mathf.Acos(adjacentLength / hypotenuseLength) * 2f;
        Handles.Label(mostOutOfViewPoint + Vector3.down * 3, $"{angle}");
        camera.fieldOfView = angle * Mathf.Rad2Deg;
        Gizmos.DrawLine(cameraPosition, mostOutOfViewPoint);
        Gizmos.DrawRay(cameraPosition, cameraDir * adjacentLength);
        Gizmos.DrawLine(
            cameraPosition + cameraDir * adjacentLength,
            mostOutOfViewPoint
        );
        Handles.DrawSolidArc(
            cameraPosition,
            Vector3.Cross(cameraDir, cameraToOutermostPointVec.normalized),
            cameraDir,
            angle * Mathf.Rad2Deg / 2,
            1f
        );
    }
}
