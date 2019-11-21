using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform grid;

    public List<FloorTileEntity> floorTiles;
    public FloorTileEntity floorTilePrefab;
    public Transform floorParent;

    public Animator fogTilePrefab;
    public Transform fogParent;

    public List<GameObject> walls;
    public GameObject wallPrefab;
    public Transform wallParent;


    public void Init()
    {
        grid.gameObject.SetActive(false);

        SpawnTiles();
    }

    void SpawnTiles()
    {
        Vector3 newPos = Vector3.zero;
        FloorTileEntity floor;
        Animator fog;
        GameObject wall;

        int tilesAmount = Mathf.RoundToInt(grid.transform.localScale.x * grid.transform.localScale.z);

        for (int x = 0; x < grid.transform.localScale.x; x ++)
        {
            for (int z = 0; z < grid.transform.localScale.z; z++)
            {
                newPos = new Vector3(grid.transform.position.x + x, 0, grid.transform.position.z - z);
                // spawn floor
                floor = Instantiate(floorTilePrefab, newPos, Quaternion.identity);
                floor.transform.parent = floorParent;
                floor.gameObject.name.Replace("(Clone)", "");
                floor.gameObject.name += "_" + x + "_" + z;
                floorTiles.Add(floor);
                
                // spawn fog
                fog = Instantiate(fogTilePrefab, newPos, Quaternion.identity);
                fog.transform.parent = fogParent;
                fog.gameObject.name.Replace("(Clone)", "");
                fog.gameObject.name += "_" + x + "_" + z;
                floor.fog = fog;

                // spawn wall
                if (Random.value > 0.9f || x == 0 || z == 0 || x == grid.transform.localScale.x - 1 || z == grid.transform.localScale.z - 1)
                {
                    wall = Instantiate(wallPrefab, newPos, Quaternion.identity);
                    wall.transform.parent = wallParent;
                    walls.Add(wall);
                    wall.gameObject.name.Replace("(Clone)", "");
                    wall.gameObject.name += "_" + x + "_" + z;

                    floor.wall = wall;
                }
            }
        }
    }
}