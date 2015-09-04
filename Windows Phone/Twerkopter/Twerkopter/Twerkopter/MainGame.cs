using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Sway_Chopter.Source.Mechanics;
using Sway_Chopter.Source.Player;

using Microsoft.Advertising.Mobile.Xna;

namespace Sway_Chopter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        private string adUnit = "188521";
        private string appID = "ed001509-62a2-4f70-a902-aa84265f8abc";

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Viewport viewport;

        public State currentState;

        public static MainGame me;

        DrawableAd advertisement;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);

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

            AdGameComponent.Initialize(this, appID);
            Components.Add(AdGameComponent.Current);

            // Create an actual ad for display.
            CreateAd();

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
            currentState.LoadContent();

            Source.Obstacles.WreckingBall.texture = Content.Load<Texture2D>("Wrecking Ball");
            Source.Obstacles.WreckingBall.whiteText = new Texture2D(GraphicsDevice, 1, 1);
            Source.Obstacles.WreckingBall.whiteText.SetData<Color>(new Color[] { Color.White });
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            currentState = currentState.Update(gameTime);

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
            currentState.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }

        private void CreateAd()
        {
            // Create a banner ad for the game.
            int width = 480;
            int height = 80;
            int x = (GraphicsDevice.Viewport.Bounds.Width - width) / 2; // centered on the display
            int y = 0;

            advertisement = AdGameComponent.Current.CreateAd(adUnit, new Rectangle(x, y, width, height), true);
        }
    }
}
