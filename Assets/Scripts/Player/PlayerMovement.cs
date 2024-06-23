using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform flipable;

    private PlayerManager playerManager;
    private Vector2 movementInput;
    private Rigidbody2D rb;
    private Camera cam;

    private float currentSpeed;
    private float currentStamina;
    private bool chargingStamina;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        currentSpeed = playerManager.WalkSpeed;
        currentStamina = playerManager.MaxStamina;
    }

    private void Update()
    {
        CalculateMovementInput();
        UpdateAnimator();
        Flip();
        HandleRunning();
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
        rb.velocity = movementInput * currentSpeed;
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

    private void HandleRunning()
    {
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && !chargingStamina)
        {
            currentSpeed = playerManager.RunSpeed;
            currentStamina -= Time.deltaTime * playerManager.StaminaSpeed;
        }
        else if (currentStamina < playerManager.MaxStamina)
        {
            currentStamina += Time.deltaTime * (playerManager.StaminaSpeed / 2);
            currentSpeed = playerManager.WalkSpeed;
        }

        if(currentStamina <= 0)
            chargingStamina = true;
        else if(currentStamina >= 10f && chargingStamina)
            chargingStamina = false;

        currentStamina = Mathf.Clamp(currentStamina, 0, playerManager.MaxStamina);

        playerManager.RunFov(Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && !chargingStamina);
    }

    public void SubtractCurrentStamina(float amount) { currentStamina -= amount; }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }
}
