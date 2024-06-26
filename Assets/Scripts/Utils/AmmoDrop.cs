using UnityEngine;

public class AmmoDrop : Collectable
{
    public override void RandAmmoAmount()
    {
        base.RandAmmoAmount();

        randValue = (int)Random.Range(minMaxRandomValue.x, minMaxRandomValue.y + GameManager.instance.CurrentRoom);

        if (worldText != null)
            worldText.text = randValue.ToString("F0");
    }

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
