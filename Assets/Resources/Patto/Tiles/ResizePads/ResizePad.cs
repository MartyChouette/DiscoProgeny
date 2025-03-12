using System.IO.Pipes;
using UnityEngine;

public class ResizePad : Tile
{
    bool creatureOnTop = false;
    bool charged = true;
    public float rechargeTime;
    float timer = 0;
    public enum ResizeType { Shrink, Enlarge }
    public ResizeType resizeType;

    public Sprite chargedSprite;
    public Sprite notChargedSprite;

    const string tinyTag = "[tiny]"; //these tags are to detect if the player or monster are already tiny or huge
    const string hugeTag = "[huge]";

    private void Start()
    {

    }
    private void Update()
    {
        if (!charged && !creatureOnTop) //just a regular timer to activate the tile again
        {
            timer += Time.deltaTime;
            if (timer >= rechargeTime)
            {
                charged = true;
                _sprite.sprite = chargedSprite;
                timer = 0;
            }
        }
    }
    public override void tileDetected(Tile otherTile)
    {
        creatureOnTop = true;
        if (!charged)
            return;

        switch (resizeType)
        {
            case ResizeType.Shrink:
                if (otherTile.name.Contains(tinyTag)) //here we check if the creature's name has the tag... if it's already tiny, it can't be shrinked
                    break;

                if (otherTile.name.Contains(hugeTag)) //if it's huge, we simply remove the tag
                {
                    string[] dissectedName = otherTile.name.Split(hugeTag);
                    otherTile.name = "";
                    foreach (string s in dissectedName)//theoretically the name should only consist of name and tag, but i can't know for sure if other tiles will mess with the name
                        otherTile.name += s;
                }
                else if (!otherTile.name.Contains(tinyTag))
                {
                    
                    otherTile.name = $"{tinyTag}{otherTile.name}"; //this is then the creature is neither tiny nor huge

                }
                otherTile.transform.localScale /= 2;
                charged = false;
                _sprite.sprite = notChargedSprite;

                break; //i change the name of the object because i can't add more tags as a mod, i'd have to change everybody's code i think.

            case ResizeType.Enlarge:
                if (otherTile.name.Contains(hugeTag))  //this case is the same but the other way around
                    break;

                if (otherTile.name.Contains(tinyTag))
                {
                    string[] dissectedName = otherTile.name.Split(tinyTag);
                    otherTile.name = "";
                    foreach (string s in dissectedName)
                        otherTile.name += s;
                }
                else if (!otherTile.name.Contains(hugeTag))
                {

                    otherTile.name = $"{hugeTag}{otherTile.name}";

                }
                otherTile.transform.localScale *= 2;
                charged = false;
                _sprite.sprite = notChargedSprite;

                break;

                //i'm thinking about adding a bool that let's you or prevents you from resiing carried items
                //also the small tunnel is only a modified wall, i was thinking about making variants so a tunnel system can be built
                //like T, L, + and I shapes
                //the only weird thing is that the player's collider is not a square, so tunnels have to be wider in the vertical axis than in the horizontal axis, if that makes sense

        }

    }

    public override void tileNoLongerDetected(Tile otherTile)
    {
        creatureOnTop = false;
    }
}
