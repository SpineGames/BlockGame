﻿///A User Iterface manager originally created for Block Game
///© 2013 Spine Games

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BlockGame.Utilities;

namespace BlockGame.UI
{
    public abstract class UIElement
    {
        string refName;
        public string RefName
        {
            get { return refName; }
            set { refName = value; }
        }
        /// <summary>
        /// This elements position relative to the top-left corner of the UI manager
        /// </summary>
        public Vector2 Position;
        public abstract Vector2 Size { get; }
        public Vector2 MaxPos { get { return Position + Size; } }

        public abstract void Render(SpriteBatch spriteBatch, Vector2 pos);
    }

    public class UIE_String : UIElement
    {
        public override Vector2 Size
        {
            get
            {
                Vector2 tSize = Font.MeasureString(" ");
                return new Vector2(tSize.X * largestSize,  tSize.Y);
            }
        }

        private string _text = "";
        public string Format = " ";
        public SpriteFont Font;
        public readonly Color Color;
        int largestSize = 0;

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public UIE_String(SpriteFont font, string format, Color color, string refName = null)
        {
            this.Format = format;
            this.Font = font;
            this.Color = color;
            this.RefName = refName;

            _text = Format;
            largestSize = _text.Length;
        }
        
        public override void Render(SpriteBatch batch, Vector2 pos)
        {
            batch.DrawString(Font, _text, Position + pos, Color);
        }
    }

    public class UIManager : UIElement
    {
        static Texture2D blankTex;
        Dictionary<string, UIElement> elements = new Dictionary<string, UIElement>();
        bool RenderBackGround = false;
        Color color;
        public Color Color { 
            get { return color; }
            set { color = value; RenderBackGround = true; color.A = (byte)(alpha * 255); }
        }
        public Vector2 Margine;
        public override Vector2 Size
        {
            get
            {
                Vector2 maxSize = Position;
                foreach (UIElement e in elements.Values)
                {
                    if (e.MaxPos.X > maxSize.X)
                        maxSize.X = e.MaxPos.X;

                    if (e.MaxPos.Y > maxSize.Y)
                        maxSize.Y = e.MaxPos.Y;
                }
                return maxSize + (Margine * 2);
            }
        }
        public Vector2 ContentSize
        {
            get
            {
                Vector2 maxSize = Position;
                foreach (UIElement e in elements.Values)
                {
                    if ((e.MaxPos - Margine).X > maxSize.X)
                        maxSize.X = e.MaxPos.X;

                    if ((e.MaxPos - Margine).Y > maxSize.Y)
                        maxSize.Y = e.MaxPos.Y;
                }
                return maxSize;
            }
        }
        public float alpha = 0.1F;
        public bool Show = true;

        public UIElement this[string refName]
        {
            get { return elements[refName]; }
        }

        public static void Initialize(Texture2D blank)
        {
            blankTex = blank;
        }

        public UIManager(Vector2 position, Vector2 margine)
        {
            this.Position = position;
            this.Margine = margine;
        }

        public UIManager(Vector2 position, Vector2 margine, Color color)
        {
            this.Position = position;
            this.Margine = margine;
            this.Color = color;
        }

        public void AddElementLeftAlign(UIElement element)
        {
            element.Position = new Vector2(0, ContentSize.Y);
            elements.Add(element.RefName, element);
        }

        public override void Render(SpriteBatch spriteBatch, Vector2 position)
        {
            if (Show)
            {
                if (RenderBackGround)
                {
                    spriteBatch.Draw(blankTex, new Rectangle((int)Position.X, (int)Position.Y, (int)MaxPos.X, (int)MaxPos.Y), Color);
                }
                foreach (UIElement e in elements.Values)
                {
                    e.Render(spriteBatch, Position + Margine);
                }
            }
        }
    }
}
