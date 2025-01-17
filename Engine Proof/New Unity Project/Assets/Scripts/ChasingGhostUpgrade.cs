﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingGhostUpgrade : MonoBehaviour
{
    public bool trigger;
    public float speed;

    public GameObject currentTile;
    public GameObject nextTile;
    public GameObject nextNextTile;
    public AllTiles allTile;

    private Tile currentT;
    private Tile nextT;
    private Tile nextNextT;

    private Player player;
    private bool eat;
    private Vector3 distance;
    private int ghostIndex;                     // use to end this ghost's turn

    private Animator anim;
    private SpriteRenderer sp;
    private int phase;
    [SerializeField] private bool isFacingRight;
    
    void Start()
    {
        trigger = false;
        eat = false;

        currentT = currentTile.GetComponent<Tile>();
        nextT = nextTile.GetComponent<Tile>();
        nextNextT = nextNextTile.GetComponent<Tile>();
        
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        ghostIndex = PlayerTurn.AddGhost();
    }
    
    void Update()
    {
        if (!currentT.flashlightOn && !PlayerTurn.Win)
        {
            currentT.SetNotEmpty();
            anim.SetBool("Stunt", false);
            //nextNextTile = allTile.FindPlayerTile();
            //nextNextT = nextNextT.GetComponent<Tile>();
            if (!trigger && PlayerTurn.ghostFinished[ghostIndex])
            {
                //check for trigger
                CheckPlayer();

                //calculate which tile to move to
                CalculateDis();

                if (nextT.playerOn)
                {
                    PlayerTurn.GameOver = true;
                    eat = true;
                }
                else
                {
                    PlayerTurn.SetGhostTurn(ghostIndex);
                }
            }
            else if (trigger && PlayerTurn.ghostFinished[ghostIndex] && !PlayerTurn.GameOver)
            {
                if (nextT.playerOn)
                {
                    eat = true;
                    PlayerTurn.GameOver = true;
                }
                else if(currentT.playerOn)
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
            currentT.SetEmpty();
            if (PlayerTurn.ghostFinished[ghostIndex])
            {
                FindTile();
                PlayerTurn.SetGhostTurn(ghostIndex);
            }
        }
        if (eat && PlayerTurn.ghostFinished[ghostIndex])
        {
            Move();
        }
    }

    public void CheckPlayer()
    {
        if (nextT.playerOn || nextNextT.playerOn)
        {
            trigger = true;
            SetAnimation();
            anim.SetBool("Awake",true);
        }
    }

    void FindTile()
    {
        nextNextTile = player.GetPlayerCurrentTile();
        nextNextT = nextNextTile.GetComponent<Tile>();
    }

    void Move()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, distance, step);
        if (Vector3.Distance(transform.position, distance) < 0.001f)
        {
            PlayerTurn.SetGhostTurn(ghostIndex);
            anim.SetBool("Move", false);
            ChangeTile();
            CalculateDis();
            if (PlayerTurn.GameOver)
            {
                PlayerTurn.SetGameOver();
            }
            else
            {
                PlayerTurn.SetGhostTurn(ghostIndex);
            }
        }
    }

    void CalculateDis()
    {
        //calculate which way to move to
        Vector3 tmp = nextTile.transform.position - currentTile.transform.position;
        distance = transform.position + tmp;

        //calculate which animation to use;
        if (nextTile.transform.position.x > currentTile.transform.position.x
            && nextTile.transform.position.y > currentTile.transform.position.y)
        {
            phase = 4;
        }
        else if (nextTile.transform.position.x > currentTile.transform.position.x
          && nextTile.transform.position.y < currentTile.transform.position.y)
        {
            phase = 3;
        }
        else if (nextTile.transform.position.x < currentTile.transform.position.x
            && nextTile.transform.position.y < currentTile.transform.position.y)
        {
            phase = 2;
        }
        else if (nextTile.transform.position.x < currentTile.transform.position.x
            && nextTile.transform.position.y > currentTile.transform.position.y)
        {
            phase = 1;
        }
    }

    public void ChangeTile()
    {
        if(nextT == nextNextT)
        {
            FindTile();
        }
        currentT.SetEmpty();
        nextT.SetNotEmpty();

        currentTile = nextTile;
        currentT = nextT;

        nextTile = nextNextTile;
        nextT = nextNextT;

        FindTile();
    }

    //1 = top left, 2 = bottom left, 3 = bottom right, 4 = top right
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
}
