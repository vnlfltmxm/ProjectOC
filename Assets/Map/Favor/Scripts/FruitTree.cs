using FavorKim;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class FruitTree : MonoBehaviour
{
    [SerializeField] private List<GameObject> fruits = new List<GameObject>();
    private Queue<GameObject> fruitQueue;
    public static int callbackCount = 0;

    private void Start()
    {
        InitFruitQueue();
    }
    private void OnEnable()
    {
        RegistOnStartStage();
    }
    private void OnDisable()
    {
        UnRegistOnStartStage();
    }

    public void RegistOnStartStage()
    {
        EventManager.Instance.AddListenerAfter("OnStartStage", OnStartStage_FruitTree, "SetTerrainData");
    }
    public void UnRegistOnStartStage()
    {
        EventManager.Instance.RemoveListener("OnStartStage", OnStartStage_FruitTree);
    }

    private void OnStartStage_FruitTree()
    {
        if (callbackCount == 0)
        {
            Debug.Log("과일나무 온스타트 스테이지");
            callbackCount = 1;
        }
        if (fruitQueue == null)
        {
            InitFruitQueue();
        }
        ResetFruit();
    }

    private void ResetFruit()
    {
        foreach (var fruit in fruits)
        {
            if (fruit.activeSelf == false)
            {
                fruitQueue.Enqueue(fruit);
            }
        }
    }

    private void InitFruitQueue()
    {
        if (fruitQueue == null)
        {
            fruitQueue = new Queue<GameObject>();
            foreach (var fruit in fruits)
            {
                fruitQueue.Enqueue(fruit);
            }
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
