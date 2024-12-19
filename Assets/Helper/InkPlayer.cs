using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
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
        public Vector2[] dashOldPos = new Vector2[15];
        public bool InTile = false;
        public bool InGhostInk => Player.HasBuff<InkDrugStatBuff>() && InkyArtifact;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            InkDashCooldown = Math.Max(-15, InkDashCooldown - 1);
            if (InkKeybindSystem.InkDash.JustPressed && InkDashCooldown == -15)
            {
                dashOldPos = new Vector2[15];
                if (Main.LocalPlayer.mount.Active)
                {
                    Main.LocalPlayer.mount.Dismount(Main.LocalPlayer);
                }
                InkDashCooldown = 50;
                DashVelocity = Player.velocity;
                if (Player.IsOnGroundPrecise())
                {
                    DashVelocity += Vector2.UnitY * 10f;
                }
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
            if (InTile && InGhostInk && InkDashCooldown > 0)
                InkDashCooldown = 30;
            if (InGhostInk && InkDashCooldown > 0)
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

                DashVelocity *= 0.92f;

                DashVelocity = DashVelocity.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(DashVelocity.Length(), 4f, 300f);


                Player.velocity = DashVelocity;
                Player.maxFallSpeed *= 20f;

                if (!InTile)
                {
                    DashVelocity.Y += Player.gravity;
                }
            }
        }
        public override void PostUpdate()
        {
            if (!Player.HasBuff<InkDrugBuff>() && !Player.HasBuff<InkDrugStatBuff>())
                Intoxication = MathHelper.Clamp(Intoxication - 0.01f, 0f, 1f);
            if (Player.HasBuff<InkDrugStatBuff>() || Player.HasBuff<InkDrugBuff>())
                Player.aggro = -900;
            if (Player.whoAmI != Main.myPlayer)
                return;

            if (InGhostInk)
            {
                for (int i = dashOldPos.Length - 2; i >= 0; i--)
                {
                    dashOldPos[i + 1] = dashOldPos[i];
                }
                dashOldPos[0] = Player.Center;

                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
                bool lastframecheck = InTile;
                if (WorldGen.SolidTile3((int)(Player.Center.X / 16), (int)(Player.Center.Y / 16)))
                    InTile = true;
                else 
                    InTile = false;
                if (lastframecheck != InTile) 
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 vel = InTile ? (-Vector2.Normalize(DashVelocity)).RotatedByRandom(MathHelper.PiOver2) : Vector2.Normalize(DashVelocity).RotatedByRandom(MathHelper.PiOver2);
                        Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-16f, 17f)), ModContent.DustType<InkDust>(), vel * 5f, 0, Color.White, 2.2f);
                        dust.shader = shader;
                    }
                }

                MusicKiller.MuffleFactor = 0.2f;
                if (InTile && InkDashCooldown > 0)
                {
                    this.CameraShakeSimple(Player.position, Vector2.Zero, 2.4f, 11, 2, 0);
                    Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-16f, 17f)), ModContent.DustType<InkDust>(), null, 0, Color.White, 2.2f);
                    dust.shader = shader;
                }
                if (!InTile && Main.rand.NextBool(5) && InkDashCooldown > 0)
                {
                    Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-4f, 5f), Main.rand.NextFloat(-4f, 5f)), ModContent.DustType<InkDust>(), null, 0, Color.White, 1.5f);
                    dust.shader = shader;
                }
            }
            else
            {
                InTile = false;
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
