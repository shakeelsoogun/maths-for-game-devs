using UnityEditor;
using UnityEngine;

// Assignment 1 from: https://docs.google.com/document/d/1Nou1ZAbNOkELggW9YqZ9LTRIW9Hc-isUpURbQ8gjiMM/edit
public class RadialTrigger : MonoBehaviour
{
    [Range(0, 5)]
    public float radius = 1;
    public Transform objectPosition;

    public void OnDrawGizmos()
    {
        // 3 ways to figure out distance
        // 1. The simplest; using Vector2.Distance (does all the work for us)
        // var distance = Vector2.Distance(objectPosition.position, transform.position);

        var diffVector = transform.position - objectPosition.position;
        // 2. Half-way house; getting the `.magnitude` property (length) of the difference vector
        // var distance = diffVector.magnitude;

        // 3. Fully manual; doing the full calculation
        var distance = Mathf.Sqrt(
            diffVector.x * diffVector.x + diffVector.y * diffVector.y
        );

        var isInside = distance < radius;

        Gizmos.color = isInside ? Color.green : Color.red;
        Gizmos.DrawLine(objectPosition.position, transform.position);
        Handles.Label(
            (objectPosition.position - transform.position) / 2
                + transform.position
                + Vector3.right / 2,
            $"{distance}"
        );

        Handles.color = isInside ? Color.red : Color.grey;
        Handles.DrawWireDisc(transform.position, Vector3.forward, radius);
    }
}
