﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Block_Game.Render
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
        /// The left-facing vector to the camera
        /// </summary>
        Vector3 cameraCrossNormal;
        /// <summary>
        /// The Yaw around the z-axis
        /// </summary>
        float cameraYaw;
        /// <summary>
        /// The up/down pitch of the camera
        /// </summary>
        float cameraPitch;
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
                Vector3 temp = Spine_Library.Tools.extraMath.getVector3(new Vector2(CameraYaw, CameraPitch), 1);
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
                Vector3 temp = Spine_Library.Tools.extraMath.getVector3(new Vector2(cameraYaw + 45, 0), 1);
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
            set { cameraYaw = MathHelper.Clamp(value, -90, 90); }
        }
        /// <summary>
        /// The up/down pitch of the camera
        /// </summary>
        public float CameraPitch
        {
            get { return cameraPitch; }
            set { cameraPitch = MathHelper.Clamp(value, -90, 90); }
        }
        /// <summary>
        /// The view parameters for this camera
        /// </summary>
        public ViewParameters View;
        #endregion

        public Camera(Vector3 Position, GraphicsDeviceManager graphics)
        {
            this.cameraPos = Position;
            this.cameraNormal = new Vector3(1,0,0);

            View = new ViewParameters();
            View.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60),
                (float)graphics.GraphicsDevice.Viewport.Width /
                (float)graphics.GraphicsDevice.Viewport.Height,
                0.1f, 1000.0f);

            View.View = Matrix.CreateLookAt(CameraPos, CameraPos + CameraNormal, new Vector3(0, 0, 1));
            View.World = Matrix.CreateTranslation(Vector3.Zero);
        }

        public void UpdateViewParameters()
        {
            View.View = Matrix.CreateLookAt(CameraPos, CameraPos + CameraNormal, new Vector3(0, 0, 1));
        }

        public void UpdateMovement()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraYaw += 0.03F;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                cameraYaw -= 0.03F;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                cameraPitch += 0.03F;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                cameraPitch -= 0.03F;

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
