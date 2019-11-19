using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEntity : MonoBehaviour
{
    public int health = 3;
    [HideInInspector] public int healthMax = 3;

    public float actionCooldown = 1;
    [HideInInspector]
    public float actionCooldownMax = 1;

    public NpcEntity npc;
    public ProjectileEntity proj;
    public Animator anim;
}