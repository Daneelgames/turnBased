using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFovSystem : MonoBehaviour
{
    public LayerMask fovMask;
    GameManager gm;

    public void Init()
    {
        gm = GameManager.instance;
        CalculateFov();
    }

    public void CalculateFov()
    {
        for (int i = 0; i < gm.levelGenerator.floorTiles.Count; i ++)
        {
            RaycastHit hit;
            if (Physics.Raycast(gm.player.tile.transform.position,
                gm.levelGenerator.floorTiles[i].transform.position - gm.player.tile.transform.position, out hit,
                Vector3.Distance(gm.player.tile.transform.position, gm.levelGenerator.floorTiles[i].transform.position), fovMask))
                {
                    if (hit.collider.gameObject != gm.levelGenerator.floorTiles[i].wall)
                    {
                        gm.levelGenerator.floorTiles[i].visible = false;
                        gm.levelGenerator.floorTiles[i].fog.SetBool("Active", true);
                    }
                }
            else
            {
                gm.levelGenerator.floorTiles[i].visible = true;
                gm.levelGenerator.floorTiles[i].fog.SetBool("Active", false);
            }
        }
    }
}