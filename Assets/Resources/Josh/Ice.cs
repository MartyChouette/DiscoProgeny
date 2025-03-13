using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example of a tile that can be picked up.
// A simple rock that can be thrown by the player and enemies alike
public class Ice : Tile
{
    public float slideMultiplier = 1.5f;
    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        Rigidbody2D trb = this.gameObject.GetComponent<Rigidbody2D>();
        Physics.IgnoreCollision(trb, rb);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * slideMultiplier, rb.linearVelocity.y);
        }

    }
    */

}



