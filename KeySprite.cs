using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformerGame
{
    class KeySprite : Sprite
    {
        public KeySprite(Texture2D newSpriteSheet, Texture2D newCollisionTxr, Vector2 newLocation)
        : base(newSpriteSheet, newCollisionTxr, newLocation)
        {
            spriteOrigin = new Vector2(0.5f, 0.5f);
            isColliding = true;

                animations = new List<List<Rectangle>>();
                animations.Add(new List<Rectangle>());
                animations[0].Add(new Rectangle(92, 26, 38, 38));
                animations[0].Add(new Rectangle(130, 26, 38, 38));
                animations[0].Add(new Rectangle(168, 26, 38, 38));
                animations[0].Add(new Rectangle(196, 26, 38, 38));
                animations[0].Add(new Rectangle(234, 26, 38, 38));
                animations[0].Add(new Rectangle(272, 26, 38, 38));
                animations[0].Add(new Rectangle(234, 26, 38, 38));
                animations[0].Add(new Rectangle(196, 26, 38, 38));
                animations[0].Add(new Rectangle(168, 26, 38, 38));
                animations[0].Add(new Rectangle(130, 26, 38, 38));

        }
    }
}
