﻿///Represents a camera with it's own view parameters
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using BlockGame.Utilities;

namespace BlockGame.Render
{
    /// <summary>
    /// Represents a view from an orgin
    /// </summary>
    public class Camera
    {
        #region Private Variables
        /// <summary>
        /// The orgin point of the camera
        /// </summary>
        Vector3 cameraPos;
        /// <summary>
        /// The normal for the camera
        /// </summary>
        Vector3 cameraNormal;
        /// <summary>
        /// The Yaw around the z-axis
        /// </summary>
        float cameraYaw;
        /// <summary>
        /// The up/down pitch of the camera
        /// </summary>
        float cameraPitch;
        /// <summary>
        /// The graphicsdevice that this camera was initialized with
        /// </summary>
        GraphicsDevice graphicsDevice;
        #endregion

        #region Public Variables
        /// <summary>
        /// The orgin point of the camera
        /// </summary>
        public Vector3 CameraPos
        {
            get { return cameraPos; }
            set { cameraPos = value; }
        }
        /// <summary>
        /// The normal for the camera
        /// </summary>
        public Vector3 CameraNormal
        {
            get
            {
                Vector3 temp = MathUtils.FromYawPitch(cameraYaw + 90, cameraPitch);
                temp.Normalize();
                return temp;
            }
        }
        /// <summary>
        /// The left-facing vector to the camera
        /// </summary>
        public Vector3 CameraCrossNormal
        {
            get
            {
                Vector3 temp = MathUtils.FromYawPitch(cameraYaw + 90, 0);
                temp.Normalize();
                return temp;
            }
        }
        /// <summary>
        /// The Yaw around the z-axis
        /// </summary>
        public float CameraYaw
        {
            get { return cameraYaw; }
            set { cameraYaw = value.Wrap(0,360); }
        }
        /// <summary>
        /// The up/down pitch of the camera
        /// </summary>
        public float CameraPitch
        {
            get { return cameraPitch; }
            set { cameraPitch = MathHelper.Clamp(value, -89, 89); }
        }
        /// <summary>
        /// The view parameters for this camera
        /// </summary>
        public ViewParameters View;

        /// <summary>
        /// Gets the graphicsdevice that this camera was initialized with
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        BoundingFrustum frustum = new BoundingFrustum(Matrix.Identity);
        public BoundingFrustum ViewFrustum
        {
            get { return frustum; }
        }
        #endregion

        /// <summary>
        /// Creates a new camera instance
        /// </summary>
        /// <param name="Position">The camera's position</param>
        /// <param name="graphics">The GraphicsDeviceManager to use</param>
        public Camera(Vector3 Position, GraphicsDevice graphics)
        {
            this.graphicsDevice = graphics;
            this.cameraPos = Position;
            this.cameraNormal = new Vector3(1,0,0);

            View = new ViewParameters();
            View.Projection = Matrix.CreateTranslation(-0.5f, -0.5f, -0.5f) * Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60),
                (float)graphics.Viewport.Width /
                (float)graphics.Viewport.Height,
                0.1f, 1000.0f);

            View.View = Matrix.CreateLookAt(CameraPos, CameraPos + CameraNormal, new Vector3(0, 0, 1));
            View.World = Matrix.CreateTranslation(Vector3.Zero);

            frustum.Matrix = View.View * View.Projection;
        }

        /// <summary>
        /// Force update the view parameter
        /// </summary>
        public void UpdateViewParameters()
        {
            View.View = Matrix.CreateLookAt(CameraPos, CameraPos + CameraNormal, new Vector3(0, 0, 1));
            frustum.Matrix = View.View * View.Projection;
        }

        /// <summary>
        /// Handles moing the camera around using W/A/S/D and arrow keys
        /// </summary>
        public void UpdateMovement()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                CameraYaw += 2.6F;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                CameraYaw -= 2.6F;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                CameraPitch += 2.6F;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                CameraPitch -= 2.6F;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                CameraPos += CameraNormal;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                CameraPos -= CameraNormal;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                CameraPos += CameraCrossNormal;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                CameraPos -= CameraCrossNormal;

            UpdateViewParameters();
        }
    }

    /// <summary>
    /// Represents a camera's view paramaters
    /// </summary>
    public struct ViewParameters
    {
        /// <summary>
        /// The view matrix
        /// </summary>
        public Matrix View;
        /// <summary>
        /// The projection matrix
        /// </summary>
        public Matrix Projection;
        /// <summary>
        /// The world matrix
        /// </summary>
        public Matrix World;
    }
}
