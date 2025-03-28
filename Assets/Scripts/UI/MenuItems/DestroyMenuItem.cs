using UnityEngine;

public class DestroyMenuItem : MenuItem
{
    public override void ToPlaceItem(RaycastHit _hit)
    {
        base.ToPlaceItem(_hit);
        
        Destroy(_hit.collider.gameObject);
    }
}
