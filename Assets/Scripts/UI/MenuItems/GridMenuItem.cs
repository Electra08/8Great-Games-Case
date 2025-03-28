using UnityEngine;

public class GridMenuItem : MenuItem
{
    [SerializeField] private GameObject gridPlaces;

    public override void OnSelect()
    {
        base.OnSelect();
        gridPlaces.SetActive(true);
    }

    public override void OnDeSelect()
    {
        base.OnDeSelect();
        gridPlaces.SetActive(false);
    }

    public override void ToPlaceItem(RaycastHit _hit)
    {
        if(_hit.collider.tag != "Grid_Place") return;

        GameObject obj = Instantiate(ItemPrefab, _hit.collider.transform.position, ItemPrefab.transform.rotation);
        obj.transform.SetParent(ItemParent);
    }
}