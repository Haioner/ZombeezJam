using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    private Transform characterTransform;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        characterTransform = transform.parent.transform.parent.transform.parent;
    }

    private void Update()
    {
        UpdateRotation();
        CalculateFlip();
    }

    private void UpdateRotation()
    {
        Vector3 direction = cam.ScreenToWorldPoint(Input.mousePosition) - transform.parent.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.parent.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void CalculateFlip()
    {
        Vector3 characterDirection = cam.ScreenToWorldPoint(Input.mousePosition) - characterTransform.position;
        characterDirection.z = 0;

        Vector3 localScale = transform.parent.localScale;
        bool isCharacterFlipped = characterTransform.localScale.x < 0;

        //Flip X
        localScale.x = Mathf.Abs(localScale.x) * (isCharacterFlipped ? -1 : 1);

        //Flip Y
        if (isCharacterFlipped)
            localScale.y = Mathf.Abs(localScale.y) * (characterDirection.x > 0 ? 1 : -1);
        else
            localScale.y = Mathf.Abs(localScale.y) * (characterDirection.x < 0 ? -1 : 1);


        transform.parent.localScale = localScale;
    }

}
