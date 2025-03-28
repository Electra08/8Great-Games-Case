using System;
using UnityEngine;

public class StickMan : MonoBehaviour,IChangeID
{ 
    public int Id = -1;
    
    private Animator _animator;
    private SkinnedMeshRenderer _meshRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }
    
    public void HandleSit()
    {
        _animator.SetTrigger("isSit");
    }

    public void ChangeId(int _id)
    {
        Id = _id;
        _meshRenderer.material = LevelManager.Instance.materials[_id];
    }
    
}