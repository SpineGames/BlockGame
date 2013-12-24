///The main game class for the block game
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BlockGame;
using BlockGame.UI;
using BlockGame.Render;
using BlockGame.Blocks;
using BlockGame.Utilities;
using BlockGame.Blocks.BlockTypes;
using Spine_Library.Input;
using System.Diagnostics;

namespace BlockGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Static Variables
        /// <summary>
        /// The manin camera in the game
        /// </summary>
        public static Camera camera;
        /// <summary>
        /// The sun for the level
        /// </summary>
        public static Sun sun;
        /// <summary>
        /// The base effect that controls lighting for the world
        /// </summary>
        public static BasicEffect worldEffect;
        /// <summary>
        /// True if the game is in debugging mode
        /// </summary>
        public static bool IsBebugging = true;
        #endregion

        #region private variables
        /// <summary>
        /// The graphics device manager
        /// </summary>
        GraphicsDeviceManager graphics;
        /// <summary>
        /// The spritebbatch used for 2D drawing
        /// </summary>
        SpriteBatch spriteBatch;
        /// <summary>
        /// The standard font to draw with
        /// </summary>
        SpriteFont spriteFont;

        /// <summary>
        /// A dictionary of all the keywatchers used in this game
        /// </summary>
        Dictionary<string, KeyWatcher> keyWatchers = new Dictionary<string, KeyWatcher>();

        /// <summary>
        /// The UI manager for the game
        /// </summary>
        UIManager UI;

        /// <summary>
        /// A trackable version of the framerate
        /// </summary>
        TrackableVariable FrameRate = new TrackableVariable();
        /// <summary>
        /// A trackable version of the camera;s position
        /// </summary>
        TrackableVariable CameraPos = new TrackableVariable();
        /// <summary>
        /// A trackable version of the number of currently loaded chunks
        /// </summary>
        TrackableVariable ChunkCount = new TrackableVariable();
        /// <summary>
        /// A trackable version of the the camera's block facing
        /// </summary>
        TrackableVariable CameraFacing = new TrackableVariable();
        /// <summary>
        /// A trackable version of the camera's yaw
        /// </summary>
        TrackableVariable CameraYaw = new TrackableVariable();

        /// <summary>
        /// Gets if the p command was performed
        /// </summary>
        private bool didPerf;

        RenderTarget2D temp;
        #endregion

        /// <summary>
        /// The initializer for the main game class
        /// </summary>
        public Game1()
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
            //Perlin.SetOctaves(5);
            Perlin.Reseed();

            Texture2D blank = new Texture2D(GraphicsDevice, 1, 1);
            blank.SetData<Color>(new Color[] { Color.White });

            this.IsMouseVisible = true;

            World.Initialize();
            UIManager.Initialize(blank);
            camera = new Camera(new Vector3(0, 0, 32), GraphicsDevice);

            keyWatchers.Add("Debug", new KeyWatcher(Keys.F1, OnDebugPressed));
            keyWatchers.Add("P", new KeyWatcher(Keys.P, PPressed));
            
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
            //load a new font
            spriteFont = Content.Load<SpriteFont>("DebugFont");

            //initialize the texture manager
            TextureManager.Initialize(
                Content.Load<Texture2D>("terrain"), 
                Content.Load<Texture2D>("terrain_normal"));
            
            //build the basic world effect
            BuildBasicEffect();
            sun = new Sun();

            UI = new UIManager(new Vector2(0), new Vector2(10), Color.Gray);
            UI.AddElementLeftAlign(
                new UIE_String(spriteFont, "FPS: {0}", Color.Black, ref FrameRate, null));
            UI.AddElementLeftAlign(
                new UIE_String(spriteFont, "{0}", Color.Black, ref CameraPos, null));
            UI.AddElementLeftAlign(
                new UIE_String(spriteFont, "Chunks: {0}", Color.Black, ref ChunkCount, null));
            UI.AddElementLeftAlign(
                new UIE_String(spriteFont, "Camera Yaw: {0}", Color.Black, ref CameraYaw, null));
            UI.AddElementLeftAlign(
                new UIE_String(spriteFont, "Camera Facing: {0}", Color.Black, ref CameraFacing, null));

            for (int x = 0; x < 6; x++)
                for (int z = 3; z >= 0; z--)
                    for (int y = 0; y < 6; y++)
                        World.AddChunk(new Point3(x, y, z));
                        
            temp = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            //World.AddChunk(new Point3(0, 0, 0));
        }

        /// <summary>
        /// Creates the effect to render with
        /// </summary>
        private void BuildBasicEffect()
        {
            worldEffect = new BasicEffect(GraphicsDevice);

            worldEffect.Projection = camera.View.Projection;
            worldEffect.View = camera.View.View;
            worldEffect.World = camera.View.World;
             
            worldEffect.AmbientLightColor = Color.DarkGray.ToVector3();
            worldEffect.World = camera.View.World;

            worldEffect.DirectionalLight0.Direction = new Vector3(1, 1, 0.5F);
            worldEffect.DirectionalLight0.DiffuseColor = Color.LightYellow.ToVector3();
            worldEffect.DirectionalLight0.Enabled = true;
            worldEffect.LightingEnabled = true;

            worldEffect.TextureEnabled = true;
            worldEffect.Texture = TextureManager.Terrain;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            worldEffect.Dispose();
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

            worldEffect.View = camera.View.View;

            //worldEffect.ViewVector = camera.CameraNormal;
            Vector3 centre = new Vector3(64, 64, 64);

            FrameRate.Value = Spine_Library.Tools.FPSHandler.getFrameRate();
            CameraPos.Value = camera.CameraPos;
            ChunkCount.Value = World.ChunkCount;
            CameraFacing.Value = camera.CameraNormal.ToBlockFacing();
            CameraYaw.Value = camera.CameraYaw;
            
            foreach (KeyWatcher k in keyWatchers.Values)
            {
                k.update();
            }

            camera.UpdateMovement();
            sun.SunTick();
            
            base.Update(gameTime);
        }

        #region Input
        /// <summary>
        /// Called when the debug key is pressed
        /// </summary>
        /// <param name="sender">The object that raised this event (the KeyWatcher in this case)</param>
        /// <param name="e">The event args generated for this event</param>
        public void OnDebugPressed(object sender, EventArgs e)
        {
            IsBebugging = !IsBebugging;

            if (IsBebugging)
            {
                UI.Show = true;
            }
            else
            {
                UI.Show = false;
            }
        }

        /// <summary>
        /// Called when the test key is pressed
        /// </summary>
        /// <param name="sender">The object that raised this event (the KeyWatcher in this case)</param>
        /// <param name="e">The event args generated for this event</param>
        public void PPressed(object sender, EventArgs e)
        {
            if (!didPerf)
            {
                Stopwatch time = new Stopwatch();
                time.Start();
                Debug.WriteLine("\nSetting Glass Cuboid\n");
                World.SetCuboid(new Cuboid(new Point3(1, 1, 1), new Point3(63, 63, 63)),
                    new BlockData(BlockManager.GetBlock("Glass").ID));

                Debug.WriteLine("\nSetting Log Cuboid\n");
                World.SetCuboid(new Cuboid(new Point3(10, 10, 10), new Point3(54, 54, 54)),
                new BlockData(BlockManager.GetBlock("Log").ID, 1));

                Debug.WriteLine("\nSetting Leaves Sphere\n");
                World.SetSphere(new Point3(8, 8, 52), 5.0F,
                new BlockData(BlockManager.GetBlock("Leaves").ID));

                Debug.WriteLine("\nTotal time: {0}s", time.Elapsed.TotalSeconds);
                time.Stop();
                didPerf = true;
            }
        }
        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Spine_Library.Tools.FPSHandler.onDraw(gameTime);

            ThreedDraw();
            SpriteBatchDraw();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles 2D drawng for the game
        /// </summary>
        private void SpriteBatchDraw()
        {
            spriteBatch.Begin();
            
            UI.Render(spriteBatch, Vector2.Zero);

            spriteBatch.End();
        }

        /// <summary>
        /// Handles the 3D drawing for the game
        /// </summary>
        private void ThreedDraw()
        {
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            
            World.Render(camera);

            sun.Render(camera);
        }
    }
}
