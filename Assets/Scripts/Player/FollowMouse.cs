using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private GameObject childObject;
    [SerializeField] private float maxDistanceX = 10f;
    [SerializeField] private float maxDistanceY = 10f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        float distanceX = Mathf.Abs(mousePosition.x - transform.position.x);
        float distanceY = Mathf.Abs(mousePosition.y - transform.position.y);

        Vector3 newPosition = mousePosition;

        if (distanceX > maxDistanceX)
        {
            float signX = Mathf.Sign(mousePosition.x - transform.position.x);
            newPosition.x = transform.position.x + signX * maxDistanceX;
        }

        if (distanceY > maxDistanceY)
        {
            float signY = Mathf.Sign(mousePosition.y - transform.position.y);
            newPosition.y = transform.position.y + signY * maxDistanceY;
        }

        childObject.transform.position = newPosition;
    }
}
