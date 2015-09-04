using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Twerkopter.Source;
using Twerkopter.Source.Mechanics;
using Twerkopter.Source.Player;
using Twerkopter.Source.Obstacles;

namespace Twerkopter
{
    public class GameState : State
    {
        public static GameState me;
        public Viewport viewport;
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

        Player player;
        Obstacles obstacles;

        public SoundEffect buttonSound;

        public GameState(GraphicsDeviceManager g, ContentManager c, Viewport v) : base(g, c, v)
        {
            me = this;
            viewport = v;

            ButtonSize = new Vector2(viewport.Width * 0.34f, viewport.Width * 0.17f);
            btnPlay = content.Load<Texture2D>("btnPlay");
            playLocation = new Vector2((viewport.Width - ButtonSize.X) / 2, viewport.Height * 0.6f);

            score = new Score(v, c);
            score.display = true;

            player = new Player(v);
            player.LoadContent(c);

            obstacles = new Obstacles(viewport);
            obstacles.LoadContent(content);

            spriteFont = content.Load<SpriteFont>("ScoreFont");
            READEsize = spriteFont.MeasureString("Get Ready");
            Menusize = spriteFont.MeasureString("Twerkopter");
            GameOverSize = spriteFont.MeasureString("Game Over");
            
            GameOverLocation = new Vector2((viewport.Width - spriteFont.MeasureString("Game Over").X) / 2, -GameOverSize.Y);
           
            Scoreboard = content.Load<Texture2D>("Dashboard");
            ScoreboardSize = new Vector2(viewport.Width * .9f, viewport.Width * 0.545625f);
            ScoreboardLocation = new Vector2((viewport.Width - ScoreboardSize.X) / 2, viewport.Height);

            buttonSound = c.Load<SoundEffect>("swing");
        }

        public override void Initialize()
        {
        }

        public override void LoadContent()
        {
        }

        public override State Update(GameTime gameTime)
        {
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
                    }
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
                    obstacles.Update(gameTime, 0);
                }

                else //The actual game loop
                {
                    if (IsGameOver)
                    {
                        GameOverLocation.Y = TransitionY((viewport.Height / 30), viewport.Height / 4, GameOverLocation.Y);
                        ScoreboardLocation.Y = TransitionY(-(viewport.Height / 30), GameOverLocation.Y + GameOverSize.Y + viewport.Height / 20, ScoreboardLocation.Y);
                        playLocation.Y = ScoreboardLocation.Y + ScoreboardSize.Y + viewport.Height / 20;

                        TouchCollection touchCollection = TouchPanel.GetState();
                        foreach (TouchLocation tl in touchCollection)
                        {
                            if (tl.State == TouchLocationState.Pressed)
                            {
                                if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                                {
                                    playLocation.Y += 5;
                                }
                            }

                            if (tl.State == TouchLocationState.Released)
                            {
                                if (new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y).Contains((int)tl.Position.X, (int)tl.Position.Y))
                                {
                                    buttonSound.Play();

                                    playLocation.Y -= 5;

                                    player = new Player(viewport);
                                    player.LoadContent(content);

                                    obstacles = new Obstacles(viewport);
                                    obstacles.LoadContent(content);

                                    score.score = 0;

                                    Menu = false;
                                    IsGameOver = false;
                                    GetReadE = true;
                                }
                            }
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
            return this;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            obstacles.Draw(spriteBatch);
            player.Draw(spriteBatch);
            if (Menu)
            {
                #region outline
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(Game1.me.viewport.Width * .5f + 3, Game1.me.viewport.Height * .25f), Color.Black, 0, Menusize * .5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(Game1.me.viewport.Width * .5f - 3, Game1.me.viewport.Height * .25f), Color.Black, 0, Menusize * .5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(Game1.me.viewport.Width * .5f, Game1.me.viewport.Height * .25f + 3), Color.Black, 0, Menusize * .5f, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(Game1.me.viewport.Width * .5f, Game1.me.viewport.Height * .25f - 3), Color.Black, 0, Menusize * .5f, 1f, SpriteEffects.None, 0f);
                #endregion

                spriteBatch.DrawString(spriteFont, "Twerkopter", new Vector2(Game1.me.viewport.Width * .5f, Game1.me.viewport.Height * .25f), Color.White, 0, Menusize * .5f, 1f, SpriteEffects.None, 0f);

                spriteBatch.Draw(btnPlay, new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y), Color.White);
            }

