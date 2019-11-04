using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageController : MonoBehaviour {

    
    public Image iv1;
    public Image iv2;

    private float targetAlpha1 = 0f;
    private float targetAlpha2 = 0f;

    public float change = 5f;
    public float speed=0.01f;

    public FileInfo[] imagePaths;

    public void SetFiles(FileInfo[] paths)
    {
        imagePaths = paths;
        InvokeRepeating("ChangeImage",0, change);
    }

    void ChangeImage()
    {
        show(imagePaths[Random.Range(0, imagePaths.Length)]);
    }

    void Update()
    {
        setAlpha(iv1, targetAlpha1);
        setAlpha(iv2, targetAlpha2);            
    }

    private void setAlpha(Image iv,float target)
    {
        Color color = iv.color;
        float diff = iv.color.a - target;
        if (Mathf.Abs(diff) > 0.001f)
        {
            if (diff > 0)
                color.a -= speed;
            else
                color.a += speed;

            iv.color = color;
        }
    }

    public void show(FileInfo info)
    {
        Sprite sprite = LoadNewSprite(info.FullName);
        if (iv1.gameObject.transform.GetSiblingIndex() == 1)
        {
            iv1.sprite = sprite;
            targetAlpha1 = 1f;
            targetAlpha2 = 0f;
            iv1.gameObject.transform.SetAsFirstSibling();
        }
        else
        {
            iv2.sprite = sprite;
            targetAlpha1 = 0f;
            targetAlpha2 = 1f;
            iv2.gameObject.transform.SetAsFirstSibling();
        }
    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
    {
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Texture2D SpriteTexture = LoadTexture(FilePath);
        //FixTransparency(SpriteTexture);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0.5f, 0.5f), 100);
        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath)
    {
        byte[] FileData;
        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);         // Create new "empty" texture                  
            Texture2D Tex2D = new Texture2D(1, 1);
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)            
                return Tex2D;                 // If data = readable -> return texture

        }
        return null;                     // Return null if load failed
    }
}
