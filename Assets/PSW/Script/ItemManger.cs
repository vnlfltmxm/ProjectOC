using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManger : Singleton<ItemManger>
{
    enum ItemTypeIndex
    {
        Equipment = 1,
        Fruit
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public bool IsCheckItemTypeIsFruit(string itemName)
    //{
    //    //string itemType = DataManger.inst.GetItem(itemName).type;

    //    //if (DataManger.inst.GetType(itemType).id == ItemTypeIndex.Fruit)
    //    //{
    //    //    return true;
    //    //}

    //    //return false;
    //}
}
