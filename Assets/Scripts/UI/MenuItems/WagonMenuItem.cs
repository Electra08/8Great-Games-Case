using DG.Tweening;
using UnityEngine;

public class WagonMenuItem : MenuItem
{
    [SerializeField] private GameObject objPrefab;
    [SerializeField] private GameObject selectedWagonHead;

    private GameObject lastWagon;
    private GameObject obj;
    
    public override void OnSelect()
    {
        base.OnSelect();
        obj = Instantiate(objPrefab, new Vector3(99,99,99), objPrefab.transform.rotation);
        
        if(selectedWagonHead != null) return;

        var wagonHeads = GameObject.FindGameObjectsWithTag("WagonHead");
        foreach (var wagonHead in wagonHeads)
        {
            wagonHead.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    public override void OnDeSelect()
    {
        base.OnDeSelect();
        Destroy(obj);
        
        var wagonHeads = GameObject.FindGameObjectsWithTag("WagonHead");
        foreach (var wagonHead in wagonHeads)
        {
            wagonHead.GetComponent<MeshRenderer>().material.color = Color.blue;
        }

        selectedWagonHead = null;
    }

    public override void ToPlaceItem(RaycastHit _hit)
    {
        base.ToPlaceItem(_hit);
        
        if(selectedWagonHead == null && _hit.collider.tag == "WagonHead")
        {
            selectedWagonHead = _hit.collider.gameObject;
            
            var wagonHeads = GameObject.FindGameObjectsWithTag("WagonHead");
            foreach (var wagonHead in wagonHeads)
            {
                wagonHead.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            
            return;
        }
        
        if(selectedWagonHead == null) return;
        
        if(_hit.collider.tag != "Grid" && lastWagon != null)
        {
            lastWagon.transform.position = _hit.transform.position;
            lastWagon = null;
            return;
        }
        
        ItemParent = selectedWagonHead.transform.parent;
        GameObject wagonObj = Instantiate(ItemPrefab,ItemParent.transform);
        wagonObj.transform.position = _hit.transform.position;
        wagonObj.transform.rotation = obj.transform.rotation;
        selectedWagonHead.GetComponentInParent<RollerCoaster>().ResetAllSettings();
        lastWagon = wagonObj;
    }

    public override void ToPlacing(RaycastHit _hit)
    {
        base.ToPlacing(_hit);
        
        if(selectedWagonHead == null) return;
        
        if(_hit.collider.tag != "Grid")
        {
            obj.transform.position = new Vector3(99, 99, 99);
            return;
        }
        
        obj.transform.position = _hit.transform.position;
    }

    public override void RotateObj()
    {
        base.RotateObj();

        var rot = obj.transform.eulerAngles;
        rot.y += 90;
        obj.transform.DORotate(rot, 0.1f);
    }
}