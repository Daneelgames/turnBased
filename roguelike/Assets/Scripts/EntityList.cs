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
            obj.proj = obj.GetComponent<ProjectileEntity>();
            obj.actionCooldownMax = obj.actionCooldown;

            if (obj.npc)
            {
                obj.npc.health = obj;
                npcEntities.Add(obj.npc);
            }
        }
    }

    public void _Update()
    {
        for (int i = healthEntities.Count - 1; i >= 0; i--)
        {
            HealthEntity he = healthEntities[i];

            if (he.health > 0)
            {
                if (he.actionCooldown > 0)
                {
                    he.actionCooldown -= Time.deltaTime;
                }
                else
                {
                    if (he.npc)
                    {
                        gm.movementSystem.NpcMove(he.npc);
                        gm.attackSystem.NpcAttack(he.npc);
                    }
                    else if (he.proj)
                    {
                        print("here");
                        gm.attackSystem.MoveProjectile(he.proj);
                    }

                    he.actionCooldown = he.actionCooldownMax;
                }
            }
        }
    }

    public void AddObject(HealthEntity he)
    {
        healthEntities.Add(he);
    }

    public void RemoveEntity (HealthEntity he)
    {
        healthEntities.Remove(he);
    }
}