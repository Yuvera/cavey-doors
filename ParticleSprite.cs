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

        public ParticleSprite(Texture2D newTxr, Vector2 newPos) : base(newTxr, newPos)
        {
            maxlife = (float)(rng.NextDouble() + 2);
            currentlife = maxlife;

            velocity = new Vector2((float)(rng.NextDouble() * 100 + 50), (float)(rng.NextDouble() * 100 + 50));
            if (rng.Next(2) > 0) velocity.X *= -1;
            if (rng.Next(2) > 0) velocity.Y *= -1;

            particolour = new Color((float)(
                    rng.NextDouble() / 2 + 0.5),
                    (float)(rng.NextDouble() / 2 + 0.5),
                    0.25f,
                    (float)(rng.NextDouble() / 2 + 0.25)
                    );
        }

        public override void Update(GameTime gameTime, Point screenSize)
        {
            spritePos += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentlife -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(
                spriteTexture,
                new Rectangle(
                    (int)spritePos.X,
                    (int)spritePos.Y,
                    (int)(spriteTexture.Width * (currentlife / maxlife) * 2),
                    (int)(spriteTexture.Height * (currentlife / maxlife) * 2)
                    ),
                particolour
                );
        }
    }
}