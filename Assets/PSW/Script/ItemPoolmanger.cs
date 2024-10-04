using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoolmanger : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabs;

    private Dictionary<string, Dictionary<int, GameObject>> itemPool = new Dictionary<string, Dictionary<int, GameObject>>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetPool()
    {
        //for (int i = 0; i < prefabs.Length; i++)
        //{
        //    int index = 0;
        //    for (int j = 0; j < DataManger.inst.GetItem(prefabs[i].name).count; j++)
        //    {
        //        GameObject item = Instantiate(prefabs[i]);
        //        item.SetActive(false); 

        //        int tempId = DataManger.inst.GetItem(prefabs[i].name).id + index;

        //        if (itemPool.ContainsKey(prefabs[i].name) == false) 
        //        {
        //            Dictionary<int, GameObject> tempDic = new Dictionary<int, GameObject>();
        //            tempDic.Add(tempId, item);
        //            itemPool.Add(prefabs[i].name, tempDic);
        //        }
        //        else
        //        {
        //            itemPool[prefabs[i].name].Add(tempId, item);
        //        }

        //        index++;
        //    }
        //}
    }

    public GameObject GetItemInPool(string itemName,int UID)
    {
        if (itemPool.ContainsKey(itemName))
        {
            if (itemPool[itemName].ContainsKey(UID))
            {
                return itemPool[itemName][UID];
            }
        }

        return null;
    }

    public void RemoveItem(string itemName, int UID)
    {
        if (itemPool.ContainsKey(itemName))
        {
            if (itemPool[itemName].ContainsKey(UID))
            {
                itemPool[itemName].Remove(UID);
            }
            else
            {
                return;
            }
        }
    }
}
