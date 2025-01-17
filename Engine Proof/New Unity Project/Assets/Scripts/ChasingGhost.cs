﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingGhost : MonoBehaviour
{
    public bool triggered;
    public bool stunt;
    public float speed;

    public GameObject tile;             //the tile that the ghost is standing on
    private Tile t;

    public GameObject nextTile;         //the tile in front of the ghost
    private Tile nextTile_t;

    private bool eat;
    private Vector3 distance;

    private int ghostIndex;             //use to end this ghost's turn
    private Player player;

    private Animator anim;
    private SpriteRenderer sp;
    private int phase;              // 1 = top left, 2 = bottom left, 3 = bottom right, 4 = top right
    [SerializeField] private bool isFacingRight;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        nextTile_t = nextTile.GetComponent<Tile>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        t = tile.GetComponent<Tile>();

        triggered = false;
        stunt = false;
        eat = false;
        
        ghostIndex = PlayerTurn.AddGhost();
    }

    void Update()
    {
        if (!t.flashlightOn)
        {
            t.SetNotEmpty();
            anim.SetBool("Stunt", false);
            //this statement is use to detect player in front of their tile
            if (!triggered && PlayerTurn.ghostFinished[ghostIndex])
            {
                //check for trigger
                CheckPlayer();
                //calculate which tile to move to
                CalculateDis();
                SetAnimation();
                PlayerTurn.SetGhostTurn(ghostIndex);
            }
            //this statement is use to move the ghost after it is triggered
            else if (triggered && PlayerTurn.ghostFinished[ghostIndex])
            {
                if (t.playerOn)
                {
                    PlayerTurn.SetGameOver();
                }
                anim.SetBool("Move", true);
                SetAnimation();
                Move();
            }
        }
        else
        {
            anim.SetBool("Stunt", true);
            //SetAnimation();
            t.SetEmpty();
            if (PlayerTurn.ghostFinished[ghostIndex])
            {
                FindPlayerTile();
                CalculateDis();
                PlayerTurn.SetGhostTurn(ghostIndex);
            }
        }
    }

    public void CheckPlayer()
    {
        if (nextTile_t.playerOn)
        {
            triggered = true;
            anim.SetBool("Awake", true);
        }
    }

    void Move()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, distance, step);
        if (Vector3.Distance(transform.position, distance) < 0.001f)
        {
            //anim.SetBool("Behind", false);
            //anim.SetBool("Front", false);
            anim.SetBool("Move", false);
            ChangeTile();
            CalculateDis();
            PlayerTurn.SetGhostTurn(ghostIndex);
        }
    }

    void CalculateDis()
    {
        //calculate which way to move to
        Vector3 tmp = nextTile.transform.position - tile.transform.position;
        distance = transform.position + tmp;

        //calculate which animation to use;
        if (nextTile.transform.position.x > tile.transform.position.x 
            && nextTile.transform.position.y > tile.transform.position.y)
        {
            phase = 4;
        } else if (nextTile.transform.position.x > tile.transform.position.x
            && nextTile.transform.position.y < tile.transform.position.y)
        {
            phase = 3;
        }
        else if (nextTile.transform.position.x < tile.transform.position.x
            && nextTile.transform.position.y < tile.transform.position.y)
        {
            phase = 2;
        }
        else if (nextTile.transform.position.x < tile.transform.position.x 
            && nextTile.transform.position.y > tile.transform.position.y)
        {
            phase = 1;
        }
        
    }

    // 1 = top left, 2 = bottom left, 3 = bottom right, 4 = top right
    void SetAnimation()
    {
        switch (phase)
        {
            case 1:
                if (isFacingRight)
                {
                    sp.flipX = false;
                    isFacingRight = !isFacingRight;
                }
                anim.SetBool("Behind", true);
                anim.SetBool("Front", false);
                break;
            case 2:
                if (isFacingRight)
                {
                    sp.flipX = false;
                    isFacingRight = !isFacingRight;
                }
                anim.SetBool("Front", true);
                anim.SetBool("Behind", false);
                break;
            case 3:
                if (!isFacingRight)
                {
                    sp.flipX = true;
                    isFacingRight = !isFacingRight;
                }
                anim.SetBool("Front", true);
                anim.SetBool("Behind", false);
                break;
            case 4:
                if (!isFacingRight)
                {
                    sp.flipX = true;
                    isFacingRight = !isFacingRight;
                }
                anim.SetBool("Behind", true);
                anim.SetBool("Front", false);
                break;
        }
    }

    void FindPlayerTile()
    {
        nextTile = player.currentTile;
        nextTile_t = player.currentTile.GetComponent<Tile>();

        //Debug.Log(nextTile);
    }

    public void ChangeTile()
    {
        t.SetEmpty();
        nextTile_t.SetNotEmpty();
        tile = nextTile;
        t = nextTile_t;
        nextTile = player.GetPlayerCurrentTile();
        nextTile_t = nextTile.GetComponent<Tile>();
    }
}
