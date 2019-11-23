using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public Transform objectsCanvases;
    public List<Animator> hearts;
    GameManager gm;

    public void Init()
    {
        gm = GameManager.instance;

        foreach (HealthEntity he in gm.entityList.healthEntities)
        {
            he.healthMax = he.health;
            if (he.npc)
            {
                he.npc.tmpHealth.text = he.health.ToString();
            }
            Vector3 canvasOffset = he.canvas.transform.localPosition;
            he.canvasOffset = canvasOffset;
            he.canvas.transform.parent = objectsCanvases;
        }
    }

    public void DamageEntity(HealthEntity damaged, HealthEntity attacker)
    {
        damaged.anim.SetTrigger("Damaged");
        damaged.health--;

        if (damaged.health <= 0)
        {
            if (attacker == gm.player)
            {
                HealEntity(gm.player);
            }

            damaged.health = 0;
            StartCoroutine(Death(damaged));
        }

        if (damaged.npc)
        {
            damaged.npc.canMove = false;

            if (damaged.npc.projectileToFire != null)
            {
                gm.attackSystem.DestroyProjectile(damaged.npc.projectileToFire);
                damaged.npc.projectileToFire = null;
            }
            damaged.npc.tmpHealth.text = damaged.health.ToString();
        }

        if (damaged == gm.player)
        {
            UpdateHearts();
        }
    }

    IEnumerator SetActive(GameObject go, bool active, float time)
    {
        yield return new WaitForSeconds(time);
        go.SetActive(active);
    }

    IEnumerator Death(HealthEntity he)
    {
        he.anim.SetBool("Death", true);
        he.gameObject.layer = 8;

        yield return null;
    }

    void HealEntity(HealthEntity he)
    {
        if (he == gm.player)
        {
            he.health++;
            if (he.health > he.healthMax) he.health = he.healthMax;

            UpdateHearts();
        }
    }

    void UpdateHearts()
    {
        HealthEntity he = gm.player;

        for (int i = 1; i <= he.healthMax; i++)
        {
            if (he.health >= i)
            {
                StartCoroutine(SetActive(hearts[i - 1].gameObject, true, 0));
            }
            else
            {
                StartCoroutine(SetActive(hearts[i - 1].gameObject, false, 0));
            }
        }
    }
}