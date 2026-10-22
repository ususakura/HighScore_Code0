using UnityEngine;
using UnityEngine.InputSystem;

public class SlashDetector : MonoBehaviour
{
    private Vector3 previousMouseWorldPosition;
    private bool hasPreviousPosition;

    private void Update()
    {
        if (!TryGetMouseWorldPosition(out Vector3 currentMouseWorldPosition))
            return;

        if (!hasPreviousPosition)
        {
            previousMouseWorldPosition = currentMouseWorldPosition;
            hasPreviousPosition = true;
            return;
        }

        ProcessSlash(previousMouseWorldPosition, currentMouseWorldPosition);

        previousMouseWorldPosition = currentMouseWorldPosition;
    }

    private bool TryGetMouseWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;

        if (Mouse.current == null || Camera.main == null)
            return false;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0f)
        );

        worldPosition.z = 0f;

        return true;
    }

    private void ProcessSlash(
    Vector3 previousPosition,
    Vector3 currentPosition)
{
    RaycastHit2D hit = Physics2D.Linecast(
        previousPosition,
        currentPosition
    );

    if (hit.collider == null)
        return;

    HandleHit(hit.collider);
}
    private void HandleHit(Collider2D hitCollider)
{
    HeartTarget heart = hitCollider.GetComponent<HeartTarget>();

    if (heart == null)
        return;

    heart.Slice();
}
    private void OnEnable()
{
    hasPreviousPosition = false;
}
}