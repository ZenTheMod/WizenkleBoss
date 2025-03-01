﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.ILDetourSystems;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Content.Dusts;
using WizenkleBoss.Content.Items.Dyes;

namespace WizenkleBoss.Content.NPCs.InkMeteorite
{
    public class Crashed : ModNPC, IMinableNPC, IDrawInInk, ITakeDamageViaInkDash
    {
        #region generics
        public static Asset<Texture2D> outline;
        public override void Load()
        {
            outline = ModContent.Request<Texture2D>(Texture + "Outline");
        }
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.width = 58;
            NPC.height = 80;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 380;
            NPC.lifeMax = 1500;
            NPC.HitSound = SoundID.Tink;

            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.behindTiles = true;

            NPC.knockBackResist = 0f;
        }
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool CanBeHitByNPC(NPC attacker) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        #endregion

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = 0;
            if (NPC.life < NPC.lifeMax * 0.8f)
                NPC.frame.Y = 1 * frameHeight;
            if (NPC.life < NPC.lifeMax * 0.5f)
                NPC.frame.Y = 2 * frameHeight;
            if (NPC.life < NPC.lifeMax * 0.3f)
                NPC.frame.Y = 3 * frameHeight;
        }

        public bool hovering;

        public float shake = 0f;

        public override void AI()
        {
            shake = Math.Clamp(shake - 0.1f, 0f, 1f);
            hovering = NPC.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y)
                && (Main.LocalPlayer.Center - Main.MouseWorld).LengthSquared() <= Math.Pow((Main.LocalPlayer.blockRange + Main.LocalPlayer.HeldItem.tileBoost) * 16, 2);
        }

        #region particles
        public void OnMined(int damageDone, Player player)
        {
            shake = 1f;
            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-30f, 31f)), ModContent.DustType<InkDust>(), vel * 5f, 0, Color.White, 1f);
                dust.shader = shader;
            }
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(17f, 17f);

                    Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-16f, 17f)), ModContent.DustType<InkRubble>(), vel, 0, Color.White, 1f);
                    dust.shader = shader;
                }
            }
        }

        public void OnDashedInto(int damageDone, Vector2 incomingVelocity, Player player)
        {
        }
        #endregion

        #region draw
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 position = NPC.Center - screenPos;
            position.X += MathF.Sin(Main.GlobalTimeWrappedHourly * 120) * 4.5f * shake;

            Main.spriteBatch.Draw(TextureAssets.Npc[Type].Value, position, NPC.frame, Color.White, 0f, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);

            var snapshit = Main.spriteBatch.CaptureSnapshot();


                // idfc theres only one on screen anyway, fuck you <3
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, snapshit.transformationMatrix);

            var barrierShader = Helper.ObjectBarrierShader;
            barrierShader.Value.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());
            barrierShader.Value.Parameters["Size"]?.SetValue(TextureAssets.Npc[Type].Value.Size());
            barrierShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            barrierShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(TextureAssets.Npc[Type].Value, position, NPC.frame, Color.White, 0f, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);

            if (hovering)
                Main.spriteBatch.Draw(outline.Value, position, NPC.frame, Main.OurFavoriteColor, 0f, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(in snapshit);

            return false;
        }

        public void Shape(Vector2 screenPosition)
        {
            Vector2 position = NPC.Center - screenPosition;
            position.X += MathF.Sin(Main.GlobalTimeWrappedHourly * 120) * 3 * shake;

            Main.spriteBatch.Draw(TextureAssets.Npc[Type].Value, position, NPC.frame, Color.White, 0f, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(outline.Value, position, NPC.frame, Main.OurFavoriteColor, 0f, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
        }
        #endregion
    }
}
