using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Chat;
using Terraria.UI.Chat;

namespace WizenkleBoss.Common.Helpers
{
    public static partial class Helper
    {
        public static readonly Vector2[] ShadowDirections = [
            -Vector2.UnitX,
            Vector2.UnitX,
            -Vector2.UnitY,
            Vector2.UnitY
        ];

        public static Vector2 GetStringSize(SpriteFont font, string text, Vector2 baseScale, float maxWidth = -1f)
        {
            TextSnippet[] snippets = [.. ChatManager.ParseMessage(text, Color.White)];
            return GetStringSize(font, snippets, baseScale, maxWidth);
        }

        public static Vector2 GetStringSize(SpriteFont font, TextSnippet[] snippets, Vector2 baseScale, float maxWidth = -1f)
        {
            Vector2 zero = Vector2.Zero;
            Vector2 vector = zero;
            Vector2 result = vector;
            float x = font.MeasureString(" ").X;
            float num2 = 0f;
            foreach (TextSnippet textSnippet in snippets)
            {
                textSnippet.Update();
                float num = textSnippet.Scale;
                if (textSnippet.UniqueDraw(justCheckingString: true, out Vector2 size, null, scale: baseScale.X * num))
                {
                    vector.X += size.X;

                    result.X = Math.Max(result.X, vector.X);
                    result.Y = Math.Max(result.Y, vector.Y + size.Y);
                    continue;
                }

                string[] array = textSnippet.Text.Split('\n');
                string[] array2 = array;
                for (int j = 0; j < array2.Length; j++)
                {
                    string[] array3 = array2[j].Split(' ');
                    for (int k = 0; k < array3.Length; k++)
                    {
                        if (k != 0)
                            vector.X += x * baseScale.X * num;

                        if (maxWidth > 0f)
                        {
                            float num3 = font.MeasureString(array3[k]).X * baseScale.X * num;
                            if (vector.X - zero.X + num3 > maxWidth)
                            {
                                vector.X = zero.X;
                                vector.Y += font.LineSpacing * num2 * baseScale.Y;
                                result.Y = Math.Max(result.Y, vector.Y);
                                num2 = 0f;
                            }
                        }

                        if (num2 < num)
                            num2 = num;

                        Vector2 vector2 = font.MeasureString(array3[k]);
                        vector.X += vector2.X * baseScale.X * num;
                        result.X = Math.Max(result.X, vector.X);
                        result.Y = Math.Max(result.Y, vector.Y + vector2.Y);
                    }

                    if (array.Length > 1 && j < array2.Length - 1)
                    {
                        vector.X = zero.X;
                        vector.Y += font.LineSpacing * num2 * baseScale.Y;
                        result.Y = Math.Max(result.Y, vector.Y);
                        num2 = 0f;
                    }
                }
            }

            return result;
        }

        public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            for (int i = 0; i < ShadowDirections.Length; i++)
            {
                DrawColorCodedString(spriteBatch, font, snippets, position + ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors: true);
            }
        }

