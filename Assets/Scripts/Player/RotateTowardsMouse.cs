using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    [Header("Rotate")]
    [SerializeField] private bool canRotate;
    [SerializeField] private Transform pivotRotation;

    [Header("Flip")]
    [SerializeField] private bool canFlipScale;
    [SerializeField] private bool canFlipRotation;
    [SerializeField] private Transform parentTransform;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (canRotate)
            UpdateRotation();

        if (canFlipScale)
            CalculateFlipScale();

        if (canFlipRotation)
            CalculateFlipRotation();
    }

    private void UpdateRotation()
    {
        Vector3 direction = cam.ScreenToWorldPoint(Input.mousePosition) - pivotRotation.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        pivotRotation.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void CalculateFlipScale()
    {
        Vector3 characterDirection = cam.ScreenToWorldPoint(Input.mousePosition) - parentTransform.position;
        characterDirection.z = 0;

        Vector3 localScale = pivotRotation.localScale;
        bool isCharacterFlipped = parentTransform.localScale.x < 0;

        //Flip X
        localScale.x = Mathf.Abs(localScale.x) * (isCharacterFlipped ? -1 : 1);

        //Flip Y
        if (isCharacterFlipped)
            localScale.y = Mathf.Abs(localScale.y) * (characterDirection.x > 0 ? 1 : -1);
        else
            localScale.y = Mathf.Abs(localScale.y) * (characterDirection.x < 0 ? -1 : 1);


        pivotRotation.localScale = localScale;
    }

    private void CalculateFlipRotation()
    {
        bool isCharacterFlipped = parentTransform.localScale.x < 0;
        Quaternion currentRotation = pivotRotation.rotation;

        if (isCharacterFlipped)
            pivotRotation.rotation = Quaternion.Euler(180, 180, currentRotation.eulerAngles.z);
        else
            pivotRotation.rotation = Quaternion.Euler(0, 0, currentRotation.eulerAngles.z);
    }


}
