using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitTree : MonoBehaviour
{
    [SerializeField] private List<GameObject> fruits = new List<GameObject>();
    private Queue<GameObject> fruitQueue = new Queue<GameObject>();


    private void Start()
    {
        InitFruitQueue();
    }

    private void InitFruitQueue()
    {
        foreach(var fruit in fruits)
        {
            fruitQueue.Enqueue(fruit);
        }
    }
    public void RipFruit()
    {
        if (fruitQueue.Count == 0) 
        {
            Debug.LogError("딸 과일이 없음");
            return; 
        }
        else
        {
            //fruits
            var fruit = fruitQueue.Dequeue();
            fruit.gameObject.SetActive(false);
        }
    }
}
