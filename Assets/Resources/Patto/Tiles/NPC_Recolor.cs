using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class NPC_Recolor : MonoBehaviour
{
    public Color shirtColor;
    public Color pantsColor;

    Color originalShirtColor = new Color(240 / 255.0f, 105 / 255.0f, 156 / 255.0f);
    Color originalPantsColor = new Color(105 / 255.0f, 205 / 255.0f, 240 / 255.0f);

    SpriteRenderer spriteRend;
    Animator animator;
    AnimatorClipInfo[] clips;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();


        // Get the animation clip from the animator
        clips = animator.GetCurrentAnimatorClipInfo(0);
        clips[0].clip


        // Clone the animation
        CloneAnimation(clips[0].clip.name);

        
        spriteRend.sprite = Recolor(spriteRend.sprite, shirtColor, pantsColor); ;
    }

    // Update is called once per frame
    void Update()
    {

    }

    Sprite Recolor(Sprite sprite, Color newShirtColor, Color newPantsColor)
    {
        Texture2D textureA = sprite.texture;

        Texture2D texture = new Texture2D(textureA.width, textureA.height, TextureFormat.RGBA32, 1, false);
        Graphics.CopyTexture(textureA, texture);

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color currentPixelXolor = texture.GetPixel(x, y);
                if (currentPixelXolor == originalShirtColor)
                {
                    texture.SetPixel(x, y, newShirtColor);
                }
                else if (currentPixelXolor == originalPantsColor)
                {
                    texture.SetPixel(x, y, newPantsColor);
                }
            }
        }
        texture.Apply();
        var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath+"newSprite.png", texture.EncodeToPNG());

        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, textureA.width, textureA.height), new Vector2(0.5f, 0.5f), 10);
        return newSprite;
    }

    public AnimationClip Generate(AnimationClip originalAnimation, Sprite[] newSprites)
    {
        AnimationClip animClip = new AnimationClip();
        animClip.frameRate = 60;   // FPS
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding = originalAnimation.bind;
        spriteBinding.type = typeof(Sprite);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[newSprites.Length];
        for (int i = 0; i < newSprites.Length; i++)
        {
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = ((float)i / 25);
            spriteKeyFrames[i].value = newSprites[i];
        }
        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, spriteKeyFrames);
        return animClip;
    }
}
