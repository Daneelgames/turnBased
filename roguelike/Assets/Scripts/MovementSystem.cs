using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    Vector3 currentPosition;
    [HideInInspector]
    public Animator anim;
    GameManager gm;

    public LayerMask movementMask;

    public void Init()
    {
        gm = GameManager.instance;
        currentPosition = gm.player.transform.position;
        anim = gm.player.GetComponent<Animator>();
    }

    public IEnumerator Move(Vector3 add, bool findClosestPoint)
    {
        Vector3 newPos = currentPosition + add;
        GameObject targetObject = CanMove(newPos);

        if (targetObject == null)
        {
            // Can not move
        }
        else if (targetObject.layer == 8) // tile
        {
            gm.CancelInvoke();
            gm.playerInput.movementCooldown = gm.playerInput.movementCooldownMax;

            gm.player.transform.position = currentPosition;
            gm.player.transform.LookAt(newPos);
            anim.SetTrigger("Move");

            for (int t = 0; t < 10; t++)
            {
                gm.player.transform.position = Vector3.Lerp(gm.player.transform.position, newPos, 0.5f);
                yield return new WaitForSeconds(0.01f);
            }
            gm.player.transform.position = newPos;
            gm.player.transform.position = new Vector3(Mathf.Round(gm.player.transform.position.x), Mathf.Round(gm.player.transform.position.y), Mathf.Round(gm.player.transform.position.z));
            currentPosition = gm.player.transform.position;

            gm.attackSystem.PlayerMoved(targetObject);

            yield return new WaitForSeconds(0.1f);
            //gm.Step(GameManager.GameEvent.PlayerAct);
        }
        else if (targetObject.layer == 9) // wall
        {
            gm.CancelInvoke();
            // Move into wall
        }
        else if (targetObject.layer == 10) // move into other unit
        {
            gm.CancelInvoke();
            gm.attackSystem.PlayerMoved(targetObject);
            yield return new WaitForSeconds(0.1f);
            //gm.Step(GameManager.GameEvent.PlayerAct);
        }
    }


    GameObject CanMove(Vector3 targetPos)
    {
        RaycastHit hit;
        Physics.Raycast(targetPos + Vector3.up * 5f, Vector3.down, out hit, 10, movementMask);
        if (hit.collider != null)
            return hit.collider.gameObject;
        else
            return null;
    }

    public void PushObject(HealthEntity he, Vector3 pushOrigin)
    {
        SavePosition(he.npc);
        Vector3 newPos = he.transform.position;

        if (pushOrigin.x < he.transform.position.x)
        {
            // push right
            newPos += Vector3.right;
        }
        else if (pushOrigin.x > he.transform.position.x)
        {
            // push left
            newPos += Vector3.left;
        }
        else if (pushOrigin.z > he.transform.position.z)
        {
            newPos += Vector3.back;
        }
        else if (pushOrigin.z < he.transform.position.z)
        {
            newPos += Vector3.forward;
        }

        GameObject targetObj = CanMove(newPos);
        if (targetObj != null)
        {
            if (targetObj.layer == 8) // tile
            {
                SavePosition(he.npc);
                he.transform.LookAt(newPos);
                //he.anim.SetTrigger("Move"); //animate damaged push
                StartCoroutine(NpcMoveSmooth(he.npc, newPos));
            }
            else if (targetObj.layer == 10) // other unit
            {
                foreach (HealthEntity hee in gm.entityList.healthEntities)
                {
                    if (hee.gameObject == targetObj)
                    {
                        gm.healthSystem.DamageEntity(hee, he);
                        break;
                    }
                }
            }
            else if (targetObj.layer == 9) // wall
            {
                gm.healthSystem.DamageEntity(he, he);
            }
        }
    }

    public void NpcMove(NpcEntity npc)
    {
        if (npc.health.health > 0)
        {
            SavePosition(npc);
            if (npc.canMove && npc.projectileToFire == null)
            {
                if (npc.moveCooldown > 0)
                    npc.moveCooldown--;
                else
                {
                    Vector3 newPos = npc.transform.position + RandomDirection();
                    GameObject targetObj = CanMove(newPos);

                    if (targetObj != null && targetObj.layer == 8 && targetObj.layer != 10) // tile and not unit
                    {
                        SavePosition(npc);
                        npc.transform.LookAt(newPos);
                        npc.health.anim.SetTrigger("Move");
                        StartCoroutine(NpcMoveSmooth(npc, newPos));
                    }
                }
            }
            npc.canMove = true;
        }
    }

    IEnumerator NpcMoveSmooth(NpcEntity npc, Vector3 newPos)
    {
        for (int t = 0; t < 10; t++)
        {
            npc.transform.position = Vector3.Lerp(npc.transform.position, newPos, 0.5f);
            npc.canvas.transform.position = npc.transform.position + npc.canvasOffset;
            yield return new WaitForSeconds(0.01f);
        }
        npc.transform.position = newPos;

        SavePosition(npc);
    }

    public void SavePosition(NpcEntity npc)
    {
        npc.savedPosition = npc.transform.position;
        npc.savedPosition = new Vector3(Mathf.Round(npc.savedPosition.x), Mathf.Round(npc.savedPosition.y), Mathf.Round(npc.savedPosition.z));
        npc.transform.position = npc.savedPosition;
    }

    Vector3 RandomDirection()
    {
        int r = Random.Range(0, 4);

        switch (r)
        {
            default:
                return new Vector3(0, 0, 1);

            case 0:
                return new Vector3(0, 0, 1);
            case 1:
                return new Vector3(1, 0, 0);
            case 2:
                return new Vector3(0, 0, -1);
            case 3:
                return new Vector3(-1, 0, 0);
        }
    }
}