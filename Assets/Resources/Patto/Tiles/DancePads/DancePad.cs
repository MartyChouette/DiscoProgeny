using System.IO.Pipes;
using UnityEngine;

public class DancePad : Tile
{
    bool playerOnTop = false;

    public enum DanceType { None, One, Two, Three }
    public DanceType danceType;

    Animator dancerAnimator;

    private void Start()
    {
        dancerAnimator = GetComponent<Animator>();

    }
    private void Update()
    {

    }
    public override void tileDetected(Tile otherTile)
    {
        if (playerOnTop || !otherTile.hasTag(TileTags.Player))
            return;

        if(otherTile.TryGetComponent(out Animator animator))
        {
            animator.SetInteger("Dance", ((int)danceType));
        }

        playerOnTop = true;
    }

    public override void tileNoLongerDetected(Tile otherTile)
    {
        playerOnTop = false;
    }
}
