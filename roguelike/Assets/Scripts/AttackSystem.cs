﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    GameManager gm;

    public List<ProjectileEntity> projectiles = new List<ProjectileEntity>();

    string dangerousTag = "Dangerous";
    string untaggedTag = "Untagged";

    

    public void Init()
    {
        gm = GameManager.instance;

        foreach (NpcEntity npc in gm.entityList.npcEntities)
        {
            npc.weaponEntity = npc.GetComponent<WeaponEntity>();
            npc.shotCooldownCurrent = Random.Range(2, npc.shotCooldownMax);
        }
    }

    public void PlayerMoved(GameObject targetObject)
    {
        switch (targetObject.layer)
        {
            default: // tile
                break;
            case 8: // tile
                break;
            case 9: // wall

                break;
            case 10: // unit
                gm.movementSystem.anim.SetTrigger("Attack");

                for (int i = gm.entityList.npcEntities.Count - 1; i >=0; i--)
                {
                    if (gm.entityList.npcEntities[i].gameObject == targetObject)
                    {
                        //gm.entityList.npcEntities[i].health.health -= 1;
                        gm.healthSystem.DamageEntity(gm.entityList.npcEntities[i].health, gm.player);
                        gm.movementSystem.PushObject(gm.entityList.npcEntities[i].health, gm.player.transform.position);
                        break;
                    }
                }
                break;
        }
    }

    public void NpcAttack(NpcEntity npc) // NPC
    {
        if (npc.health.health <= 0)
        {
            // do nothing
        }
        else // NPC ACT
        {
            gm.movementSystem.SavePosition(npc);
            var he = npc.health;
            // enemies attack
            if (he.npc && he.npc.weaponEntity)
            {
                he.npc.attackTarget = gm.player;

                if (he.npc.weaponEntity.aimType == WeaponEntity.AimType.Cross)
                {
                    if (Mathf.Round(he.transform.position.x) == Mathf.Round(gm.player.transform.position.x) || Mathf.Round(he.transform.position.z) == Mathf.Round(gm.player.transform.position.z))
                    {
                        ProjectileEntity projectile = Instantiate(he.npc.weaponEntity.projectile, he.transform.position, Quaternion.identity);
                        projectile.master = he;

                        HealthEntity projectileHealth = projectile.GetComponent<HealthEntity>();
                        projectile.health = projectileHealth;
                        projectileHealth.proj = projectile;

                        gm.entityList.AddObject(projectileHealth);

                        if (gm.player.transform.position.x > projectile.transform.position.x)
                            projectile.direction = new Vector3(1, 0, 0);
                        else if (gm.player.transform.position.x < projectile.transform.position.x)
                            projectile.direction = new Vector3(-1, 0, 0);
                        else if (gm.player.transform.position.z < projectile.transform.position.z)
                            projectile.direction = new Vector3(0, 0, -1);
                        else if (gm.player.transform.position.z > projectile.transform.position.z)
                            projectile.direction = new Vector3(0, 0, 1);

                        projectile.newPos = projectile.transform.position;
                        he.npc.shotCooldownCurrent = he.npc.shotCooldownMax;
                        projectiles.Add(projectile);
                        projectile.telegraphTurn = true;
                        npc.projectileToFire = projectile;

                        for (int i = 0; i <= projectile.movementSpeed; i++)
                        {
                            Animator newDangerousSprite = Instantiate(projectile.dangerousSprite, projectile.transform.position, Quaternion.identity);
                            projectile.dangerousSprites.Add(newDangerousSprite);
                        }
                        npc.moveCooldown = 1;
                        StartCoroutine(CalculateDangerousTiles(projectile));
                    }
                }
            }
        }
    }

    void CheckDamage() 
    {
        if (projectiles.Count > 0)
        { 
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (projectiles[i] != null && !projectiles[i].telegraphTurn)
                {
                    HealthEntity damaged = null;
                    foreach (Animator tile in projectiles[i].dangerousSprites)
                    {
                        if (tile.gameObject.tag == dangerousTag)
                        {
                            foreach (HealthEntity he in gm.entityList.healthEntities)
                            {
                                if (he.health > 0 && he != projectiles[i].master)
                                {
                                    if (Vector3.Distance(tile.transform.position, he.transform.position) < 0.5f)
                                    {
                                        damaged = he;
                                        break;
                                    }
                                }
                            }
                            if (damaged != null)
                                break;
                        }
                    }

                    if (damaged != null)
                    {
                        // damage entity
                        gm.healthSystem.DamageEntity(damaged, projectiles[i].master);
                        projectiles[i].damagedObject = damaged;
                        projectiles[i].deathPosition = damaged.transform.position;
                    }

                    if (projectiles[i].stepsLast <= 0)
                    {
                        print("stepLast");
                        DestroyProjectile(projectiles[i]);
                    }
                    else if (projectiles[i].damagedObject)
                    {
                        print("damagedObject");
                        DestroyProjectile(projectiles[i]);
                    }
                    else if (projectiles[i].wallOnWay)
                    {
                        print("wallOnwWAy");
                        DestroyProjectile(projectiles[i]);
                    }
                }
            }
        }
    }

    public void DestroyProjectile(ProjectileEntity projectile)
    {
        for (int i = projectile.dangerousSprites.Count - 1; i >= 0; i --)
        {
            projectile.dangerousSprites[i].SetTrigger("Stop");
            Destroy(projectile.dangerousSprites[i].gameObject, 1f);
        }

        projectiles.Remove(projectile);
        gm.entityList.RemoveEntity(projectile.health);

        var particles = Instantiate(projectile.deathParticles, projectile.deathPosition, Quaternion.identity);
        Destroy(particles, 2);
        Destroy(projectile.gameObject);
    }

    IEnumerator CalculateDangerousTiles(ProjectileEntity proj)
    {
        if (proj.telegraphTurn)
        {

        }
        int wallOnWay = 10000000;
        for (int i = 0; i < proj.dangerousSprites.Count; i++)
        {
            if (wallOnWay > i)
            {
                Animator anim = proj.dangerousSprites[i];
                anim.gameObject.SetActive(false);
                anim.transform.position = proj.newPos + proj.direction * i;
                anim.transform.LookAt(anim.transform.position + proj.direction);

                RaycastHit hit;

                if (Physics.Raycast(proj.dangerousSprites[i].transform.position + Vector3.up * 5, Vector3.down, out hit, 10, gm.movementSystem.movementMask))
                {
                    if (hit.collider.tag == "Wall")
                    {
                        proj.wallOnWay = true;
                        proj.deathPosition = proj.dangerousSprites[i].transform.position;
                        wallOnWay = i;
                        //proj.stepsLast = 0;
                        proj.dangerousSprites[i].gameObject.SetActive(false);
                    }
                    else if (hit.collider.tag == "Unit" && hit.collider.gameObject != proj.master.gameObject)
                    {

                    }
                }
            }
        }


        for (int i = 0; i < proj.dangerousSprites.Count; i++)
        {
            if (wallOnWay > i)
            {
                proj.dangerousSprites[i].gameObject.SetActive(true);
                proj.dangerousSprites[i].SetTrigger("Reset");
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void MoveProjectile(ProjectileEntity proj)
    {
        if (!proj.telegraphTurn)
        {
            proj.master.npc.projectileToFire = null;
            Vector3 newPos = proj.transform.position + proj.direction * proj.movementSpeed;
            proj.newPos = newPos;

            proj.stepsLast--;

            StartCoroutine(MoveProjectileOverTime(proj));
        }
        else
        {
            proj.master.anim.SetTrigger("Attack");
            proj.telegraphTurn = false;
        }
    }

    IEnumerator MoveProjectileOverTime(ProjectileEntity proj)
    {
        Vector3 newPos = proj.newPos;
        if (proj.damagedObject) newPos = proj.damagedObject.transform.position;
        else if (proj.wallOnWay) newPos = proj.deathPosition;

        for (int t = 0; t < 8; t++)
        {
            if (proj != null)
            {
                proj.transform.position = Vector3.Lerp(proj.transform.position, newPos, 0.2f);
                yield return new WaitForSeconds(0.01f);
            }
            else break;
        }

        if (proj != null)
        {
            proj.transform.position = proj.newPos;

            if (!proj.wallOnWay)
                proj.deathPosition = proj.newPos;

            if (proj.stepsLast > 0 && !proj.damagedObject)
            {
                StartCoroutine(CalculateDangerousTiles(proj));
            }
        }
    }

    public void TurnOffTelegraphTurns()
    {
        foreach (ProjectileEntity proj in projectiles)
        {
            proj.telegraphTurn = false;
            DangerousTilesSetDanger(true, proj);
        }
    }

    void DangerousTilesSetDanger(bool dangerous, ProjectileEntity proj)
    {
        foreach (Animator anim in proj.dangerousSprites)
        {
            if (dangerous)
                anim.gameObject.tag = dangerousTag;
            else
                anim.gameObject.tag = untaggedTag;
        }

    }
}