using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace PlatformerGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D backgroundTxr, playerSheetTxr, platformSheetTxr, doorSheetTxr, whiteBox, deathScreenTxr, endScreenTxr, UIboxGem, UIboxKey, UIboxes; // create texture2d variables
        SpriteFont UITextFont, symbolFont; // create font variables
        SoundEffect jumpSnd, bumpSnd, dieSnd, fanfareSnd, keySnd, winSnd; // create sound effect variables
        Song BGmusic, gameOverMusic; // create background music variable(s)
        float playTime = 0, previousTime = 0;
        public bool hasGem, restartLevel, winScreen, hasKey = false;
        Point screenSize = new Point(800, 450); // screensize
        int levelNumber = 0; // create level number count
        PlayerSprite playerSprite; // create player sprite
        GemSprite gemSprite; // create gem sprite
        KeySprite keySprite; // create key sprite
        DoorSprite doorSprite; // create door sprite
        DoorSprite finalDoorSprite;
        Texture2D particleTxr;
        List<ParticleSprite> particleList = new List<ParticleSprite>();
        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>(); // create platforms array
        List<Vector2> doors = new List<Vector2>(); // create doors array
        List<Vector2> finalDoor = new List<Vector2>(); // create final doors array
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
            UIboxGem = Content.Load<Texture2D>("UIItemGem"); // load UI gem spritesheet
            UIboxKey = Content.Load<Texture2D>("UIItemKey"); // load UI key spritesheet
            UIboxes = Content.Load<Texture2D>("UIItemSlots"); // load UI boxes spritesheet
            platformSheetTxr = Content.Load<Texture2D>("spriteSheet2"); // load platform spritesheet
            doorSheetTxr = Content.Load<Texture2D>("spriteSheet3"); // load door spritesheet
            UITextFont = Content.Load<SpriteFont>("UI Font"); // load UI font ( text )
            symbolFont = Content.Load<SpriteFont>("SymbolFont"); // load Symbol font ( symbols )
            jumpSnd = Content.Load<SoundEffect>("jump"); // load jump sound effect
            winSnd = Content.Load<SoundEffect>("youWin"); // load win sound effect
            bumpSnd = Content.Load<SoundEffect>("bump"); // load collision sound effect
            fanfareSnd = Content.Load<SoundEffect>("fanfare"); // load fanfare sound when touching gem
            dieSnd = Content.Load<SoundEffect>("die"); // load death sound effect
            keySnd = Content.Load<SoundEffect>("tink"); // load key sound effect
            BGmusic = Content.Load<Song>("music1"); // load background song file
            gameOverMusic = Content.Load<Song>("gameover"); // load game over sound file
            deathScreenTxr = Content.Load<Texture2D>("deathScreen"); // load death screen image
            endScreenTxr = Content.Load<Texture2D>("endScreen"); // load end screen image
            whiteBox = new Texture2D(GraphicsDevice, 1, 1); // collision
            whiteBox.SetData(new[] { Color.White } ); // collision
            particleTxr = Content.Load<Texture2D>("particle"); // load particle texture
            winScreen = false;
            keySprite = new KeySprite(doorSheetTxr, whiteBox, new Vector2(200, 200)); // load key sprite
            doorSprite = new DoorSprite(doorSheetTxr, whiteBox, new Vector2(200, 200)); // load door sprite
            finalDoorSprite = new DoorSprite(doorSheetTxr, whiteBox, new Vector2(200, 200)); // load last door sprite
            playerSprite = new PlayerSprite(playerSheetTxr, whiteBox, new Vector2(100,50), jumpSnd, bumpSnd); // load player sprite from playersprite class
            gemSprite = new GemSprite(playerSheetTxr, whiteBox, new Vector2(200, 200)); // load gem sprite from gemsprite class
            
            BuildLevels(); // build levels
            gemSprite.spritePos = gems[levelNumber]; // sets the initial location of the gem to the location in the first level
            keySprite.spritePos = keys[levelNumber]; // sets the initial location of the key to the location in the first level
            doorSprite.spritePos = doors[levelNumber]; // sets the initial location of the door to the location in the first level
            finalDoorSprite.spritePos = finalDoor[levelNumber];

            MediaPlayer.IsRepeating = true; // makes song repeat forever
            MediaPlayer.Play(BGmusic); // plays the BGmusic
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (playerSprite.lives > 0)
            {
                playTime += (float)gameTime.ElapsedGameTime.TotalSeconds; // time will keep increasing while the player has more than 0 lives
            }

            playerSprite.Update(gameTime, levels[levelNumber]);
            if (playerSprite.spritePos.Y > screenSize.Y + 50) // reset player when they are outside the screen
            {
                playerSprite.lives--;
                for (int i = 0; i < 16; i++) particleList.Add(new ParticleSprite(particleTxr, whiteBox, new Vector2(playerSprite.spritePos.X + (playerSheetTxr.Width / 2) - (particleTxr.Width / 2), playerSprite.spritePos.Y + (playerSheetTxr.Height / 2) - (particleTxr.Height / 2))));
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
                gemSprite.spritePos = gems[levelNumber]; // sets the gemSprite position to its position in the level
                keySprite.spritePos = keys[levelNumber]; // sets the keySprite position to its position in the level
                doorSprite.spritePos = doors[levelNumber]; // sets the doorSprite position to its position in the level
                finalDoorSprite.spritePos = finalDoor[levelNumber];
                playTime = 0;
                MediaPlayer.IsRepeating = true; // makes song repeat forever
                MediaPlayer.Play(BGmusic); // plays the BGmusic
                playerSprite.ResetPlayer(new Vector2(100, 50));
            }

            if (playerSprite.checkCollision(keySprite)) // checks if player collided with key
            {
                hasKey = true;
                keySprite.isDead = true;
                keySprite.spritePos = new Vector2(-500, -500); // sets the keySprite position outside of the game so player cant collide with it
                keySnd.Play();
            }

            if (playerSprite.checkCollision(doorSprite) && hasKey) // switches level when player has key and collides with door
            {
                hasKey = false; // sets hasKey to false
                hasGem = false; // sets hasGem to false
                keySprite.isDead = false; 
                gemSprite.isDead = false;
                levelNumber++; //increases level count
                gemSprite.spritePos = gems[levelNumber]; // sets gem position to its position in the current level
                keySprite.spritePos = keys[levelNumber]; // sets key position to its position in the current level
                doorSprite.spritePos = doors[levelNumber]; // sets door position to its position in the current level
                finalDoorSprite.spritePos = finalDoor[levelNumber]; // sets final door position to its position in the current level
                playerSprite.ResetPlayer(new Vector2(100, 50)); // resets player position
                fanfareSnd.Play(); // plays fanfare
                if (playTime != 0) previousTime = playTime; // sets previous level time
                playTime = 0; // resets playtime
            }

            if (playerSprite.checkCollision(finalDoorSprite) && hasKey) // checks if player has a key and collided with the final door
            {
                winScreen = true; // sets winScreen to true, which draws the win screen
                fanfareSnd.Play(); // plays fanfare
                playerSprite.lives = 0; // sets the players lives to 0, thus stopping the player from moving
                playerSprite.ResetPlayer(new Vector2(100, 50)); // resets player position
            }

            if (playerSprite.checkCollision(gemSprite)) // checks if player collided with gem
            {
                hasGem = false;
                gemSprite.spritePos = new Vector2(-500, -500); // sets the gemSprite position outside of the game so player cant collide with it
                gemSprite.isDead = true;
                fanfareSnd.Play();
            }

            foreach (ParticleSprite particle in particleList) particle.Update(gameTime, screenSize);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            string livesString = "";

            _spriteBatch.Begin(); // begin spritebatch

            _spriteBatch.Draw(backgroundTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); // draw background

            _spriteBatch.Draw(UIboxes, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); // draw UI boxes

            foreach (ParticleSprite particle in particleList) // draw particles for each particle in the particle list
            {
                particle.Draw(_spriteBatch); // draw particles
            }


            doorSprite.Draw(_spriteBatch, gameTime); //draw doorsprite

            finalDoorSprite.Draw(_spriteBatch, gameTime); //draw doorsprite

            if (!keySprite.isDead) keySprite.Draw(_spriteBatch, gameTime); //draw keysprite
            if (keySprite.isDead) _spriteBatch.Draw(UIboxKey, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); //draw key sprite UI

            if ((playerSprite.lives > 0) || (winScreen)) playerSprite.Draw(_spriteBatch, gameTime); // draw playersprite

            if (!gemSprite.isDead) gemSprite.Draw(_spriteBatch, gameTime); //draw gem sprite
            if (gemSprite.isDead) _spriteBatch.Draw(UIboxGem, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); //draw gem sprite UI

            
            foreach(PlatformSprite platform in levels[levelNumber])
            {
                platform.Draw(_spriteBatch, gameTime); // draw platforms
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
            "Time: " + Math.Round(playTime), //display time
            new Vector2(110, 10),
            Color.White
            );

            if (levelNumber != 0)
            {
                _spriteBatch.DrawString(
                UITextFont,
                "Level " + levelNumber + " Time: " + Math.Round(previousTime), // draws the time 
                new Vector2(310, 10),
                Color.White
                );
            }

            if (playerSprite.outOfLives) // game over screen
            {
                Vector2 textSize = UITextFont.MeasureString("GAME OVER"); // measures the size of the text "GAME OVER" with the UITextFont font.
                _spriteBatch.Draw(deathScreenTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); // draws death screen background (red backdrop)
                _spriteBatch.DrawString(UITextFont, "GAME OVER", new Vector2((screenSize.X / 2) - (textSize.X / 2), (screenSize.Y / 2) - (textSize.Y / 2)), Color.White); // draws the game over text
                MediaPlayer.Play(gameOverMusic); // plays the game over music
            }

            if (winScreen) // end screen
            {
                Vector2 textSize = UITextFont.MeasureString("YOU WIN"); // measures the size of the text "YOU WIN" with the UITextFont font.
                _spriteBatch.Draw(endScreenTxr, new Rectangle(0, 0, screenSize.X, screenSize.Y), Color.White); // draws end screen background (blue backdrop)
                _spriteBatch.DrawString(UITextFont, "YOU WIN", new Vector2((screenSize.X / 2) - (textSize.X / 2), (screenSize.Y / 2) - (textSize.Y / 2)), Color.White); // draws the end screen text
                MediaPlayer.Stop(); // stops background music
                winSnd.Play(); // plays win sound
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        void BuildLevels() // levels
        {
            levels.Add(new List<PlatformSprite>()); 
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 300)));
            levels[0].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 300)));
            gems.Add(new Vector2(300, 200));
            doors.Add(new Vector2(250, 236)); //note to self: -64 Y for door on platform
            keys.Add(new Vector2(60, 250));
            finalDoor.Add(new Vector2(800, 800));

            levels.Add(new List<PlatformSprite>());
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(100, 200)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 150)));
            levels[1].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(400, 100)));
            gems.Add(new Vector2(460, 60));
            doors.Add(new Vector2(100, 136));
            keys.Add(new Vector2(400, 80));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 100)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(40, 100)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 250)));
            levels[2].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(35, 400)));
            gems.Add(new Vector2(35,330));
            doors.Add(new Vector2(35, 336));
            keys.Add(new Vector2(120, 180));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[3].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 100)));
            levels[3].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(35, 100)));
            levels[3].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(500, 250)));
            levels[3].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(400, 300)));
            levels[3].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(250, 300)));
            levels[3].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(35, 350)));
            gems.Add(new Vector2(500, 150));
            doors.Add(new Vector2(35, 286));
            keys.Add(new Vector2(250, 250));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[4].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(35, 100)));
            levels[4].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 100)));
            levels[4].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(252, 212)));
            levels[4].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(172, 160)));
            levels[4].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(378, 253)));
            levels[4].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(534, 220)));
            levels[4].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(740, 220)));
            gems.Add(new Vector2(740, 200));
            doors.Add(new Vector2(35, 36));
            keys.Add(new Vector2(534, 200));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[5].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(35, 100)));
            levels[5].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(120, 100)));
            levels[5].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(338, 100)));
            levels[5].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(542, 100)));
            levels[5].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(698, 144)));
            levels[5].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(289, 170)));
            levels[5].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(195, 234)));
            gems.Add(new Vector2(698, 100));
            doors.Add(new Vector2(542, 36));
            keys.Add(new Vector2(195, 160));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[6].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 372)));
            levels[6].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(259, 372)));
            levels[6].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(259, 221)));
            levels[6].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(423, 300)));
            levels[6].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(638, 300)));
            gems.Add(new Vector2(638, 250));
            doors.Add(new Vector2(49, 308));
            keys.Add(new Vector2(259, 170));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[7].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(35, 100)));
            levels[7].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(259, 100)));
            gems.Add(new Vector2(35, 30));
            doors.Add(new Vector2(35, 36));
            keys.Add(new Vector2(259, 60));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[8].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 253)));
            levels[8].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(226, 253)));
            levels[8].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(406, 253)));
            levels[8].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(586, 253)));
            levels[8].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(714, 180)));
            levels[8].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(406, 100)));
            levels[8].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(586, 100)));
            gems.Add(new Vector2(406, 50));
            doors.Add(new Vector2(49, 189));
            keys.Add(new Vector2(586, 50));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 102)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 102)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(241, 102)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(337, 102)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 102)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(625, 102)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(625, 221)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 221)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(241, 221)));
            levels[9].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 221)));
            gems.Add(new Vector2(740, 160));
            doors.Add(new Vector2(49, 157));
            keys.Add(new Vector2(625, 50));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[10].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 253)));
            levels[10].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(241, 253)));
            levels[10].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 253)));
            levels[10].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(625, 253)));
            levels[10].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(721, 253)));
            levels[10].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(721, 379)));
            gems.Add(new Vector2(20, 217));
            doors.Add(new Vector2(721, 315));
            keys.Add(new Vector2(721, 206));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[11].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 307)));
            levels[11].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(241, 275)));
            levels[11].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 243)));
            levels[11].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(625, 211)));
            levels[11].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(625, 391)));
            gems.Add(new Vector2(725, 250));
            doors.Add(new Vector2(625, 327));
            keys.Add(new Vector2(422, 202));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[12].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 118)));
            levels[12].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 118)));
            levels[12].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(337, 418)));
            levels[12].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(529, 386)));
            levels[12].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(721, 354)));
            gems.Add(new Vector2(245, 236));
            doors.Add(new Vector2(337, 354));
            keys.Add(new Vector2(721, 316));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 137)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 137)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(337, 137)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 137)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(529, 137)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(625, 137)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(177, 280)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(273, 280)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(369, 280)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(465, 280)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(561, 280)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(657, 280)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(753, 280)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 418)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 418)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(241, 418)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(337, 418)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 418)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(529, 418)));
            levels[13].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(625, 418)));
            gems.Add(new Vector2(625, 80));
            doors.Add(new Vector2(529, 354));
            keys.Add(new Vector2(177, 236));
            finalDoor.Add(new Vector2(900, 900));

            levels.Add(new List<PlatformSprite>());
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 94)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 94)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(337, 94)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 94)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(529, 94)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 126)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 158)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 190)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(49, 222)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(145, 222)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(241, 222)));
            levels[14].Add(new PlatformSprite(platformSheetTxr, whiteBox, new Vector2(433, 222)));
            gems.Add(new Vector2(433, 58));
            doors.Add(new Vector2(900, 900));
            keys.Add(new Vector2(49, 180));
            finalDoor.Add(new Vector2(433, 158));

        }

    }
}