            else
            {
                if (GetReadE)
                {
                    #region outline
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(Game1.me.viewport.Width * .5f + 3, Game1.me.viewport.Height * .25f), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 1f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(Game1.me.viewport.Width * .5f - 3, Game1.me.viewport.Height * .25f), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 1f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(Game1.me.viewport.Width * .5f, Game1.me.viewport.Height * .25f + 3), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 1f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(Game1.me.viewport.Width * .5f, Game1.me.viewport.Height * .25f - 3), Color.White, 0, new Vector2(READEsize.X * .5f, 0), 1f, SpriteEffects.None, 0f);
                    #endregion

                    spriteBatch.DrawString(spriteFont, "Get Ready", new Vector2(Game1.me.viewport.Width * .5f, Game1.me.viewport.Height * .25f), Color.Green, 0, new Vector2(READEsize.X * .5f, 0), 1f, SpriteEffects.None, 0f);
                }

                else
                {
                    if (IsGameOver)
                    {
                        #region outline
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X + 3, GameOverLocation.Y), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X - 3, GameOverLocation.Y), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X, GameOverLocation.Y + 3), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(spriteFont, "Game Over", new Vector2(GameOverLocation.X, GameOverLocation.Y - 3), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        #endregion

                        spriteBatch.DrawString(spriteFont, "Game Over", GameOverLocation, Color.Orange, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        spriteBatch.Draw(Scoreboard, new Rectangle((int)ScoreboardLocation.X, (int)ScoreboardLocation.Y, (int)ScoreboardSize.X, (int)ScoreboardSize.Y), Color.White);

                        #region Score
                            ScoreSize = spriteFont.MeasureString(score.score.ToString());
                            #region Outline
                            spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) + 3, ScoreboardLocation.Y + (ScoreboardSize.Y / 4)), Color.Black, 0, new Vector2(ScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) - 3, ScoreboardLocation.Y + (ScoreboardSize.Y / 4)), Color.Black, 0, new Vector2(ScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y / 4) + 3), Color.Black, 0, new Vector2(ScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y / 4) - 3), Color.Black, 0, new Vector2(ScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            #endregion

                            spriteBatch.DrawString(spriteFont, score.score.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y / 4)), Color.White, 0, new Vector2(ScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                        #endregion

                            #region HighScore
                            HighScoreSize = spriteFont.MeasureString(score.getHighScore().ToString());
                            #region Outline
                            spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) + 3, ScoreboardLocation.Y + (ScoreboardSize.Y * .7f)), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10) - 3, ScoreboardLocation.Y + (ScoreboardSize.Y * .7f)), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y * .7f) + 3), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y * .7f) - 3), Color.Black, 0, new Vector2(HighScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            #endregion

                            spriteBatch.DrawString(spriteFont, score.highScore.ToString(), new Vector2(ScoreboardLocation.X + ScoreboardSize.X - (ScoreboardSize.X / 10), ScoreboardLocation.Y + (ScoreboardSize.Y * .7f)), Color.White, 0, new Vector2(HighScoreSize.X, 0), 1f, SpriteEffects.None, 0f);
                            #endregion
                        spriteBatch.Draw(btnPlay, new Rectangle((int)playLocation.X, (int)playLocation.Y, (int)ButtonSize.X, (int)ButtonSize.Y), Color.White);
                    }

                    else
                    {
                        score.Draw(spriteBatch);
                    }
                }
            }
            spriteBatch.End();
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