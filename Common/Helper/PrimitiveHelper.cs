using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizenkleBoss.Common.Helper
{
    public struct VertexInfo2(Vector2 position, Vector3 texCoord, Color color) : IVertexType
    {
        private static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0));

        public Vector2 Position = position;
        public Color Color = color;
        public Vector3 TexCoord = texCoord;

        public readonly VertexDeclaration VertexDeclaration => _vertexDeclaration;
    }
    public class TrailData(Vector2 Position, float Rotation)
    {
        public Vector2 Position = Position;
        public float Rotation = Rotation;
    }
}
