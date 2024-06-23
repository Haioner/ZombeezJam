using UnityEngine;

public class Medkit : Collectable
{
    public override void OnCollect(Collider2D collision)
    {
        collision.GetComponent<HealthController>().Heal(randValue);
    }

    public override bool CanCollect()
    {
        HealthController healthController = objectCollider.GetComponent<HealthController>();
        return healthController.GetCurrentHealth() < healthController.GetMaxHealth();
    }
}
