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
using Twerkopter;

namespace Twerkopter.Source.Obstacles
{
    public class Obstacles
    {
        List<Texture2D> textures;
        public List<Vector2> locations;
        public List<WreckingBall> WreckingBalls;
        public List<bool> didPass;

        public SoundEffect coin;

        public Vector2 size;
        Texture2D left;
        Texture2D right;

        float passPoint;

        Viewport viewport;

        Random r;
        ContentManager c;

        public Obstacles(Viewport vp)
        {
            Initialize(vp);
        }

        private void Initialize(Viewport vp)
        {
            viewport = vp;

            passPoint = viewport.Height - (viewport.Width * 0.25f * 1.56f * 1.5f);

            size = new Vector2(viewport.Width, viewport.Width * 0.0625f);
            r = new Random();

            textures = new List<Texture2D>();
            locations = new List<Vector2>();
            didPass = new List<Boolean>();
            WreckingBalls = new List<WreckingBall>();
        }

        public void LoadContent(ContentManager content)
        {
            c = content;
            left = content.Load<Texture2D>("Steel Beam Left");
            right = content.Load<Texture2D>("Steel Beam");

            for (int i = 0; i < 7; i++)
            {
                textures.Add(left);
                textures.Add(right);
            }

            Vector2 vc1 = new Vector2(r.Next((int)(viewport.Width / 20), (int)(viewport.Width * 0.4f)) - size.X, size.Y * 3);
            Vector2 vc2 = new Vector2(vc1.X + size.X + (viewport.Width * 0.6f), vc1.Y);
            locations.Add(vc1);

            locations.Add(vc2);
            WreckingBalls.Add(new WreckingBall(new Vector2(vc1.X + size.X - (15 * (size.X / 512)), vc1.Y + (17 * (size.Y / 32)))));
            WreckingBalls.Add(new WreckingBall(new Vector2(vc2.X + (15 * (size.X / 512)), vc2.Y + (17 * (size.Y / 32)))));

            didPass.Add(false);
            didPass.Add(false);

            for (int i = 2; i < 7; i++)
            {
                Vector2 vc3 = new Vector2(r.Next((int)(viewport.Width / 20), (int)(viewport.Width * 0.4f)) - size.X, locations[locations.Count - 2].Y - (size.Y * 16));
                Vector2 vc4 = new Vector2(vc3.X + size.X + (viewport.Width * 0.6f), locations[locations.Count - 2].Y - (size.Y * 16));

                locations.Add(vc3);
                locations.Add(vc4);

                WreckingBalls.Add(new WreckingBall(new Vector2(vc3.X + size.X - (15 * (size.X / 512)), vc3.Y + (17 * (size.Y / 32)))));
                WreckingBalls.Add(new WreckingBall(new Vector2(vc4.X + + (15 * (size.X / 512)), vc4.Y + (17 * (size.Y / 32)))));

                didPass.Add(false);
                didPass.Add(false);
            }

            coin = content.Load<SoundEffect>("coin");
        }

        public void Update(GameTime gameTime, int upSpeed)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                locations[i] = new Vector2(locations[i].X, locations[i].Y + upSpeed);
            }
            foreach (WreckingBall wb in WreckingBalls)
                wb.Update(gameTime, upSpeed);

            if (!didPass[0])
            {
                if (locations[0].Y > passPoint)
                {
                    coin.Play();
                    GameState.me.score.score++;
                    didPass[0] = true;
                    didPass[1] = true;
                }
            }

            if (locations[0].Y > viewport.Height)
            {
                locations.RemoveAt(0);
                locations.RemoveAt(0);

                Vector2 vc3 = new Vector2(r.Next((int)(viewport.Width / 20), (int)(viewport.Width * 0.4f)) - size.X, locations[locations.Count - 2].Y - (size.Y * 16));
                Vector2 vc4 = new Vector2(vc3.X + size.X + (viewport.Width * 0.6f), locations[locations.Count - 2].Y - (size.Y * 16));

                locations.Add(vc3);
                locations.Add(vc4);

                WreckingBalls.Add(new WreckingBall(new Vector2(vc3.X + size.X - (15 * (size.X / 512)), vc3.Y + (17 * (size.Y / 32)))));
                WreckingBalls.Add(new WreckingBall(new Vector2(vc4.X + (15 * (size.X / 512)), vc4.Y + (17 * (size.Y / 32)))));

                didPass.RemoveAt(0);
                didPass.RemoveAt(0);

                didPass.Add(false);
                didPass.Add(false);
            }
            WreckingBall.UpdateRotation(gameTime);

            List<WreckingBall> removeQueue = new List<WreckingBall>();
            foreach (WreckingBall wb in WreckingBalls)
                if (wb.pos.Y > viewport.Height)
                    removeQueue.Add(wb);
            foreach (WreckingBall wb in removeQueue)
                WreckingBalls.Remove(wb);

        }

        public void Draw(SpriteBatch spritebatch)
        {
            for (int i = 0; i < 9; i++)
            {
                spritebatch.Draw(textures[i], new Rectangle((int)locations[i].X, (int)locations[i].Y, (int)size.X, (int)size.Y), Color.White);
            }

            foreach (WreckingBall wb in WreckingBalls)
                wb.Draw(spritebatch);
        }

        public bool wreckingBallCollision(Player.Player p)
        {
            float ballRadiusSquared = 32 * 32;
            Vector2 ballOffset = new Vector2((float)Math.Cos(WreckingBall.rotation + MathHelper.PiOver2), (float)Math.Sin(WreckingBall.rotation + MathHelper.PiOver2)) * 65;
            List<Vector2> ballPositions = new List<Vector2>();

            foreach (WreckingBall b in WreckingBalls)
            {
                if (b.pos.Y >= viewport.Height * .5f)
                    ballPositions.Add(b.pos);
            }

            for (int x = 0; x < p.texture.Width; x++)
                for (int y = 0; y < 160; y++)
                {
                    int X = x;
                    if (p.flip)
                        X = p.texture.Width - x;
                    if (p.textureData[x, y].A > 25) // transparency threshold
                    {
                        Vector2 pos = p.location + new Vector2(X, y);
                        foreach (Vector2 v in ballPositions)
                        {
                            if (Vector2.DistanceSquared(pos, v + ballOffset) < ballRadiusSquared)
                            {
                                return true;
                            }
                        }
                    }
                }

            return false;
        }
    }
}

