using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTileEntity : MonoBehaviour
{
    public List<HealthEntity> objectsOnTile;
    public bool visible = true;

    public GameObject wall;
    public Animator fog;
}