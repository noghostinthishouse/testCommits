﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Tile pointedTile;
    private Tile prevPointedTile;
    private float speed = 10.0f;
    private Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (PlayerTurn.playerTurn && !player.IsMove())
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
            //change character sprite to match rotation
            if (direction.x > 0 && direction.y > 0)
            {
                player.SetDirection(0);
            }
            else if (direction.x > 0 && direction.y < 0)
            {
                player.SetDirection(1);
            }
            else if (direction.x < 0 && direction.y > 0)
            {
                player.SetDirection(2);
            }
            else if (direction.x < 0 && direction.y < 0)
            {
                player.SetDirection(3);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Tile")
        {
            if (pointedTile)
            {
                prevPointedTile = pointedTile;
            }
            pointedTile = collider.GetComponent<Tile>();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Tile")
        {
            pointedTile = collider.GetComponent<Tile>();
        }
    }

    public void SelectThisTile()
    {
        pointedTile.MoveToThisTile();
    }
}
