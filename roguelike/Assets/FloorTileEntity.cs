using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTileEntity : MonoBehaviour
{
    public List<HealthEntity> objectsOnTile;
    public bool visible = false;

    public GameObject wall;
    public Animator fog;
}