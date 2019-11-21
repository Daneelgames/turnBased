using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityList : MonoBehaviour
{
    public List<HealthEntity> healthEntities;
    public List<NpcEntity> npcEntities;

    GameManager gm;

    public void Init()
    {
        gm = GameManager.instance;

        foreach (HealthEntity obj in healthEntities)
        {
            // get links
            obj.npc = obj.gameObject.GetComponent<NpcEntity>();
            obj.anim = obj.GetComponent<Animator>();
            if (obj.npc)
            {
                obj.npc.health = obj;
                npcEntities.Add(obj.npc);
            }

            foreach (FloorTileEntity floor in gm.levelGenerator.floorTiles)
            {
                if (Vector3.Distance(floor.transform.position, obj.transform.position) < 0.5f)
                {
                    obj.tile = floor;
                    break;
                }
            }
        }
    }
}