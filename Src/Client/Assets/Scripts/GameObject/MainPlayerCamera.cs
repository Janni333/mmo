using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    public Camera playerCamera;
    public GameObject player;
    void Update()
    {
        if (player != null)
        {
            this.transform.position = player.transform.position;
            this.transform.rotation = player.transform.rotation;
        }
    }
}
