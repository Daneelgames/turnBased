using System.Collections;
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
        Init();
    }

    void Init()
    {
        playerInput.Init();
        entityList.Init();
        movementSystem.Init();
        attackSystem.Init();
        healthSystem.Init();
        musicGeneratorSystem.Init();

        playerInput.playersTurn = true;
    }

    private void Update()
    {
        playerInput._Update();
        healthSystem._Update();
        entityList._Update();
    }

    /*
    public void Step(GameEvent lastEvent) 
    {
        switch(lastEvent)
        {
            case GameEvent.PlayerAct:
                //CancelInvoke("AutoPassTurn");
        print("here");
                attackSystem.TurnOffTelegraphTurns();
                playerInput.playersTurn = false;
                // npcs that move are first

                // EXPERIMENTAL:
                StartCoroutine(attackSystem.NpcAttack());
                StartCoroutine(attackSystem.MoveProjectiles());

                musicGeneratorSystem.Step();
                break;

            case GameEvent.ProjectilesMove:
                //attacking are second
        print("here");
                break;

            case GameEvent.NpcAct:
        print("here");
                StartCoroutine(movementSystem.NpcMove());
                break;

            case GameEvent.NpcMove:
        print("here");
                playerInput.playersTurn = true;
                StartCoroutine(movementSystem.NpcMove());
                break;
        }
    }
    */
}