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
            FloorTileEntity tile = gm.levelGenerator.floorTiles[i];

            if (Physics.Raycast(gm.player.tile.transform.position + Vector3.up * 0.5f,
                tile.transform.position - gm.player.tile.transform.position, out hit,
                Vector3.Distance(gm.player.tile.transform.position, tile.transform.position), fovMask))
                {
                    tile.visible = false;
                    tile.fog.SetBool("Active", true);
                    SetActiveObjects(tile, false);
                }
            else
            {
                tile.visible = true;
                tile.fog.SetBool("Active", false);
                SetActiveObjects(tile, true);
            }
        }
    }

    void SetActiveObjects(FloorTileEntity tile, bool active)
    {
        for (int j = tile.objectsOnTile.Count - 1; j >= 0; j--)
        {
            HideObject(tile.objectsOnTile[j], !active);
        }
    }

    public void HideObject(HealthEntity he, bool hide)
    {
        he.canvas.gameObject.SetActive(!hide);

        if (he.anim)
            he.anim.SetBool("Hidden", hide);
    }
}