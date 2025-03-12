using UnityEngine;

public class Sticky_Pig : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        //  get the Tile component from the colliding object.
        Tile tile = collision.gameObject.GetComponent<Tile>();
        if (tile != null)
        {
            
            if ( TileTags.Enemy != 0 || (TileTags.Friendly) != 0)
            {
                Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (otherRb != null)
                {


                    // stop  duplicate joints by checking if one already on this connected body.
                    FixedJoint2D[] joints = GetComponents<FixedJoint2D>();
                    foreach (FixedJoint2D joint in joints)
                    {
                        if (joint.connectedBody == otherRb)
                        {
                            return; // Already attached, so exit.
                        }
                    }
                    
                    FixedJoint2D newJoint = gameObject.AddComponent<FixedJoint2D>();
                    newJoint.connectedBody = otherRb;


                }
            }
        }
    }
}

