using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform flipable;

    private PlayerManager playerManager;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Camera cam;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Update()
    {
        CalculateMovementInput();
        UpdateAnimator();
        Flip();
    }

    private void FixedUpdate()
    {
        CalculateMovement();
    }

    private void CalculateMovementInput()
    {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput.Normalize();
    }

    private void CalculateMovement()
    {
        rb.velocity = movementInput * playerManager.MaxSpeed;
    }

    private void UpdateAnimator()
    {
        playerManager.anim.SetFloat("Speed", rb.velocity.magnitude);
    }

    private void Flip()
    {
        Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        direction.z = 0;

        Vector3 localScale = flipable.localScale;
        localScale.x = direction.x < 0 ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        flipable.localScale = localScale;
    }
}
