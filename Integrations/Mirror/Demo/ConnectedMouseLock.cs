using System;
using System.Collections;
using System.Collections.Generic;
using DoubTech.TPSCharacterController;
#if MIRROR
using Mirror;
#endif
using UnityEngine;

#if MIRROR
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
#else
public class ConnectedMouseLock : MonoBehaviour
{
    [SerializeField] private CameraLockManager lockManager;

    private void Awake() {
        throw new Exception("Mirror has not been added to project. Make sure MIRROR is in your project defines.");
    }
}
#endif