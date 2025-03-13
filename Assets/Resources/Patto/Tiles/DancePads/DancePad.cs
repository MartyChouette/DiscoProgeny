using System.IO.Pipes;
using UnityEngine;

public class DancePad : Tile
{
    bool playerOnTop = false;

    public enum DanceType { None, One, Two, Three }
    public DanceType danceType;

    public Sprite frame1;
    public Sprite frame2;

    public float bpm = 0;
    float _aux;
    float timer = 0;

    private void Start()
    {
        _aux = 60 / bpm;

    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > _aux)
        {
            timer -= _aux;
            if (_sprite.sprite == frame2)
                _sprite.sprite = frame1;
            else
                _sprite.sprite = frame2;
        }
    }
    public override void tileDetected(Tile otherTile)
    {
        if (playerOnTop || !otherTile.hasTag(TileTags.Player))
            return;

        if (otherTile.TryGetComponent(out Animator animator))
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
