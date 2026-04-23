using InputHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MouseBuddy;
using PrimitiveBuddy;
using ResolutionBuddy;

namespace MouseBuddy.PointTransformExample
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D _texture;
        Primitive titlesafe;
        IResolution _resolution;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.PortraitDown;

            _resolution = new ResolutionComponent(this, graphics, new Point(720, 1280), new Point(480, 640), false, false, false);

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
            // TODO: Add your initialization logic here

            var input = new MouseComponent(this, EchoCoord);

            var debug = new DebugInputComponent(this, Identity);

            base.Initialize();
        }

        Vector2 EchoCoord(Vector2 pos)
        {
            return pos;
        }

        Matrix Identity()
        {
            return Matrix.Identity;
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

            spriteBatch = new SpriteBatch(GraphicsDevice);
            titlesafe = new Primitive(graphics.GraphicsDevice, spriteBatch);
            _texture = Content.Load<Texture2D>("Braid_screenshot8");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
#if !__IOS__
                Exit();
#endif
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
            // Clear to Black
            graphics.GraphicsDevice.Clear(Color.Black);

            // Calculate Proper Viewport according to Aspect Ratio
            Resolution.ResetViewport();

            spriteBatch.Begin(SpriteSortMode.Immediate,
                              BlendState.AlphaBlend,
                              null, null, null, null,
                              Resolution.TransformationMatrix());

            spriteBatch.Draw(_texture, Vector2.Zero, Color.White);

            titlesafe.Thickness = 3.0f;
            titlesafe.Rectangle(Resolution.TitleSafeArea, Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}