﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject currentTile;
    public float directionX = 1.0f;
    public float directionY = 1.0f;

    private Transform current_t;
    private GameObject nextTile;
    private Transform next_t;
    private Tile tile;
    private Tile tile_nextTile;
    private bool move;
    private Vector3 distance;

    private SpriteRenderer sp;
    public Sprite[] sprites; // four directions, o - top right, 1 - bottom right, 2 - top left , 3 - bottom left

    public PlayerMovement my_movement;
    public GameObject my_flashight;         // so we can switch between different flashlights
    private Inventory my_inventory;
    private pauseMenu game_menu;

	void Start ()
    {
        game_menu = GameObject.FindGameObjectWithTag("Menu Canvas").GetComponent<pauseMenu>();

        current_t = currentTile.GetComponent<Transform>();
        my_inventory = GetComponent<Inventory>();
        tile = currentTile.GetComponent<Tile>();
        sp = GetComponent<SpriteRenderer>();

        nextTile = null;
        move = false;
        SelectDirections();

        //tile.DebugGetAllTile();
    }
	
	void Update ()
    {

        //only move when play select a valid tile
        if (move && PlayerTurn.playerTurn)
        {
            //directions
            SelectDirections();

            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, distance, step);
            
            //update flashlight here so it will only call the function when player moves
            my_flashight.GetComponent<Flashlight>().FlashlightFollowPlayer();

            if (Vector3.Distance(transform.position,distance) < 0.001f)
			{
                PickUpItem();
                CheckEndGame();
				move = false;
                PlayerTurn.SetPlayerTurn();
            }
        }

        //place and pick up flashlight
        if (Input.GetMouseButtonDown(1) && my_flashight)
        {
            
            //place
            if (!my_flashight.GetComponent<Flashlight>().IsPlaced())
            {
                for (int i = 0; i < 4; i++)
                {
                    if (tile.GetAdjacentTileT(i) == my_flashight.GetComponent<Flashlight>().GetPointedTile())
                    {
                        tile.PlaceFlashlight(my_flashight);
                        my_flashight.GetComponent<Flashlight>().Place();
                    }
                }
            }
            //pick up
            else
            {
                //check if player is on the tile with the flashlight
                Debug.Log(tile.flashlightPlaced);
                if (tile.flashlightPlaced)
                {
                    my_flashight = tile.PickUpFlashlight();
                    my_flashight.GetComponent<Flashlight>().PickUp();
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            PlayerTurn.Restart();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            LevelComplete();
        }

        if (Input.GetMouseButtonDown(0))
        {
            my_movement.SelectThisTile();
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            my_flashight.GetComponent<Flashlight>().TurnOn();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            my_flashight.GetComponent<Flashlight>().TurnOff();
        }

    }

    public void SelectTile(GameObject selectedTile)
    {
        bool found = false;
        if (!move)
        {
            for (int i = 0; i < 4; i++)
            {
                found = false;
                if (tile.GetAdjacentTile(i) == selectedTile)
                {
                    //assign nextTile and get Tile component
                    nextTile = selectedTile;
                    tile_nextTile = selectedTile.GetComponent<Tile>();
                    found = true;

                    //adjacent tile and is empty
                    if (tile_nextTile.IsEmpty())
                    {
                        found = true;
                        //enable move
                        move = true;

                        tile.playerOn = false;
                        tile_nextTile.playerOn = true;

                        //get Transform
                        next_t = nextTile.GetComponent<Transform>();
                        CalculateDis();
                        NextTurn();
                    }
                }
            }
            //if move is not enable, tell the player that they can't move to the tile
            if (!move)
            {
                if (found)
                {
                    if (!tile_nextTile.IsEmpty())
                    {
                        Debug.Log("Tile is not empty");
                    }
                }
                else
                {
                    Debug.Log("Not adjacent tile");
                }

            }
        }
        
    }

    void CalculateDis()
    {
        //calculate which way to move to
        Vector3 tmp = next_t.position - current_t.position;
        distance = transform.position + tmp;
        directionX = tmp.x;
        directionY = tmp.y;
    }

    void NextTurn()
    {
        //change currentTile to nextTile
        //change nextTile to null
        currentTile = nextTile;
        current_t = nextTile.GetComponent<Transform>();
        tile = nextTile.GetComponent<Tile>();

        nextTile = null;
        next_t = null;
        tile_nextTile = null;
    }

    void PickUpItem()
    {
        if (tile.item)
        {
            my_inventory.TakeItem(tile.item);
            tile.GetItem();
            my_inventory.DisplayAllItem();
        }
    }

    void CheckEndGame()
    {
        if ((currentTile == my_inventory.endTile) && my_inventory.allItem && !my_flashight.GetComponent<Flashlight>().IsPlaced())
        {
            LevelComplete();
        }
    }

    void SelectDirections()
    {
        if (directionX > 0 && directionY > 0)
        {
            sp.sprite = sprites[0];
        }
        else if (directionX > 0 && directionY < 0)
        {
            sp.sprite = sprites[1];
        }
        else if (directionX < 0 && directionY > 0)
        {
            sp.sprite = sprites[2];
        }
        else if (directionX < 0 && directionY < 0)
        {
            sp.sprite = sprites[3];
        }
    }

    public void SetDirection(int directionIndex)
    {
        sp.sprite = sprites[directionIndex];
    }

    public GameObject GetPlayerCurrentTile()
    {
        return currentTile;
    }

    public bool IsMove()
    {
        return move;
    }

    void LevelComplete()
    {
        Debug.Log("Level complete");
        PlayerTurn.GameOver = true;
        stageSelectController.stageClear[SceneManager.GetActiveScene().buildIndex - 2] = 1;
        game_menu.Victory();
    }
}
