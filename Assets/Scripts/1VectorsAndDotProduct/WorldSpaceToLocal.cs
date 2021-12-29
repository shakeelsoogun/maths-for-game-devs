using UnityEditor;
using UnityEngine;

public class WorldSpaceToLocal : MonoBehaviour
{
    public Transform objectToTransform;

    public void OnDrawGizmos()
    {
        Handles.color = Color.grey;
        DrawXYGizmos(Vector3.zero, Vector3.up, Vector3.right);
        Handles.Label(
            Vector3.down * 0.3f + Vector3.left * 0.7f,
            "World space origin"
        );
        DrawXYGizmos(transform.position, transform.up, transform.right);

        var localisedWorldSpace = TransformWorldToLocal(
            objectToTransform.position
        );
        var nowBackToWorldSpace = TransformLocalToWorld(localisedWorldSpace);
        Handles.Label(
            objectToTransform.position + Vector3.down * 0.3f + Vector3.left,
            $"Original World Space {objectToTransform.position}"
        );
        Handles.Label(
            objectToTransform.position + Vector3.down * 0.6f + Vector3.left,
            $"Diff Vec {objectToTransform.position - transform.position}"
        );
        Handles.Label(
            objectToTransform.position + Vector3.down * 0.9f + Vector3.left,
            $"Local to {name} {localisedWorldSpace}"
        );
        Handles.Label(
            objectToTransform.position + Vector3.down * 1.2f + Vector3.left,
            $"Now back to world space {nowBackToWorldSpace}"
        );
        Handles.Label(
            transform.position + Vector3.down * 0.3f,
            $"transform.position - {transform.position}"
        );
        Handles.Label(
            transform.position + Vector3.down * 0.6f,
            $"transform.right - {transform.right}"
        );
        Handles.Label(
            transform.position + Vector3.down * 0.9f,
            $"transform.up - {transform.up}"
        );
    }

    Vector3 TransformWorldToLocal(Vector3 worldPosition)
    {
        // Get the offset between the worldPosition and the new origin (transform)
        var offsetVec = worldPosition - transform.position;

        // Goal: Use the difference between the points to realign worldPosition with the normal of the local axis
        // The World axis is: x is Vector3.right (1, 0, 0), y is Vector3.up (0, 1, 0)
        // Therefore for Local axis: x is transform.right, y is transform.up
        // (these are somewhat the same, but the `transform` versions applies the rotation of the object to these vectors)
        // => Use the dot product to project the difference to the local object space (x, y) axis
        var x =
            offsetVec.x * transform.right.x + offsetVec.y * transform.right.y; // Vector3.Dot(offsetVec, transform.right);
        var y = offsetVec.x * transform.up.x + offsetVec.y * transform.up.y; // Vector3.Dot(offsetVec, transform.up);
        return new Vector3(x, y, 0);
    }

    Vector3 TransformLocalToWorld(Vector3 localPosition)
    {
        // 1. Get the axis of the local space
        // 2. Multiply (scale) this by the localPosition
        // 3. Add it to the origin of the local space to transform it according to the world space origin
        var x = transform.right * localPosition.x;
        var y = transform.up * localPosition.y;
        return x + y + transform.position;
    }

    private void DrawXYGizmos(Vector3 point, Vector3 up, Vector3 right)
    {
        var originalColor = Gizmos.color;
        var handleOriginalColor = Handles.color;
        Gizmos.color = Handles.color = Color.green;
        Gizmos.DrawRay(point, up);
        Handles.Label(point + up, "y");
        Gizmos.color = Handles.color = Color.red;
        Gizmos.DrawRay(point, right);
        Handles.Label(point + right, "x");
        Gizmos.color = originalColor;
        Handles.color = handleOriginalColor;
    }
}
