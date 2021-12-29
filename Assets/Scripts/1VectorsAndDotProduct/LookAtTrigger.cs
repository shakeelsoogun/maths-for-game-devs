using UnityEditor;
using UnityEngine;

// Assignment 2 from: https://docs.google.com/document/d/1Nou1ZAbNOkELggW9YqZ9LTRIW9Hc-isUpURbQ8gjiMM/edit
public class LookAtTrigger : MonoBehaviour
{
    [Range(0, 1)]
    public float sensitivity = 0.5f;

    public Transform viewObject;
    public Transform view;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(view.position, viewObject.position);
        Gizmos.DrawLine(viewObject.position, transform.position);
        // We normalize both vectors in this case because we wanted to use a scale of 0-1
        // If we wanted to do speed in a given direction (for example), we only normalize one of these.
        var directionToView = (view.position - viewObject.position).normalized;
        var triggerVec = (transform.position - viewObject.position).normalized;

        // The easy way to figure this out
        // var triggerDotProduct = Vector2.Dot(triggerVec, directionToView);

        // The manual calculation
        var triggerDotProduct =
            triggerVec.x * directionToView.x + triggerVec.y * directionToView.y;

        var isLookingAtTrigger = triggerDotProduct >= sensitivity;

        Handles.color = isLookingAtTrigger ? Color.green : Color.grey;
        Handles.Label(
            viewObject.position + Vector3.down,
            $"{triggerDotProduct}"
        );
        Handles.DrawWireDisc(transform.position, Vector3.forward, 1);
        // Handles.Button()
    }
}
