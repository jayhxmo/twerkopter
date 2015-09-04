using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Sway_Chopter.Source.Player;
using Twerkopter;

namespace Sway_Chopter.Source.Obstacles
{
    public class WreckingBall
    {
        public static Texture2D texture;
        public static Texture2D whiteText;
        public Vector2 pos;
        public static float rotation;
        private static int direction = 1;
        private static bool flip = true;
        Vector2 size;

        public WreckingBall(Vector2 position, ContentManager c)
        {
            this.pos = position;
            texture = c.Load<Texture2D>("Wrecking Ball");
            size = new Vector2(MainGame.me.viewport.Height / 100, (MainGame.me.viewport.Height / 100) / 64 * 106);
        }

        public void Update(GameTime gameTime, int moveUp)
        {
            pos.Y += moveUp;
        }

        public static void UpdateRotation(GameTime gameTime)
        {
            float f = 1f - (float)Math.Abs(rotation) / MathHelper.PiOver2;
            f /= 2f;

            rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver4 * direction * f * 2f;
            if (flip && (rotation > MathHelper.PiOver4 || rotation < -MathHelper.PiOver4))
            {
                direction *= -1;
                flip = false;
            }
            if (Math.Abs(rotation) < MathHelper.PiOver4 / 2)
                flip = true;

        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, this.pos, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, rotation, new Vector2(32, 5), 2f, SpriteEffects.None, 0f);
        }
    }
}

