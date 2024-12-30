using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#nullable enable
namespace WizenkleBoss.Common.Helper
{
    public static class SpriteBatchSnapshotCache
    {
        private const BindingFlags SBBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        internal static FieldInfo _sortModeField, _blendStateField, _samplerStateField, _depthStencilStateField, _rasterizerStateField, _effectField, _transformMatrixField;
        internal static FieldInfo SortModeField => _sortModeField ??= typeof(SpriteBatch).GetField("sortMode", SBBindingFlags);
        internal static FieldInfo BlendStateField => _blendStateField ??= typeof(SpriteBatch).GetField("blendState", SBBindingFlags);
        internal static FieldInfo SamplerStateField => _samplerStateField ??= typeof(SpriteBatch).GetField("samplerState", SBBindingFlags);
        internal static FieldInfo DepthStencilStateField => _depthStencilStateField ??= typeof(SpriteBatch).GetField("depthStencilState", SBBindingFlags);
        internal static FieldInfo RasterizerStateField => _rasterizerStateField ??= typeof(SpriteBatch).GetField("rasterizerState", SBBindingFlags);
        internal static FieldInfo EffectField => _effectField ??= typeof(SpriteBatch).GetField("customEffect", SBBindingFlags);
        internal static FieldInfo TransformMatrixField => _transformMatrixField ??= typeof(SpriteBatch).GetField("transformMatrix", SBBindingFlags);

        public static void Begin(this SpriteBatch spriteBatch, in SpriteBatchSnapshot snapshot)
        {
            spriteBatch.Begin(snapshot.sortMode, snapshot.blendState, snapshot.samplerState, snapshot.depthStencilState, snapshot.rasterizerState, snapshot.effect, snapshot.transformationMatrix);
        }

        /// <inheritdoc cref="SpriteBatchSnapshot.Capture(SpriteBatch)"/>
        public static SpriteBatchSnapshot CaptureSnapshot(this SpriteBatch spriteBatch)
        {
            return SpriteBatchSnapshot.Capture(spriteBatch);
        }

        class Loader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
            }

            void ILoadable.Unload()
            {
                _sortModeField = null;
                _blendStateField = null;
                _samplerStateField = null;
                _depthStencilStateField = null;
                _rasterizerStateField = null;
                _effectField = null;
                _transformMatrixField = null;
            }
        }
    }
    /// <summary>Contains the data for a <see cref="SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix)"/> call.</summary>
    public struct SpriteBatchSnapshot
    {
        private static readonly Matrix identityMatrix = Matrix.Identity;
        public SpriteSortMode sortMode;
        public BlendState blendState;
        public SamplerState samplerState;
        public DepthStencilState depthStencilState;
        public RasterizerState rasterizerState;
        public Effect? effect;
        public Matrix transformationMatrix;

        public SpriteBatchSnapshot(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState? blendState = null, SamplerState? samplerState = null, DepthStencilState? depthStencilState = null, RasterizerState? rasterizerState = null, Effect? effect = null, Matrix? transformationMatrix = null)
        {
            this.sortMode = sortMode;
            this.blendState = blendState ?? BlendState.AlphaBlend;
            this.samplerState = samplerState ?? SamplerState.LinearClamp;
            this.depthStencilState = depthStencilState ?? DepthStencilState.None;
            this.rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
            this.effect = effect;
            this.transformationMatrix = transformationMatrix ?? identityMatrix;
        }

        /// <summary>Calls <seealso cref="SpriteBatch.Begin(SpriteSortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix)"/> with the data on this <seealso cref="SpriteBatchSnapshot"/> instance.</summary>
        /// <param name="spriteBatch">The spritebatch to begin.</param>
        public readonly void Begin(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformationMatrix);
        }

        public static SpriteBatchSnapshot Capture(SpriteBatch spriteBatch)
        {
            SpriteSortMode sortMode = (SpriteSortMode)SpriteBatchSnapshotCache.SortModeField.GetValue(spriteBatch);
            BlendState blendState = (BlendState)SpriteBatchSnapshotCache.BlendStateField.GetValue(spriteBatch);
            SamplerState samplerState = (SamplerState)SpriteBatchSnapshotCache.SamplerStateField.GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)SpriteBatchSnapshotCache.DepthStencilStateField.GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)SpriteBatchSnapshotCache.RasterizerStateField.GetValue(spriteBatch);
            Effect effect = (Effect)SpriteBatchSnapshotCache.EffectField.GetValue(spriteBatch);
            Matrix transformMatrix = (Matrix)SpriteBatchSnapshotCache.TransformMatrixField.GetValue(spriteBatch);

            return new SpriteBatchSnapshot(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        // void Revalidate()
        // {
        //     blendState ??= BlendState.AlphaBlend;
        //     samplerState ??= SamplerState.LinearClamp;
        //     depthStencilState ??= DepthStencilState.None;
        //     rasterizerState ??= RasterizerState.CullCounterClockwise;
        // }
    }
}
