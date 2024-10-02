using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    [SerializeField] GameObject Wire;
    [SerializeField] BoxCollider collision;
    public void CutWire()
    {
        Wire.SetActive(false);
        collision.enabled = false;
    }
    public void ResetFence()
    {
        Wire.SetActive(true);
        collision.enabled = true;
    }
}
