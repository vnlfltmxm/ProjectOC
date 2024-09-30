using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltCutter : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(other.TryGetComponent(out Fence f))
            {
                f.CutWire();
            }
            if(other.TryGetComponent(out FruitTree ft))
            {
                ft.RipFruit();
            }
            else
            {
                Debug.LogError("과일나무랑 접촉하지 않음");
            }
        }
    }
}
