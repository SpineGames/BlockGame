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
using BlockGame.Render;
using BlockGame.Blocks;
using BlockGame.Utilities;
using Block_Game.Render;
using BlockGame;
using Block_Game.Blocks;
using Block_Game.Utilities;

namespace Block_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Chunk[,] testChunks;
        public static Camera camera;
        public static Sun sun;

        /// <summary>
        /// The base effect that controls lighting for the world
        /// </summary>
        public static StandardEffect worldEffect;

        double f;

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
            World.Initialize();
            camera = new Camera(new Vector3(0, 0, 32), graphics);

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

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    for(int z = 0; z < 5; z ++)
                        World.AddChunk(new Point3(x, y, z));
            
            //testChunks = new Chunk[2, 2];

            //testChunks[0, 0] = new Point3(0, 0, 0);
            //testChunks[0, 1] = new Point3(0, 1, 0);
            //testChunks[1, 1] = new Point3(1, 1, 0);
            //testChunks[1, 0] = new Point3(1, 0, 0);

            //foreach (Chunk c in testChunks)
            //{
            //    c.SetCuboid(new Point3(0, 0, 0), new Point3(32, 32, 32), 1);
            //}

            //testChunks[0, 0].SetSphere(new Point3(16, 16, 30), 8, 0);
            //testChunks[0, 0].SetSphere(new Point3(16, 16, 30), 4, Block.Dirt.ID);
            //testChunks[0, 0].SetSphere(new Point3(16, 16, 32), 4, Block.Glass.ID);

            //foreach (Chunk c in testChunks)
            //{
            //    c.PushRenderState();
            //}
        }

        private void BuildBasicEffect()
        {
            //BasicEffect t = new BasicEffect(GraphicsDevice);
            //t.View = camera.View.View;
            //t.World = camera.View.World;
            //t.Projection = camera.View.Projection;
            //t.Texture = TextureManager.Terrain;
            //t.TextureEnabled = true;
            //t.LightingEnabled = true;
            //t.AmbientLightColor = Color.Red.ToVector3();

            worldEffect = new StandardEffect(Content);

            worldEffect.Projection = camera.View.Projection;
            worldEffect.View = camera.View.View;
            worldEffect.World = camera.View.World;

            worldEffect.AmbientLightColor = Color.LightYellow.ToVector4();
            worldEffect.AmbientLightIntensity = 0.3F;

            worldEffect.DiffuseDirection = new Vector3(1, 1, 0.5F);
            worldEffect.DiffuseColor = Color.White.ToVector4();

            //worldEffect.DiffuseLightDirection = new Vector3(1,1,1);
            //worldEffect.DiffuseColor = Color.Green.ToVector4();
            //worldEffect.DiffuseIntensity = 1F;

            worldEffect.Texture = TextureManager.Terrain;
            //worldEffect.NormalMap = TextureManager.NormalMap;

            worldEffect.BaseEffect.CurrentTechnique = worldEffect.BaseEffect.Techniques["Textured"];
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            worldEffect.BaseEffect.Dispose();
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

            worldEffect.Projection = camera.View.Projection;
            worldEffect.View = camera.View.View;
            f++;
            //worldEffect.ViewVector = camera.CameraNormal;
            Vector3 centre = new Vector3(64, 64, 64);

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                World.SetCuboid(new Cuboid(new Point3(0,0,0), new Point3(64,64,64)), 
                    new BlockData(BlockManager.Log.ID));
                World.SetCuboid(new Cuboid(new Point3(1, 1, 1), new Point3(63, 63, 63)),
                    new BlockData(BlockManager.Glass.ID));
            }

            camera.UpdateMovement();
            sun.SunTick();

            base.Update(gameTime);
        }

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

            spriteBatch.DrawString(spriteFont, "FPS: " + Spine_Library.Tools.FPSHandler.getFrameRate(), new Vector2(10,10), Color.Black);
            spriteBatch.DrawString(spriteFont, "" + camera.CameraPos, new Vector2(10, 25), Color.Black);
            spriteBatch.DrawString(spriteFont, "Chunks: " + World.ChunkCount, new Vector2(10, 40), Color.Black);

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
