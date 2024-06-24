using UnityEngine;

public class Parallax : MonoBehaviour
{
    [System.Serializable]
    public struct ParallaxLayer
    {
        public Transform transform;
        public float speed;
    }

    [SerializeField] private ParallaxLayer[] layers;
    [SerializeField] private float smooth = 1f;
    [SerializeField] private float maxDistance = 10f; // Dist�ncia m�xima para aplicar parallax

    private Vector3 previousCameraPosition;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        previousCameraPosition = mainCamera.transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 cameraDeltaMovement = mainCamera.transform.position - previousCameraPosition;
        previousCameraPosition = mainCamera.transform.position;

        for (int i = 0; i < layers.Length; i++)
        {
            Transform layerTransform = layers[i].transform;

            // Subtrai a posi��o Z da c�mera da posi��o Z da camada
            float distanceToCamera = Vector2.Distance(new Vector2(mainCamera.transform.position.x, mainCamera.transform.position.y),
                                                      new Vector2(layerTransform.position.x, layerTransform.position.y));

            if (distanceToCamera <= maxDistance)
            {
                float parallaxX = cameraDeltaMovement.x * layers[i].speed;
                float parallaxY = cameraDeltaMovement.y * layers[i].speed;
                Vector3 targetPosition = new Vector3(layerTransform.position.x + parallaxX, layerTransform.position.y + parallaxY, layerTransform.position.z);
                layerTransform.position = Vector3.Lerp(layerTransform.position, targetPosition, smooth * Time.deltaTime);
            }
        }
    }
}
