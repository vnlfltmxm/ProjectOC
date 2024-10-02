using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour,IItem
{
    protected new Rigidbody rigidbody;

    public virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public virtual void PickUpItem()
    {
        //플레이어 인벤에 추가하는 기능
    }

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
        rigidbody.useGravity = false;
        this.gameObject.transform.parent = playerHandTransform;
        this.gameObject.transform.position = playerHandTransform.position;
    }

    public virtual void DropItem()
    {
        this.gameObject.transform.parent = null;
        rigidbody.useGravity = true;
    }
}
