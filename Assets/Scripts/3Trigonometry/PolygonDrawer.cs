using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PolygonDrawer : MonoBehaviour
{
    [Range(3, 100)]
    public int noOfSides = 3;
    [Range(0, 100)]
    public float radius = 1f;
    public int density = 1;

    public void OnValidate()
    {
        if (noOfSides < 3)
        {
            noOfSides = 3;
        }

        if (radius < 0)
        {
            radius = 0;
        }
    }

    public void OnDrawGizmos()
    {
        //// Part 7A & 7B: Draw the polygon, using density to decide how many
        //// lines to skip over when drawing the shape

        // First work out all the points of the polygon
        var points = new Vector2[noOfSides];
        for (int i = 0; i < noOfSides; i++)
        {
            var angleRad = (float)Math.PI * 2 * i / (float)noOfSides;
            var x = radius * Mathf.Cos(angleRad) + transform.position.x;
            var y = radius * Mathf.Sin(angleRad) + transform.position.y;
            points[i] = new Vector2(x, y);
        }

        // Draw the points as spheres then draw the lines to the next point
        // Density is used as an offset (using modulo points.Length so we can
        // cycle through all the points)
        for (int i = 0; i < points.Length; i++)
        {
            var currentPoint = points[i];
            Gizmos.DrawSphere(currentPoint, 0.1f);
            var nextPoint = points[(i + density) % points.Length];
            Gizmos.DrawLine(currentPoint, nextPoint);
        }

        //// Optional extra: Calculate the area of the polygon

        // Firstly, if density is the same as noOfSides, opt out because this isn't even a shape
        if (density % noOfSides == 0)
        {
            Handles.Label(
                transform.position + Vector3.down * (radius / 10f),
                $"0m2"
            );
            return;
        }

        // We split the polygon into triangles from the first point to
        // (n + 1) and (n + 2) for each other point. We then calculate the
        // area of each of these triangles to work out the whole polygon
        var firstPoint = points[0];
        var totalPolygonArea = 0f;
        for (int i = 1; i < points.Length - 1; i++)
        {
            var trianglePoint2 = points[i];
            var trianglePoint3 = points[i + 1];
            // Visualise the inner polygon triangles
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(firstPoint, trianglePoint2);
            Gizmos.DrawLine(firstPoint, trianglePoint3);

            // We use the dot product to break each triangle into two
            // right-angled triangles - we can also use that point to work out
            // the base of both triangles and then
            var halfPoints = points.Length / 2f;
            var pointThatFormsRightAngle =
                (i >= halfPoints) ? trianglePoint3 : trianglePoint2;
            var connectingPoint =
                (i >= halfPoints) ? trianglePoint2 : trianglePoint3;
            var longestSideVec = firstPoint - connectingPoint;
            var adjacentVec = connectingPoint - pointThatFormsRightAngle;
            var triangle1Base = Vector2.Dot(
                longestSideVec.normalized,
                adjacentVec
            );
            var triangle2Base = longestSideVec.magnitude - triangle1Base;
            var directionToFirstPoint =
                (connectingPoint - firstPoint).normalized;
            var basePoint =
                connectingPoint + directionToFirstPoint * triangle1Base;
            Handles.color = Color.red;
            Handles.DrawDottedLine(
                pointThatFormsRightAngle,
                basePoint,
                radius / 2f
            );
            var triangleHeight =
                (basePoint - pointThatFormsRightAngle).magnitude;
            var triangle1Area = triangle1Base * triangleHeight / 2;
            var triangle2Area = triangle2Base * triangleHeight / 2;
            totalPolygonArea += triangle1Area + triangle2Area;
        }

        // For densities > 1, the shape isn't quite so normal and we now have
        // isoceles triangle cut-outs for the shape on the outside, so we work
        // this out and subtract it from the whole
        var angleBetweenPoints = (float)Math.PI * 2 / noOfSides;
        var halfWay = Mathf.FloorToInt(noOfSides / 2f);
        var normalizedDensity = density % noOfSides;
        var stepsIn =
            normalizedDensity > halfWay
                ? noOfSides - normalizedDensity
                : normalizedDensity;

        float angleOfOuterTriangle = 0;
        float totalOuterTriangleArea = 0f;
        if (stepsIn > 0)
        {
            // 2 -> 1 / 4 = 0.25f
            // 3 -> 2 / 6 = 0.33f
            // 4 -> 3 / 8 = 0.375f
            // 5 -> 4 / 10 = 0.4f
            // 6 -> 5 / 12 = 0.4166f
            // Weirdest number sequence ever, but (n - 1) / 2n is the key!
            angleOfOuterTriangle =
                (stepsIn - 1) / (stepsIn * 2f) * stepsIn * angleBetweenPoints;

            // All points are equidistant, so use one and multiply the area for the rest
            var diffVec = points[0] - points[1];
            var outerTriangleHeight =
                Mathf.Tan(angleOfOuterTriangle) * diffVec.magnitude / 2;
            var outerTriangleArea = diffVec.magnitude * outerTriangleHeight / 2;
            totalOuterTriangleArea = outerTriangleArea * (noOfSides - 1);
        }

        // Visualise the triangle cut out angles so its possible to see what we're talking about
        for (var i = 0; i < points.Length; i++)
        {
            // Draw the outer triangle angle
            var currentPoint = points[i];
            var adjacentPoint = points[(i + 1) % points.Length];
            var diffVec = adjacentPoint - currentPoint;
            Handles.color = Color.white;
            Handles.DrawSolidArc(
                currentPoint,
                Vector3.forward,
                diffVec,
                angleOfOuterTriangle * Mathf.Rad2Deg,
                diffVec.magnitude * 0.3f
            );
            Handles.Label(
                currentPoint + Vector2.down * (radius / 10f),
                $"{angleOfOuterTriangle * Mathf.Rad2Deg}"
            );
        }

        // Finally, we present the overall area
        var overallArea = totalPolygonArea - totalOuterTriangleArea;
        Handles.Label(
            transform.position + Vector3.down * (radius / 10f),
            $"{overallArea}m2"
        );
    }
}
