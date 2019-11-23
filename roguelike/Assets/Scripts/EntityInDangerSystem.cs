using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInDangerSystem : MonoBehaviour
{
    public Camera cam;
    public Canvas parentedCanvas;

    GameManager gm;

    public void Init()
    {
        gm = GameManager.instance;
    }

    public void CallMark(HealthEntity target)
    {
        target.inDangerFeedback.SetBool("Active", true);
    }

    public void HideAllMarks()
    {
        foreach (HealthEntity he in gm.entityList.healthEntities)
        {

        }
    }

    IEnumerator HideMark(DangerMark mark)
    {
        mark.anim.SetBool("Active", false);
        yield return new WaitForSeconds(5f);
        mark.gameObject.SetActive(false);
    }
}