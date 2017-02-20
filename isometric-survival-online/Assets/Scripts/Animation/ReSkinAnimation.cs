using System;
using UnityEngine;

namespace Assets.Scripts.Animation
{


    public class ReSkinAnimation : MonoBehaviour
    {
        public String spritePath;


        void LateUpdate()
        {
            var subSprites = Resources.LoadAll<Sprite>(spritePath);


            Debug.Log(subSprites.Length);
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                string spriteName = spriteRenderer.sprite.name;
                var newSprite = Array.Find(subSprites, item => item.name == spriteName);

                if (newSprite)
                    spriteRenderer.sprite = newSprite;
            }
        }
    }
}