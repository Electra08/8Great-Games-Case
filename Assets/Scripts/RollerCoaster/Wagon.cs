using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Wagon : MonoBehaviour,IChangeID
{
    public int Id = -1;
    [SerializeField] private float duration = 0.3f;
    private Queue<Transform> sittingPositions;
    
    private MeshRenderer _meshRenderer;
    private Material _material;
    private RollerCoaster _rollerCoaster;

    private void Awake()
    {
        _rollerCoaster = transform.parent.GetComponent<RollerCoaster>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.material;
        sittingPositions = new Queue<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            sittingPositions.Enqueue(transform.GetChild(i));
        }
    }

    private void ChangeColor(Color color)
    {
        _material.color = color;
    }
    
    public void ChangeId(int _id)
    {
        Id = _id;
        _meshRenderer.material = LevelManager.Instance.materials[_id];
    }
    
    public void SitToPos(StickMan stickMan)
    {
        Transform sitPos = sittingPositions.Dequeue();
        Vector3 targetPos = sitPos.localPosition;
        Vector3 middlePoint = (targetPos + stickMan.transform.localPosition)/2;
        middlePoint.y += 5;
        stickMan.transform.SetParent(sitPos.transform);
        
        Vector3[] pathPoints = new Vector3[]
        {
            stickMan.transform.localPosition,
            middlePoint,
            Vector3.zero,
        };
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(stickMan.transform.DOLocalPath(pathPoints, duration, PathType.CatmullRom).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            stickMan.HandleSit();
            ParticleManager.Instance.GetParticle(sitPos);
        }));
        sequence.Join(stickMan.transform.DOLocalRotate(Vector3.zero, duration*3).SetEase(Ease.Linear));
        sequence.OnComplete(() =>
        {
            stickMan.transform.localPosition = Vector3.zero;
            if (sittingPositions.Count == 0) _rollerCoaster.DoneWagon(gameObject);
        });
    }

    public bool IsHasSitPos()
    {
        return sittingPositions.Count > 0;
    }
    
    public void OnSelect()
    {
        ChangeColor(LevelManager.Instance.selectedColors[Id]);
    }
    
    public void OnDeSelect()
    {
        ChangeColor(LevelManager.Instance.normalColors[Id]);
    }
    
}