﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameEvent {GameReady,  PlayerAct, NpcMove, NpcAct, ProjectilesMove}

    public static GameManager instance;
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public MovementSystem movementSystem;
    [HideInInspector] public AttackSystem attackSystem;
    [HideInInspector] public HealthSystem healthSystem;
    [HideInInspector] public MusicGeneratorSystem musicGeneratorSystem;
    [HideInInspector] public LevelGenerator levelGenerator;
    [HideInInspector] public PlayerFovSystem playerFovSystem;
    public EntityInDangerSystem entityInDangerSystem;
    public HealthEntity player;

    [HideInInspector] public EntityList entityList;

    private void Awake()
    {
        instance = this;
        entityList = GetComponent<EntityList>();
        playerInput = GetComponent<PlayerInput>();
        movementSystem = GetComponent<MovementSystem>();
        attackSystem = GetComponent<AttackSystem>();
        healthSystem = GetComponent<HealthSystem>();
        musicGeneratorSystem = GetComponentInChildren<MusicGeneratorSystem>();
        levelGenerator = GetComponent<LevelGenerator>();
        playerFovSystem = GetComponent<PlayerFovSystem>();
        Init();
    }

    void Init()
    {
        levelGenerator.Init();
        playerInput.Init();
        entityList.Init();
        movementSystem.Init();
        attackSystem.Init();
        healthSystem.Init();
        musicGeneratorSystem.Init();
        playerFovSystem.Init();
        entityInDangerSystem.Init();

        Step(GameEvent.GameReady);
    }

    private void Update()
    {
        playerInput._Update();    
    }

    public void Step(GameEvent lastEvent) 
    {
        switch(lastEvent)
        {
            case GameEvent.GameReady:
                playerInput.playersTurn = true;
                //Invoke("AutoPassTurn", 0.5f);
                musicGeneratorSystem.Step();
                break;

            case GameEvent.PlayerAct:
                //CancelInvoke("AutoPassTurn");
                entityInDangerSystem.HideAllMarks();
                attackSystem.TurnOffTelegraphTurns();
                playerInput.playersTurn = false;
                // npcs that move are first
                StartCoroutine(attackSystem.MoveProjectiles());
                musicGeneratorSystem.Step();
                break;

            case GameEvent.ProjectilesMove:
                //attacking are second
                StartCoroutine(attackSystem.NpcAttack());
                break;

            case GameEvent.NpcAct:
                StartCoroutine(movementSystem.NpcMove());
                break;

            case GameEvent.NpcMove:
                playerInput.playersTurn = true;
                playerFovSystem.CalculateFov();
                //Invoke("AutoPassTurn", 0.5f);
                break;
        }
    }

    void AutoPassTurn()
    {
        Step(GameEvent.PlayerAct);
    }
}