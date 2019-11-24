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
            he.inDangerFeedback.SetBool("Active", false);
        }
    }
}