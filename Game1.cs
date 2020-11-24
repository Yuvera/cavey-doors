using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;


// To Do List
// -----------------------------------------
// Sounds > BG Music, Death Sound
// Death Animation with particle burst
// Title Screen > Start Button
// Game Over Screen > Restart button
// Timer / Time Limit
// Add door > change levels when colliding
// Change platform side collision 
// Make Levels
// Story / Characters (Dialogue?)
// Change Coin to collectable item > item indicator
// Overhaul Textures

// To Do List (Optional) 
// -----------------------------------------
// Double Jump
// Death Counter
// Shadow enemy (Copies player movement)
// Locked Door, need key > similar to gem.
// 

namespace PlatformerGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D backgroundTxr, playerSheetTxr, platformSheetTxr, whiteBox; // create texture2d variables
        SpriteFont UITextFont, symbolFont; // create font variables
        SoundEffect jumpSnd, bumpSnd, fanfareSnd; // create sound effect variables

        Point screenSize = new Point(800, 450); // screensize
        int levelNumber = 0; // create level number count
        PlayerSprite playerSprite; // create player sprite
        GemSprite gemSprite; // create gem sprite

        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>(); // create platforms array
        List<Vector2> gems = new List<Vector2>(); // create gems array

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
            UITextFont = Content.Load<SpriteFont>("UI Font"); // load UI font ( text )
            symbolFont = Content.Load<SpriteFont>("SymbolFont"); // load Symbol font ( symbols )
            jumpSnd = Content.Load<SoundEffect>("jump"); // load jump sound effect
            bumpSnd = Content.Load<SoundEffect>("bump"); // load collision sound effect
            fanfareSnd = Content.Load<SoundEffect>("fanfare"); // load fanfare sound when touching gem

            whiteBox = new Texture2D(GraphicsDevice, 1, 1); // collision
            whiteBox.SetData(new[] { Color.White } ); // collision

            playerSprite = new PlayerSprite(playerSheetTxr, whiteBox, new Vector2(100,50), jumpSnd, bumpSnd); // load player sprite from playersprite class
            gemSprite = new GemSprite(playerSheetTxr, whiteBox, new Vector2(200, 200)); // load gem sprite from gemsprite class

            BuildLevels(); // build levels
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            playerSprite.Update(gameTime, levels[levelNumber]);
            if (playerSprite.spritePos.Y > screenSize.Y + 50) // reset player when they are outside the screen
            {
                playerSprite.lives--;
                if (playerSprite.lives <=0) // reset player when they run out of lives
                {
                    playerSprite.lives = 3;
                    levelNumber = 0;
                    gemSprite.spritePos = gems[levelNumber];

                }
                playerSprite.ResetPlayer(new Vector2(100, 50));
            }

            if (playerSprite.checkCollision(gemSprite)) // checks if player collided with gem
            {
                levelNumber++;
                if (levelNumber >= levels.Count) levelNumber = 0;
                gemSprite.spritePos = gems[levelNumber];
                playerSprite.ResetPlayer(new Vector2(100, 50));
                fanfareSnd.Play();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            string livesString = "";

            _spriteBatch.Begin();

            _spriteBatch.Draw(backgroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White);

            playerSprite.Draw(_spriteBatch, gameTime);

            gemSprite.Draw(_spriteBatch, gameTime);
            
            foreach(PlatformSprite platform in levels[levelNumber])
            {
                platform.Draw(_spriteBatch, gameTime);
            }

            for (int i = 0; i < playerSprite.lives; i++) livesString += "W";
            //

            _spriteBatch.DrawString(symbolFont, livesString, new Vector2(15, 10), Color.White);

            _spriteBatch.DrawString(
                UITextFont,
                "level " + (levelNumber + 1),
                new Vector2(screenSize.X - 15 - UITextFont.MeasureString("level " + (levelNumber + 1)).X, 10),
                Color.White
                );

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void BuildLevels()
        {
            levels.Add(new List<PlatformSprite>());
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 300)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 300)));
            gems.Add(new Vector2(200, 200));

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 200)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 150)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(400, 100)));
            gems.Add(new Vector2(570, 60));

            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 100)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 140)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(40, 100)));
            gems.Add(new Vector2(35,330));
        }

    }
}
