using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.Dusts;
using WizenkleBoss.Content.Items.Dyes;

namespace WizenkleBoss.Assets.Helper
{
    public class InkPlayer : ModPlayer
    {
        public float Intoxication = 0;
        public bool InkyArtifact = false;
        public int InkDashCooldown = 0;
        public Vector2 DashVelocity;
        public bool InTile => WorldGen.SolidTile3((int)(Player.Center.X / 16), (int)(Player.Center.Y / 16));
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            InkDashCooldown = Math.Max(-15, InkDashCooldown - 1);
            if (InkKeybindSystem.InkDash.JustPressed && InkDashCooldown == -15)
            {
                if (Main.LocalPlayer.mount.Active)
                {
                    Main.LocalPlayer.mount.Dismount(Main.LocalPlayer);
                }
                InkDashCooldown = 50;
            }
        }
        public override void ResetEffects()
        {
            InkyArtifact = false;
        }
        public override void UpdateDead()
        {
            InkyArtifact = false;
        }
        public override void OnEnterWorld()
        {
            if (Player.HasBuff<InkDrugBuff>() || Player.HasBuff<InkDrugStatBuff>())
            {
                Player.ClearBuff(ModContent.BuffType<InkDrugBuff>());
                Player.ClearBuff(ModContent.BuffType<InkDrugStatBuff>());
            }
        }
        public override void PreUpdateMovement()
        {
            if (WorldGen.SolidTile3((int)(Player.Center.X / 16), (int)(Player.Center.Y / 16)) && Player.HasBuff<InkDrugStatBuff>() && InkyArtifact && InkDashCooldown > 0)
                InkDashCooldown = 30;
            if (Player.HasBuff<InkDrugStatBuff>() && InkyArtifact && InkDashCooldown > 0)
            {
                Vector2 normalized = Vector2.Zero;

                if (PlayerInput.Triggers.Current.Up)
                    normalized += new Vector2(0, -1);
                if (PlayerInput.Triggers.Current.Left)
                    normalized += new Vector2(-1, 0);
                if (PlayerInput.Triggers.Current.Right)
                    normalized += new Vector2(1, 0);
                if (PlayerInput.Triggers.Current.Down)
                    normalized += new Vector2(0, 1);

                normalized = normalized.SafeNormalize(Vector2.Zero);

                DashVelocity += normalized * 1.2f;

                DashVelocity *= 0.9f;

                Player.maxFallSpeed *= 20f;
            }
        }
        public override void PostUpdate()
        {
            if (!Player.HasBuff<InkDrugBuff>() && !Player.HasBuff<InkDrugStatBuff>())
                Intoxication = MathHelper.Clamp(Intoxication - 0.01f, 0f, 1f);
            if (Player.HasBuff<InkDrugStatBuff>() || Player.HasBuff<InkDrugBuff>())
                Player.aggro = -900;
            if (InTile && Main.rand.NextBool(3))
            {
                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
                Dust dust = Dust.NewDustPerfect(Player.Center, ModContent.DustType<InkDust>(), null, 0, Color.White, 2);
                dust.shader = shader;
            }
        }
        public void DrawGoo()
        {
            Main.spriteBatch.Draw(TextureRegistry.Bloom, new Rectangle(Main.screenWidth / 2, Main.screenHeight / 2, (int)(Main.screenWidth * Intoxication * 4.3f), (int)(Main.screenHeight * Intoxication * 3.3f)), null, (Color.White * Intoxication), 0, TextureRegistry.Bloom.Size() / 2f, SpriteEffects.None, 0f);
        }
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if ((Player.HasBuff<InkDrugStatBuff>() || Player.HasBuff<InkDrugBuff>()) && InkSystem.InsideInkTargetDrawnToThisFrame && Player.Center.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2)) < 900)
            {
                foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers)
                {
                    layer.Hide();
                }
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.HasBuff<InkDrugStatBuff>())
            {
                Player.ClearBuff(ModContent.BuffType<InkDrugStatBuff>());
                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
                for (int i = 0; i < 40; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, ModContent.DustType<InkDust>(), 0, -5, 0, Color.White, 2);
                    Main.dust[dust].shader = shader;
                }
            }
        }
    }
}
