using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformerGame
{
    class DoorSprite : Sprite
    {
        public DoorSprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation)
         : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 0f);
            isColliding = true;
            //drawCollision = true;

            animations = new List<List<Rectangle>>();
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 46, 64));
        }
    }
}
