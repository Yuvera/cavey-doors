using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace PlatformerGame
{
    class ParticleSprite : Sprite
    {
        Random rng = new Random();
        Vector2 velocity;
        float maxlife;
        public float currentlife;
        Color particolour;

        public ParticleSprite(Texture2D newTxr, Texture2D newCollisionTxr, Vector2 newPos) : base(newTxr, newCollisionTxr, newPos)
        {
            maxlife = (float)(rng.NextDouble() + 6);
            currentlife = maxlife;

            velocity = new Vector2((float)(rng.NextDouble() * 100 + 50), (float)(rng.NextDouble() * 100 + 50));
            if (rng.Next(2) > 0) velocity.X *= -1;
            if (rng.Next(2) > 0) velocity.Y *= -1;

            particolour = new Color((float)(
                    rng.NextDouble() / 2 + 0.2),
                    (float)(rng.NextDouble() / 2 + 0.5),
                    0.2f,
                    (float)(rng.NextDouble() / 2 + 0.5)
                    );
        }

        public void Update(GameTime gameTime, Point screenSize)
        {
            spritePos += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentlife -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(
                spriteSheet,
                new Rectangle(
                    (int)spritePos.X,
                    (int)(spritePos.Y - (currentlife / maxlife) * 1),
                    (int)(spriteSheet.Width * (currentlife / maxlife) * 1),
                    (int)(spriteSheet.Height * (currentlife / maxlife) * 1)
                    ),
                particolour
                );
        }
    }
}