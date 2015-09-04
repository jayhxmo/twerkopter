using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Sway_Chopter.Source.Player
{
    public class Player
    {
        public Texture2D texture;
        Rectangle src;
        public bool flip;
        public Vector2 location;
        public Vector2 size;

        public Color[,] textureData;

        Viewport viewport;
        public bool prevTapState = false;

        float timer = 0f;
        public int side = 0;

        float animationtimer = 0f;
        float durationTimer = 25f;
        int frames = 0;

        double x;
        double y;
        double speed;
        double velocity;
        double accel = ACCEL_RATE;

        const double ACCEL_RATE = 0.05;
        const double VERT_SPEED = 1;
        const int ssize = 2;
        const int chainLength = 4;

        public Player(Viewport vp)
        {
            Initialize(vp);
        }

        private void Initialize(Viewport vp)
        {
            viewport = vp;

            size = new Vector2(viewport.Width * 0.25f, viewport.Width * 0.25f  * 3.061538461538462f);

            float xSize = viewport.Width * 0.25f;
            float sizeY = xSize * 1.56f;

            location.Y = viewport.Height - (size.Y * 0.7f);
            location.X = (viewport.Width - size.X) / 2;
        }

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>(@"MileyCyrus");
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(data);
            textureData = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    textureData[x, y] = data[x + y * texture.Width];
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            animationtimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (animationtimer > durationTimer)
            {
                frames++;
                if (frames >= 1)
                {
                    frames = 0;
                }
                animationtimer = 0;
            }
            src = new Rectangle(32 * frames, 0, 32, 32);
            
            if (!flip)
            {
                if (location.X <= (viewport.Width - size.X))
                {
                    velocity += accel;
                    speed += velocity;
                    x += speed;
                }
            }

            else
            {
                if (location.X >= 0)
                {
                    velocity += accel;
                    speed += velocity;
                    x -= speed;
                }
            }
            

            //y += VERT_SPEED;                

            

            location.X = (float)x;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            SpriteEffects fx = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spritebatch.Draw(texture, new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), null, Color.White, 0f, Vector2.Zero, fx, 0f);
        }

        public void TapUpdate()
        {
            if (timer > .1f)
            {
                if (flip)
                {
                    src.X = 0;
                    flip = false;
                    side = 0;
                }

                else
                {
                    src.X = 0;
                    flip = true;
                    side = 1;
                }

                prevTapState = false;
                timer = 0;

                velocity = 0;
                speed = 0;
            }
        }

        public bool collidesWithPlatform(Rectangle platform)
        {
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < 160; y++)
                {
                    int X = x;
                    if (flip)
                        X = texture.Width - x;
                    if (textureData[x, y].A > 25) // transparency threshold
                    {
                        Point p = new Point((int)location.X + x, (int)location.Y + y);
                        if (platform.Contains(p))
                        {
                            return true;
                        }
                    }
                }

            return false;
        }
    }
}
