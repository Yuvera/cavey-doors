using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System;


// TO DO (issues)
// Remove key and gem when collided with player
// Game over screen - stop player movement
// Key Sprite 2nd half - flip sprite

// To Do List
// -----------------------------------------
// Stopwatch timer, resets when player dies
// Change Coin to collectable item
// Death Animation ( particle burst )
// Add door > change levels when colliding and has key
// Change platform side collision 
// UI: Key and Gem collected
// Title Screen > Start Button
// Game Over Screen > Restart button
// Add previous level time in next level/pause menu
// pause menu - quit button, pauses game and timer
// Story / Characters (Dialogue?)
// Add/Change Textures
// Make Levels

// To Do List (Maybe) 
// -----------------------------------------
// Double Jump
// Death Counter
// Shadow enemy (Copies player movement)
// 

namespace PlatformerGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D backgroundTxr, playerSheetTxr, platformSheetTxr, doorSheetTxr, whiteBox, deathScreenTxr; // create texture2d variables
        SpriteFont UITextFont, symbolFont; // create font variables
        SoundEffect jumpSnd, bumpSnd, dieSnd, fanfareSnd, keySnd; // create sound effect variables
        Song BGmusic, gameOverMusic; // create background music variable(s)
        float playTime = 0;
        public bool hasKey, hasGem, restartLevel = false;
        Point screenSize = new Point(800, 450); // screensize
        int levelNumber = 0; // create level number count
        PlayerSprite playerSprite; // create player sprite
        GemSprite gemSprite; // create gem sprite
        KeySprite keySprite; // create key sprite
        DoorSprite doorSprite; // create door sprite
        Texture2D particleTxr;
       // List<ParticleSprite> particleList = new List<ParticleSprite>();
        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>(); // create platforms array
        List<Vector2> doors = new List<Vector2>(); // create gems array
        List<Vector2> gems = new List<Vector2>(); // create gems array
        List<Vector2> keys = new List<Vector2>(); // create keys array

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X;
            _graphics.PreferredBackBufferHeight = screenSize.Y;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            backgroundTxr = Content.Load<Texture2D>("background"); // load background image 
            playerSheetTxr = Content.Load<Texture2D>("spriteSheet1"); // load player spritesheet
            platformSheetTxr = Content.Load<Texture2D>("spriteSheet2"); // load platform spritesheet
            doorSheetTxr = Content.Load<Texture2D>("spriteSheet3"); // load door spritesheet
            UITextFont = Content.Load<SpriteFont>("UI Font"); // load UI font ( text )
            symbolFont = Content.Load<SpriteFont>("SymbolFont"); // load Symbol font ( symbols )
            jumpSnd = Content.Load<SoundEffect>("jump"); // load jump sound effect
            bumpSnd = Content.Load<SoundEffect>("bump"); // load collision sound effect
            fanfareSnd = Content.Load<SoundEffect>("fanfare"); // load fanfare sound when touching gem
            dieSnd = Content.Load<SoundEffect>("die"); // load death sound effect
            keySnd = Content.Load<SoundEffect>("tink"); // load key sound effect
            BGmusic = Content.Load<Song>("music1"); // load background song file
            gameOverMusic = Content.Load<Song>("gameover"); // load game over sound file
            deathScreenTxr = Content.Load<Texture2D>("deathScreen"); // load death screen image
            whiteBox = new Texture2D(GraphicsDevice, 1, 1); // collision
            whiteBox.SetData(new[] { Color.White } ); // collision

            keySprite = new KeySprite(doorSheetTxr, whiteBox, new Vector2(200, 200)); // load key sprite
            doorSprite = new DoorSprite(doorSheetTxr, whiteBox, new Vector2(200, 200)); // load door sprite
            playerSprite = new PlayerSprite(playerSheetTxr, whiteBox, new Vector2(100,50), jumpSnd, bumpSnd); // load player sprite from playersprite class
            gemSprite = new GemSprite(playerSheetTxr, whiteBox, new Vector2(200, 200)); // load gem sprite from gemsprite class
            BuildLevels(); // build levels
            gemSprite.spritePos = gems[levelNumber]; // sets the initial location of the gem to the location in the first level
            keySprite.spritePos = keys[levelNumber]; // sets the initial location of the key to the location in the first level
            doorSprite.spritePos = doors[levelNumber]; // sets the initial location of the door to the location in the first level

            MediaPlayer.IsRepeating = true; // makes song repeat forever
            MediaPlayer.Play(BGmusic); // plays the BGmusic
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (playerSprite.lives > 0)
            {
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            playerSprite.Update(gameTime, levels[levelNumber]);
            if (playerSprite.spritePos.Y > screenSize.Y + 50) // reset player when they are outside the screen
            {
                playerSprite.lives--;
                dieSnd.Play();
                if ((playerSprite.lives <=0)) // detects player when they run out of lives
                {
                    playerSprite.outOfLives = true;
                }
                playerSprite.ResetPlayer(new Vector2(100, 50));
            }

            if (restartLevel) // restarts the level
            {
                playerSprite.lives = 3;
                levelNumber = 0;
                gemSprite.spritePos = gems[levelNumber];
                keySprite.spritePos = keys[levelNumber];
                doorSprite.spritePos = doors[levelNumber];
                playTime = 0;
                MediaPlayer.IsRepeating = true; // makes song repeat forever
                MediaPlayer.Play(BGmusic); // plays the BGmusic
                playerSprite.ResetPlayer(new Vector2(100, 50));
            }

            if (playerSprite.checkCollision(keySprite)) // checks if player collided with key
            {
                hasKey = true;
                keySprite.isDead = true;
                keySnd.Play();
            }

            if (playerSprite.checkCollision(doorSprite) && hasKey) // switches level when player has key and collides with door
            {
                hasKey = false;
                hasGem = false;
                keySprite.isDead = false;
                gemSprite.isDead = false;
                levelNumber++;
                if (levelNumber >= levels.Count) levelNumber = 0;
                gemSprite.spritePos = gems[levelNumber];
                keySprite.spritePos = keys[levelNumber];
                doorSprite.spritePos = doors[levelNumber];
                playerSprite.ResetPlayer(new Vector2(100, 50));
                fanfareSnd.Play();
                playTime = 0;
            }

            if (playerSprite.checkCollision(gemSprite)) // checks if player collided with gem
            {
                hasGem = false;
                gemSprite.isDead = true;
                fanfareSnd.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            string livesString = "";

            _spriteBatch.Begin();

            _spriteBatch.Draw(backgroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); // draw background

            doorSprite.Draw(_spriteBatch, gameTime); //draw doorsprite

            if (!keySprite.isDead) keySprite.Draw(_spriteBatch, gameTime); //draw keysprite

            if (playerSprite.lives > 0) playerSprite.Draw(_spriteBatch, gameTime); // draw playersprite

            if (!gemSprite.isDead) gemSprite.Draw(_spriteBatch, gameTime); //draw gem sprite


            foreach(PlatformSprite platform in levels[levelNumber])
            {
                platform.Draw(_spriteBatch, gameTime);
            }

            for (int i = 0; i < playerSprite.lives; i++) livesString += "W"; // draw lives
            //

            _spriteBatch.DrawString(symbolFont, livesString, new Vector2(15, 10), Color.White); // draw lives

            _spriteBatch.DrawString(
                UITextFont,
                "level " + (levelNumber + 1),
                new Vector2(screenSize.X - 15 - UITextFont.MeasureString("level " + (levelNumber + 1)).X, 10), // draw level indicator
                Color.White
                );

            _spriteBatch.DrawString(
            UITextFont,
            "Time: " + Math.Round(playTime),
            new Vector2(110, 10),
            Color.White
            );

            if (playerSprite.outOfLives) // game over screen
            {
                //MediaPlayer.Stop();
                MediaPlayer.Play(gameOverMusic); // plays the game over music
               // MediaPlayer.IsRepeating = false; // disables repeating music
                Vector2 textSize = UITextFont.MeasureString("GAME OVER");
                _spriteBatch.Draw(deathScreenTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);
                _spriteBatch.DrawString(UITextFont, "GAME OVER", new Vector2((screenSize.X / 2) - (textSize.X / 2), (screenSize.Y / 2) - (textSize.Y / 2)), Color.White);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void BuildLevels()
        {
            levels.Add(new List<PlatformSprite>());
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 300)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 300)));
            gems.Add(new Vector2(200, 200));
            doors.Add(new Vector2(250, 236)); //note to self: -64 Y for door on platform
            keys.Add(new Vector2(60, 250));

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 200)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 150)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(400, 100)));
            gems.Add(new Vector2(460, 60));
            doors.Add(new Vector2(100, 136));
            keys.Add(new Vector2(400, 80));

            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 100)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 250)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(40, 100)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(35, 400)));
            gems.Add(new Vector2(35,330));
            doors.Add(new Vector2(35, 336));
            keys.Add(new Vector2(120, 180));
        }

    }
}
