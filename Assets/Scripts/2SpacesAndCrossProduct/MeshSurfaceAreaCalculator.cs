using System.Linq;
using UnityEditor;
using UnityEngine;

public class MeshSurfaceAreaCalculator : MonoBehaviour
{
    public void OnDrawGizmos()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        var triangles = GetStructuredTriangles(mesh);
        // Surface Area = Sum(area of each face)
        // Area of face = Sum(area of triangles that make up face), hence
        // summing the area of the triangles essentially gets the total
        // surface area
        float surfaceArea = triangles
            .Select(triangle => AreaOfTriangle(triangle))
            .Sum();
        Handles.Label(
            transform.position + Vector3.up * 1f,
            $"Area: {surfaceArea}m2"
        );
    }

    private float AreaOfTriangle(Vector3[] triangle)
    {
        // ||cross(a, b)|| = area of parallelogram of a and b = 2 * area of triangle
        // ||cross(a, b)|| / 2 = area of triangle
        var lineTo1 = triangle[1] - triangle[0];
        var lineTo2 = triangle[2] - triangle[0];
        var areaOfParallelogram = Vector3.Cross(lineTo1, lineTo2).magnitude;
        return areaOfParallelogram / 2;
    }

    private Vector3[][] GetStructuredTriangles(Mesh mesh)
    {
        // Converts the odd triangles single array into the Vector3[][] of
        // triangles to be more easily usable
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[][] structuredTriangles = new Vector3[triangles.Length / 3][];
        for (int i = 0; i < triangles.Length / 3; i++)
        {
            Vector3[] triangle = new Vector3[3];
            for (int j = 0; j < 3; j++)
            {
                triangle[j] = vertices[triangles[i * 3 + j]];
            }
            structuredTriangles[i] = triangle;
        }
        return structuredTriangles;
    }
}