        public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth, bool ignoreColors = false)
        {
            Vector2 vector = position;
            Vector2 result = vector;
            float x = font.MeasureString(" ").X;
            Color color = baseColor;
            float num3 = 0f;
            for (int i = 0; i < snippets.Length; i++)
            {
                TextSnippet textSnippet = snippets[i];
                textSnippet.Update();
                if (!ignoreColors)
                    color = textSnippet.GetVisibleColor();

                float num2 = textSnippet.Scale;
                if (textSnippet.UniqueDraw(justCheckingString: false, out Vector2 size, spriteBatch, vector, color, baseScale.X * num2))
                {
                    vector.X += size.X;

                    result.X = Math.Max(result.X, vector.X);
                    continue;
                }

                string[] array = Regex.Split(textSnippet.Text, "(\n)");
                bool flag = true;
                foreach (string text in array)
                {
                    string[] array2 = Regex.Split(text, "( )");
                    array2 = text.Split(' ');
                    if (text == "\n")
                    {
                        vector.Y += font.LineSpacing * num3 * baseScale.Y;
                        vector.X = position.X;
                        result.Y = Math.Max(result.Y, vector.Y);
                        num3 = 0f;
                        flag = false;
                        continue;
                    }

                    for (int k = 0; k < array2.Length; k++)
                    {
                        if (k != 0)
                            vector.X += x * baseScale.X * num2;

                        if (maxWidth > 0f)
                        {
                            float num4 = font.MeasureString(array2[k]).X * baseScale.X * num2;
                            if (vector.X - position.X + num4 > maxWidth)
                            {
                                vector.X = position.X;
                                vector.Y += font.LineSpacing * num3 * baseScale.Y;
                                result.Y = Math.Max(result.Y, vector.Y);
                                num3 = 0f;
                            }
                        }

                        if (num3 < num2)
                            num3 = num2;

                        spriteBatch.DrawString(font, array2[k], vector, color, rotation, origin, baseScale * textSnippet.Scale * num2, SpriteEffects.None, 0f);
                        Vector2 vector2 = font.MeasureString(array2[k]);

                        vector.X += vector2.X * baseScale.X * num2;
                        result.X = Math.Max(result.X, vector.X);
                    }

                    if (array.Length > 1 && flag)
                    {
                        vector.Y += (float)font.LineSpacing * num3 * baseScale.Y;
                        vector.X = position.X;
                        result.Y = Math.Max(result.Y, vector.Y);
                        num3 = 0f;
                    }

                    flag = true;
                }
            }
            return result;
        }

        public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
            return DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, maxWidth);
        }

        public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, float rotation, Color color, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
            return DrawColorCodedString(spriteBatch, font, snippets, position, color, rotation, origin, baseScale, maxWidth);
        }

        public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, float rotation, Color color, Color shadowColor, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            DrawColorCodedStringShadow(spriteBatch, font, snippets, position, shadowColor, rotation, origin, baseScale, maxWidth, spread);
            return DrawColorCodedString(spriteBatch, font, snippets, position, color, rotation, origin, baseScale, maxWidth);
        }

        public static void DrawColorCodedStringShadow(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            for (int i = 0; i < ShadowDirections.Length; i++)
            {
                DrawColorCodedString(spriteBatch, font, text, position + ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, maxWidth, ignoreColors: true);
            }
        }

        public static Vector2 DrawColorCodedString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false)
        {
            Vector2 vector = position;
            Vector2 result = vector;
            string[] array = text.Split('\n');
            float x = font.MeasureString(" ").X;
            Color color = baseColor;
            float num = 1f;
            float num2 = 0f;
            string[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                string[] array3 = array2[i].Split(':');
                foreach (string text2 in array3)
                {
                    string[] array4 = text2.Split(' ');
                    for (int k = 0; k < array4.Length; k++)
                    {
                        if (k != 0)
                            vector.X += x * baseScale.X * num;

                        if (maxWidth > 0f)
                        {
                            float num3 = font.MeasureString(array4[k]).X * baseScale.X * num;
                            if (vector.X - position.X + num3 > maxWidth)
                            {
                                vector.X = position.X;
                                vector.Y += font.LineSpacing * num2 * baseScale.Y;
                                result.Y = Math.Max(result.Y, vector.Y);
                                num2 = 0f;
                            }
                        }

                        if (num2 < num)
                            num2 = num;

                        spriteBatch.DrawString(font, array4[k], vector, color, rotation, origin, baseScale * num, SpriteEffects.None, 0f);
                        vector.X += font.MeasureString(array4[k]).X * baseScale.X * num;
                        result.X = Math.Max(result.X, vector.X);
                    }
                }

                vector.X = position.X;
                vector.Y += font.LineSpacing * num2 * baseScale.Y;
                result.Y = Math.Max(result.Y, vector.Y);
                num2 = 0f;
            }

            return result;
        }

        public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            TextSnippet[] snippets = [.. ChatManager.ParseMessage(text, baseColor)];
            ChatManager.ConvertNormalSnippets(snippets);
            DrawColorCodedStringShadow(spriteBatch, font, snippets, position, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
            return DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, maxWidth);
        }

        public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, Color shadowColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(text, baseColor).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            DrawColorCodedStringShadow(spriteBatch, font, snippets, position, shadowColor, rotation, origin, baseScale, maxWidth, spread);
            return DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, maxWidth);
        }
    }
}
