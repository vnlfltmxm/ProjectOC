using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private TerrainData hardZoneTerrainData;
    [SerializeField] private TerrainData softZoneTerrainData;
    [SerializeField] private TerrainData dateZoneTerrainData;

    [SerializeField] private List<Terrain> terrains = new List<Terrain>(6);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SetTerrainData();
        }
    }


    private void SetTerrainData()
    {
        int softCount = 2;
        int hardCount = 2;
        int dateCount = 2;
        for(int i = 0; i < terrains.Count; i++) 
        {
            TerrainData data = null;

            while (softCount + hardCount + dateCount > 0)
            {
                int temp = Random.Range(0, 3);
                
                switch (temp)
                {
                    case 0:
                        if (softCount == 0) continue;
                        softCount--;
                        data = softZoneTerrainData;
                        break;
                    case 1:
                        if (hardCount == 0) continue;
                        hardCount--;
                        data = hardZoneTerrainData;
                        break;
                    case 2:
                        if (dateCount == 0) continue;
                        dateCount--;
                        data = dateZoneTerrainData;
                        break;
                    default:
                        continue;
                }
                
                terrains[i].terrainData = data;
                terrains[i].GetComponent<TerrainCollider>().terrainData = terrains[i].terrainData;
                break;
            }


        }
    }
}
