using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    [SerializeField] GameObject Wire;

    public void CutWire()
    {
        Wire.SetActive(false);
    }
}
