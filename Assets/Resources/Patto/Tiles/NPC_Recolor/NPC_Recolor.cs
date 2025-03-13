using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

public class NPC_Recolor : MonoBehaviour
{
    public Color shirtColor;
    public Color pantsColor;

    Color originalShirtColor = new Color(240 / 255.0f, 105 / 255.0f, 156 / 255.0f);
    Color originalPantsColor = new Color(105 / 255.0f, 205 / 255.0f, 240 / 255.0f);

    SpriteRenderer spriteRend;
    Animator animator;
    AnimationClip[] originalClips;

    AnimatorOverrideController animatorOverrideController;
    AnimationClipOverrides clipOverrides;

    Sprite[] newSprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        originalClips = animator.runtimeAnimatorController.animationClips;

        for (int i = 0; i < originalClips.Length; i++)
        {
            AnimationClip newClip = GenerateRecoloredAnimationClip(originalClips[i], newSprites);
            clipOverrides[newClip.name] = newClip;
        }

        animatorOverrideController.ApplyOverrides(clipOverrides);
    }

    // Update is called once per frame
    void Update()
    {

    }

    Sprite RecolorSprite(Sprite sprite, Color newShirtColor, Color newPantsColor)
    {
        Texture2D textureA = sprite.texture;

        Texture2D texture = new Texture2D(textureA.width, textureA.height, TextureFormat.RGBA32, 1, false);
        texture.filterMode = FilterMode.Point;
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

        //save image to file just for debug purposes
        /*var dirPath = Application.dataPath + "/../SaveImages/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath+"newSprite.png", texture.EncodeToPNG());*/

        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, textureA.width, textureA.height), new Vector2(0.5f, 0.5f), 10);
        return newSprite;
    }

    public AnimationClip GenerateRecoloredAnimationClip(AnimationClip originalAnimation, Sprite[] newSprites)
    {
        AnimationClip animClip = new AnimationClip();
        animClip.frameRate = originalAnimation.frameRate;
        EditorCurveBinding spriteBinding = AnimationUtility.GetObjectReferenceCurveBindings(originalAnimation)[0];
        ObjectReferenceKeyframe[] originalKeyframes = AnimationUtility.GetObjectReferenceCurve(originalAnimation, spriteBinding);
        ObjectReferenceKeyframe[] newKeyframes = new ObjectReferenceKeyframe[originalKeyframes.Length];

        for (int i = 0; i < newKeyframes.Length; i++)
        {
            ObjectReferenceKeyframe newKeyframe = new ObjectReferenceKeyframe();
            newKeyframe.time = originalKeyframes[i].time;
            newKeyframe.value = RecolorSprite(originalKeyframes[i].value as Sprite, shirtColor, pantsColor);
            newKeyframes[i] = newKeyframe;
        }

        AnimationUtility.SetObjectReferenceCurve(animClip, spriteBinding, newKeyframes);

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(originalAnimation);
        //settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(animClip, settings);

        animClip.name = originalAnimation.name;

        return animClip;
    }
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }

    public AnimationClip this[string name]
    {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}
