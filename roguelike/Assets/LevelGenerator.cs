using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform tilesParent;
    public Transform grid;
    public GameObject floorTilePrefab;

    public void Init()
    {
        SpawnFloorTiles();
    }

    void SpawnFloorTiles()
    {
        Vector3 newPos = Vector3.zero;
        GameObject go;

        int tilesAmount = Mathf.RoundToInt(grid.transform.localScale.x * grid.transform.localScale.z);

        for (int x = 0; x < grid.transform.localScale.x; x ++)
        {
            for (int z = 0; z < grid.transform.localScale.z; z++)
            {
                print(x);
                newPos = new Vector3(grid.transform.position.x + x, 0, grid.transform.position.z - z);
                go = Instantiate(floorTilePrefab, newPos, Quaternion.identity);
                go.transform.parent = tilesParent;
            }
        }
    }
}