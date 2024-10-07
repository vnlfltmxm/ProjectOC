using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemPoolmanger : Singleton<ItemPoolmanger>
{
    
    [SerializeField]
    private GameObject[] prefabs;

    private Dictionary<string, Dictionary<int, GameObject>> EquipmentPool = new Dictionary<string, Dictionary<int, GameObject>>();
    private Dictionary<string, Queue<GameObject>> FruitPool = new Dictionary<string, Queue<GameObject>>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitPool()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            
            //if (IsCheckItemTypeIsFruit(prefabs[i].name) == true)
            //{
            //    Queue<GameObject> tempQue = new Queue<GameObject>();
            //    FruitPool.Add(prefabs[i].name, tempQue);
            //}
            //else
            //{
            //    Dictionary<int, GameObject> tempDic = new Dictionary<int, GameObject>();

            //    EquipmentPool.Add(prefabs[i].name, tempDic);

            //}

            SetPool(i);
        }
    }

    public void SetPool(int prefabsIndex)
    {

        
        //if (IsCheckItemTypeIsFruit(prefabs[prefabsIndex].name) == true)
        //{
        //    for (int j = 0; j < DataManger.inst.GetItem(prefabs[prefabsIndex].name).count; j++)
        //    {
        //        GameObject item = Instantiate(prefabs[prefabsIndex]);
        //        item.SetActive(false);

        //        FruitPool[prefabs[prefabsIndex].name].Enqueue(item);

        //    }
        //}
        //else
        //{
        //    int index = 0;

        //    //for (int j = 0; j < DataManger.inst.GetItem(prefabs[prefabsIndex].name).count; j++)
        //    //{
        //    //    GameObject item = Instantiate(prefabs[prefabsIndex]);
        //    //    item.SetActive(false);

        //    //    int tempId = DataManger.inst.GetItem(prefabs[prefabsIndex].name).id + index;



        //    //    EquipmentPool[prefabs[prefabsIndex].name].Add(tempId, item);


        //    //    index++;
        //    //}
        //}


    }

    //public GameObject GetItemInPool(string itemName,int UID)
    //{
    //    //(Clone)날리는 부분 필요

    //    if (IsCheckItemTypeIsFruit(itemName) == true)
    //    {
    //        if (FruitPool.ContainsKey(itemName))
    //        {
    //            return FruitPool[itemName].Dequeue();
    //        }
    //    }
    //    else
    //    {
    //        if (EquipmentPool.ContainsKey(itemName))
    //        {
    //            if (EquipmentPool[itemName].ContainsKey(UID))
    //            {
    //                return EquipmentPool[itemName][UID];
    //            }
    //        }
    //    }

        

    //    return null;
    //}

    public void RemoveItem(string itemName, int UID)
    {
        if (EquipmentPool.ContainsKey(itemName))
        {
            if (EquipmentPool[itemName].ContainsKey(UID))
            {
                EquipmentPool[itemName].Remove(UID);
            }
            else
            {
                return;
            }
        }
    }

    
}
