using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    

    [SerializeField] private List<Transform> zones = new List<Transform>();
    [SerializeField] private List<Transform> areas = new List<Transform>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetTerrainData();
        }
    }


    private void SetTerrainData()
    {
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
