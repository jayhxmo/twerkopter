using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Sway_Chopter.Source.Mechanics;
using Sway_Chopter.Source.Player;
using Sway_Chopter.Source.Obstacles;

namespace Twerkopter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Viewport viewport;

        public static MainGame me;
        public SpriteFont spriteFont;

        #region Menu
        Vector2 ButtonSize;
        Texture2D btnPlay;
        Texture2D btnRanking;
        Texture2D btnRate;

        Vector2 playLocation;
        Vector2 rankingLocation;
        Vector2 rateLocation;
        bool Menu = true;
        Vector2 Menusize;
        #endregion

        #region GameOver
        Texture2D Scoreboard;
        Vector2 ScoreboardLocation;
        Vector2 ScoreboardSize;
        bool IsGameOver = true;
        Vector2 GameOverLocation;
        Vector2 GameOverSize;

        Texture2D MedalBronze;
        Texture2D MedalSilver;
        Texture2D MedalGold;
        Vector2 MedalLocation;
        Vector2 MedalSize;
        #endregion

        #region Platform
        Texture2D platform;
        Rectangle platformRect;
        #endregion

        public bool GetReadE = true;
        Vector2 READEsize;

        public Score score;
        Vector2 ScoreSize;
        Vector2 HighScoreSize;

        public Player player;
        Obstacles obstacles;

        public SoundEffect buttonSound;


        bool mouseClick = false;
        MouseState mouse;

        KeyboardState keyboard;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;

            // TODO: Add your initialization logic here
            viewport = GraphicsDevice.Viewport;

            me = this;

            ButtonSize = new Vector2(viewport.Width * 0.12f, viewport.Width * 0.06f);
            
            playLocation = new Vector2((viewport.Width / 2) - ButtonSize.X - (viewport.Height / 40), viewport.Height * 0.6f);
            rateLocation = new Vector2((viewport.Width / 2) + (viewport.Height / 40), viewport.Height * 0.6f);

            player = new Player(viewport);
            obstacles = new Obstacles(viewport);

            ScoreboardSize = new Vector2(viewport.Width * .4f, (viewport.Width * .4f) / 124 * 76);
            ScoreboardLocation = new Vector2((viewport.Width - ScoreboardSize.X) / 2, viewport.Height);

            MedalSize = new Vector2(70 * ScoreboardSize.Y / 194);
            MedalLocation = new Vector2(27 * (ScoreboardSize.X / 320), 76 * (ScoreboardSize.Y / 194));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            var content = this.Content;

            btnPlay = content.Load<Texture2D>("btnPlay");
            btnRate = content.Load<Texture2D>("btnRate");

            player.LoadContent(content);
            obstacles.LoadContent(content);

            score = new Score(viewport, content);
            score.display = true;

            spriteFont = content.Load<SpriteFont>("ScoreFont");            
            Scoreboard = content.Load<Texture2D>("Dashboard");

            READEsize = spriteFont.MeasureString("Get Ready");
            Menusize = spriteFont.MeasureString("Twerkopter");

            GameOverSize = spriteFont.MeasureString("Game Over");
            GameOverLocation = new Vector2(viewport.Width / 2, 20);

            MedalBronze = content.Load<Texture2D>("Bronze-Medal");
            MedalSilver = content.Load<Texture2D>("Silver-Medal");
            MedalGold = content.Load<Texture2D>("Gold-Medal");

            buttonSound = content.Load<SoundEffect>("swing");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();

            if (Menu)
            {
                TouchCollection touchCollection = TouchPanel.GetState();
                foreach (TouchLocation tl in touchCollection)
                {
                    if (tl.State == TouchLocationState.Pressed)
                    {
                        if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                        {
                            playLocation.Y += 5;
                        }

                        if (new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                        {
                            rateLocation.Y += 5;
                        }
                    }

                    if (tl.State == TouchLocationState.Released)
                    {
                        if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                        {
                            playLocation.Y -= 5;
                            Menu = false;
                            IsGameOver = false;
                            GetReadE = true;
                            buttonSound.Play();
                        }

                        if (new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                        {
                            /*
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri("http://www.windowsphone.com/en-us/store/app/twerkopter/856c085f-ef09-40ef-be62-4f64011ae84a", UriKind.Absolute);
                            webBrowserTask.Show();
                             * */
                        }
                    }
                }

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (!mouseClick)
                    {
                        if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)mouse.X, mouse.Y))
                        {
                            playLocation.Y += 5;
                        }

                        if (new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains(mouse.Position.X, mouse.Y))
                        {
                            rateLocation.Y += 5;
                        }
                    }

                    mouseClick = true;
                }

                else
                {
                    if (mouseClick)
                    {
                        mouseClick = false;

                        if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains(mouse.X, mouse.Y))
                        {
                            playLocation.Y -= 5;
                            Menu = false;
                            IsGameOver = false;
                            GetReadE = true;
                            buttonSound.Play();
                        }

                    }
                }

                if (keyboard.IsKeyDown(Keys.Space) ||
                    keyboard.IsKeyDown(Keys.Enter))
                {
                    Menu = false;
                    IsGameOver = false;
                    GetReadE = true;
                    buttonSound.Play();
                }

                obstacles.Update(gameTime, 0);
            }

            else
            {
                if (GetReadE)
                {
                    TouchCollection touchCollection = TouchPanel.GetState();
                    foreach (TouchLocation tl in touchCollection)
                    {
                        if (tl.State == TouchLocationState.Released)
                        {
                            buttonSound.Play();
                            GetReadE = false;
                        }
                    }

                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        mouseClick = true;
                    }

                    else
                    {
                        if (mouseClick)
                        {
                            buttonSound.Play();
                            GetReadE = false;

                            mouseClick = false;
                        }
                    }

                    if (keyboard.IsKeyDown(Keys.Space) ||
                    keyboard.IsKeyDown(Keys.Enter))
                    {
                        buttonSound.Play();
                        GetReadE = false;

                        mouseClick = false;
                    }

                    obstacles.Update(gameTime, 0);
                }

                else //The actual game loop
                {
                    if (IsGameOver)
                    {
                        GameOverLocation.Y = TransitionY((viewport.Height / 30), viewport.Height / 4, GameOverLocation.Y);
                        ScoreboardLocation.Y = TransitionY(-(viewport.Height / 30), GameOverLocation.Y + GameOverSize.Y + viewport.Height / 20, ScoreboardLocation.Y);
                        rateLocation.Y = playLocation.Y = ScoreboardLocation.Y + ScoreboardSize.Y + viewport.Height / 20;

                        TouchCollection touchCollection = TouchPanel.GetState();
                        foreach (TouchLocation tl in touchCollection)
                        {
                            if (tl.State == TouchLocationState.Pressed)
                            {
                                if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                                {
                                    playLocation.Y += 5;
                                }

                                if (new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                                {
                                    rateLocation.Y += 5;
                                }
                            }

                            if (tl.State == TouchLocationState.Released)
                            {
                                if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                                {
                                    buttonSound.Play();

                                    playLocation.Y -= 5;

                                    player = new Player(viewport);
                                    player.LoadContent(this.Content);

                                    obstacles = new Obstacles(viewport);
                                    obstacles.LoadContent(this.Content);

                                    score.score = 0;

                                    Menu = false;
                                    IsGameOver = false;
                                    GetReadE = true;
                                }

                                if (new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                                {
                                    /*
                                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                                    webBrowserTask.Uri = new Uri("http://www.windowsphone.com/en-us/store/app/twerkopter/856c085f-ef09-40ef-be62-4f64011ae84a", UriKind.Absolute);
                                    webBrowserTask.Show();
                                     * */
                                }
                            }
                        }

                        if (mouse.LeftButton == ButtonState.Pressed)
                        {
                            if (!mouseClick)
                            {
                                if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains(mouse.X, mouse.Y))
                                {
                                    playLocation.Y += 5;
                                }

                                if (new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains(mouse.X, mouse.Y))
                                {
                                    rateLocation.Y += 5;
                                }
                            }

                            mouseClick = true;
                        }

                        else
                        {
                            if (mouseClick)
                            {
                                if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains(mouse.X, mouse.Y))
                                {
                                    buttonSound.Play();

                                    playLocation.Y -= 5;

                                    player = new Player(viewport);
                                    player.LoadContent(this.Content);

                                    obstacles = new Obstacles(viewport);
                                    obstacles.LoadContent(this.Content);

                                    score.score = 0;

                                    Menu = false;
                                    IsGameOver = false;
                                    GetReadE = true;

                                    mouseClick = false;
                                }
                            }
                            
                        }

                        if (keyboard.IsKeyDown(Keys.Space) ||
                            keyboard.IsKeyDown(Keys.Enter))
                        {
                            buttonSound.Play();

                            playLocation.Y -= 5;

                            player = new Player(viewport);
                            player.LoadContent(this.Content);

                            obstacles = new Obstacles(viewport);
                            obstacles.LoadContent(this.Content);

                            score.score = 0;

                            Menu = false;
                            IsGameOver = false;
                            GetReadE = true;

                            mouseClick = false;
                        }

                        obstacles.Update(gameTime, 0);
                    }

                    else
                    {
                        TouchCollection touchCollection = TouchPanel.GetState();
                        foreach (TouchLocation tl in touchCollection)
                        {
                            if (tl.State == TouchLocationState.Released)
                            {
                                buttonSound.Play();
                                player.TapUpdate();
                            }
                        }

                        if (mouse.LeftButton == ButtonState.Pressed)
                        {
                            mouseClick = true;
                        }

                        else
                        {
                            if (mouseClick)
                            {
                                buttonSound.Play();
                                player.TapUpdate();

                                mouseClick = false;
                            }
                        }

                        obstacles.Update(gameTime, 4);
                        player.Update(gameTime);

                        if (PlayerIsRekt())
                        {
                            IsGameOver = true;
                            score.saveScore();
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            obstacles.Draw(spriteBatch);
            player.Draw(spriteBatch);
            if (Menu)
            {
                #region outline
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(MainGame.me.viewport.Width * .5f + 3, MainGame.me.viewport.Height * .25f), Color.Black, 0, Menusize * .5f, 2f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(MainGame.me.viewport.Width * .5f - 3, MainGame.me.viewport.Height * .25f), Color.Black, 0, Menusize * .5f, 2f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .25f + 3), Color.Black, 0, Menusize * .5f, 2f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .25f - 3), Color.Black, 0, Menusize * .5f, 2f, SpriteEffects.None, 0f);
                #endregion

                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .25f), Color.White, 0, Menusize * .5f, 2f, SpriteEffects.None, 0f);

                spriteBatch.Draw(btnPlay, new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y), Color.White);
                spriteBatch.Draw(btnRate, new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y), Color.White);
            }

            else
            {
                if (GetReadE)
                {
                    #region outline
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(MainGame.me.viewport.Width * .5f + 3, MainGame.me.viewport.Height * .25f), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(MainGame.me.viewport.Width * .5f - 3, MainGame.me.viewport.Height * .25f), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .25f + 3), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .25f - 3), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                    #endregion

                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(MainGame.me.viewport.Width * .5f, MainGame.me.viewport.Height * .25f), Color.Green, 0, new Vector2(READEsize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                }

                else
                {
                    if (IsGameOver)
                    {
                        #region outline
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X + 3, GameOverLocation.Y), Color.White, 0, new Vector2(GameOverSize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X - 3, GameOverLocation.Y), Color.White, 0, new Vector2(GameOverSize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X, GameOverLocation.Y + 3), Color.White, 0, new Vector2(GameOverSize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X, GameOverLocation.Y - 3), Color.White, 0, new Vector2(GameOverSize.X * .5f, 0), 2f, SpriteEffects.None, 0f);
                        #endregion

                        spriteBatch.DrawString(spriteFont, "Game Over", GameOverLocation, Color.Orange, 0, new Vector2(GameOverSize.X * .5f, 0), 2f, SpriteEffects.None, 0f);

                        spriteBatch.Draw(Scoreboard, new Rectangle((int)ScoreboardLocation.X, (int)ScoreboardLocation.Y, (int)ScoreboardSize.X, (int)ScoreboardSize.Y), Color.White);

                        #region Score
                        ScoreSize = spriteFont.MeasureString(score.score.ToString());
                        #region Outline
                        spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) + 3, ScoreboardLocation.Y + (ScoreboardSize.Y / 4)), Color.Black, 0, new Vector2(ScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) - 3, ScoreboardLocation.Y + (ScoreboardSize.Y / 4)), Color.Black, 0, new Vector2(ScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y / 4) + 3), Color.Black, 0, new Vector2(ScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y / 4) - 3), Color.Black, 0, new Vector2(ScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        #endregion

                        spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y / 4)), Color.White, 0, new Vector2(ScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        #endregion

                        #region HighScore
                        HighScoreSize = spriteFont.MeasureString(score.getHighScore().ToString());
                        #region Outline
                        spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) + 3, ScoreboardLocation.Y + (ScoreboardSize.Y * .7f)), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) - 3, ScoreboardLocation.Y + (ScoreboardSize.Y * .7f)), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y * .7f) + 3), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y * .7f) - 3), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        #endregion

                        spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y * .7f)), Color.White, 0, new Vector2(HighScoreSize.X, 0), 2f, SpriteEffects.None, 0f);
                        #endregion

                        #region Medal
                        if (score.score >= 10)
                        {
                            if (Convert.ToInt32(score.score.ToString().Remove(score.score.ToString().Length - 1)) % 3 == 1)
                            {
                                spriteBatch.Draw(MedalBronze, new Rectangle((int)(MedalLocation.X + ScoreboardLocation.X), (int)(MedalLocation.Y + ScoreboardLocation.Y), (int)MedalSize.X, (int)MedalSize.Y), Color.White);
                            }

                            if (Convert.ToInt32(score.score.ToString().Remove(score.score.ToString().Length - 1)) % 3 == 2)
                            {
                                spriteBatch.Draw(MedalSilver, new Rectangle((int)(MedalLocation.X + ScoreboardLocation.X), (int)(MedalLocation.Y + ScoreboardLocation.Y), (int)MedalSize.X, (int)MedalSize.Y), Color.White);
                            }

                            if (Convert.ToInt32(score.score.ToString().Remove(score.score.ToString().Length - 1)) % 3 == 0)
                            {
                                spriteBatch.Draw(MedalGold, new Rectangle((int)(MedalLocation.X + ScoreboardLocation.X), (int)(MedalLocation.Y + ScoreboardLocation.Y), (int)MedalSize.X, (int)MedalSize.Y), Color.White);
                            }
                        }
                        #endregion

                        spriteBatch.Draw(btnPlay, new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y), Color.White);
                        spriteBatch.Draw(btnRate, new Rectangle((int)rateLocation.X, (int)rateLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y), Color.White);
                    }

                    else
                    {
                        score.Draw(spriteBatch);
                    }
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public bool PlayerIsRekt()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!obstacles.didPass[i])
                {
                    Rectangle rect = new Rectangle(
                        (int)obstacles.locations[i].X,
                        (int)obstacles.locations[i].Y,
                        (int)obstacles.size.X,
                        (int)obstacles.size.Y);
                    if (player.collidesWithPlatform(rect))
                        return true;


                }
            }

            foreach (WreckingBall w in obstacles.WreckingBalls)
            {
                if (w.pos.Y > viewport.Height / 2)
                {
                    if (obstacles.wreckingBallCollision(player))
                        return true;
                }
            }

            return false;
        }

        public float TransitionY(float rate, float destinationY, float Y)
        {
            if (rate > 0)
            {
                if (Y < destinationY)
                {
                    Y += rate;
                    return Y;
                }
            }

            else if (rate < 0)
            {
                if (Y > destinationY)
                {
                    Y += rate;
                    return Y;
                }
            }

            return Y;
        }
    }
}
