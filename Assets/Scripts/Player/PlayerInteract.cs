using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRadius = 5f;
    private IInteractable lastNearestInteractable = null;

    private void Update()
    {
        UpdateNearest();
    }

    private void UpdateNearest()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRadius);
        IInteractable nearestInteractable = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                float distanceToInteractable = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToInteractable < nearestDistance)
                {
                    nearestInteractable = interactable;
                    nearestDistance = distanceToInteractable;
                }
            }
        }

        if(lastNearestInteractable != null && lastNearestInteractable != nearestInteractable)
            lastNearestInteractable.IsNearest = false;

        if (nearestInteractable != null)
        {
            nearestInteractable.IsNearest = true;

            if (Input.GetKeyDown(KeyCode.E))
                nearestInteractable.Interact();
        }

        lastNearestInteractable = nearestInteractable;
    }
}
