using System;
using System.Collections;
using System.Collections.Generic;
#if MIRROR
using Mirror;
#endif
using UnityEngine;

#if MIRROR
public class DisableCameraOnSpawn : NetworkBehaviour
{
    [SerializeField] private Camera camera;


    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        Debug.Log("Start authority");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("Start local player");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Start server");
        if (!isServerOnly)
        {
            camera.gameObject.SetActive(false);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Start OnStartClient");
        camera.gameObject.SetActive(false);
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Start OnConnectedToServer");
        camera.gameObject.SetActive(false);
    }
}
#else
public class DisableCameraOnSpawn : MonoBehaviour
{
    [SerializeField] private Camera camera;
}
#endif
