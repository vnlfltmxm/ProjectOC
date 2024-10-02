using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour,IItem
{
    public virtual void ChoiceItem(Transform playerHandTransform)
    {
        if (playerHandTransform == null) 
        { 
            return;
        }

        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
        }
        this.gameObject.transform.parent = playerHandTransform;
        this.gameObject.transform.position = playerHandTransform.position;
    }
}
