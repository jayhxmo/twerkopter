using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twerkopter
{
    public abstract class State
    {
        public GraphicsDeviceManager graphics;
        public ContentManager content;
        public Viewport viewport;

        public State(GraphicsDeviceManager g, ContentManager c, Viewport v)
        {
            graphics = g;
            content = c;
            viewport = v;
        }

        public string type;
        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual State Update(GameTime gameTime) { return this; }
        public virtual void Draw(GameTime gameTime, SpriteBatch s) { }
    }
}