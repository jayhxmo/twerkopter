using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Twerkopter.Source.Mechanics;
using Twerkopter.Source.Player;

namespace Twerkopter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        public Viewport viewport;

        public State currentState;

        public static Game1 me;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            me = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            viewport = GraphicsDevice.Viewport;

            int width = viewport.Width;
            int height = viewport.Height;

            if (width > height)
            {
                int temp = width;
                width = height;
                height = temp;
            }

            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;

            graphics.SupportedOrientations = DisplayOrientation.Portrait;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            viewport = GraphicsDevice.Viewport;

            currentState = new GameState(graphics, Content, viewport);
            currentState.Initialize();

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
            font = Content.Load<SpriteFont>("spriteFont1");

            // TODO: use this.Content to load your game content here
            currentState.LoadContent();

            Source.Obstacles.WreckingBall.texture = Content.Load<Texture2D>("Wrecking Ball");
            Source.Obstacles.WreckingBall.whiteText = new Texture2D(GraphicsDevice, 1, 1);
            Source.Obstacles.WreckingBall.whiteText.SetData<Color>(new Color[] { Color.White });
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Hello from MonoGame!", new Vector2(16, 16), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
