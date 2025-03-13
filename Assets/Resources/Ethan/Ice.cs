using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example of a tile that can be picked up.
// A simple rock that can be thrown by the player and enemies alike
public class Ice : Tile
{
    public float slideForce = 5f; 
    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 slidingDirection = rb.linearVelocity.normalized; 
            rb.AddForce(slidingDirection * slideForce, ForceMode2D.Force);
        }
    }
}



