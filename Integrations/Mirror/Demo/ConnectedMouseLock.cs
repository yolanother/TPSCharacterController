using System;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController;
using Mirror;
using UnityEngine;

public class ConnectedMouseLock : NetworkBehaviour
{
    [SerializeField] private CameraLockManager lockManager;

    private void Awake()
    {
        lockManager.Unlock();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if(!lockManager.IsLocked) lockManager.Lock();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if(!lockManager.IsLocked) lockManager.Lock();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && lockManager.IsLocked)
        {
            lockManager.Unlock();
        }
    }
}
