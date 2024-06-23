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
            float parallaxX = cameraDeltaMovement.x * layers[i].speed;
            float parallaxY = cameraDeltaMovement.y * layers[i].speed;
            Vector3 targetPosition = new Vector3(layerTransform.position.x + parallaxX, layerTransform.position.y + parallaxY, layerTransform.position.z);
            layerTransform.position = Vector3.Lerp(layerTransform.position, targetPosition, smooth * Time.deltaTime);
        }
    }
}
