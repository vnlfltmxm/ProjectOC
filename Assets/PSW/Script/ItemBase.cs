using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour,IItem
{
    protected int ItemUID;
    protected new Rigidbody rigidbody;

    public virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void SetItemUID(int itemUID)
    {
        ItemUID = itemUID;
    }

    public virtual void PickUpItem()
    {
        //�÷��̾� �κ��� �߰��ϴ� ���
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
