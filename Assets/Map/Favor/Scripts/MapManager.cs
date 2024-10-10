using FavorKim;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    

    [SerializeField] private List<Transform> zones = new List<Transform>();
    [SerializeField] private List<Transform> areas = new List<Transform>();
    private void OnEnable()
    {
        OnStartStage_MapManager();
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveListener("OnStartStage", SetTerrainData);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            EventManager.Instance.InvokeEventHanler("OnStartStage");
        }
    }

    private void OnStartStage_MapManager()
    {
        EventManager.Instance.AddListener("OnStartStage", SetTerrainData, out bool already);
    }

    private void SetTerrainData()
    {
        Debug.Log("맵매니저 온 스타트 스테이지");
        List<int> indexes = new List<int>();
        Vector3 tempPos = new Vector3(0, 2.02f, 0);
        foreach (Transform z in zones)
        {
            int index = 0;
            while (true)
            {
                index = Random.Range(0, areas.Count);
                if (indexes.Contains(index))
                {
                    continue;
                }
                else
                {
                    indexes.Add(index);
                    break;
                }
            }
            z.SetParent(areas[index]);

            z.transform.localPosition = tempPos;
        }
    }
}
