using UnityEngine;

public class AmmoDrop : Collectable
{
    public override void OnCollect(Collider2D collision)
    {
        base.OnCollect(collision);
        collision.GetComponentInChildren<WeaponController>().AddInventoryAmmo(randValue);
    }

    public override bool CanCollect()
    {
        return true;
    }
}
