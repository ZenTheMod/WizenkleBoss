﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Shaders;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.ILDetourSystems;
using WizenkleBoss.Common.Packets;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.Dusts;
using WizenkleBoss.Content.Items.Dyes;

namespace WizenkleBoss.Common.Ink
{
    public class InkPlayer : ModPlayer
    {
        public float Intoxication = 0;
        public bool InkyArtifact = false;
        public int InkDashCooldown = -60;
        public Vector2 DashVelocity;
        public Vector2[] dashOldPos = new Vector2[15];

        private int timer = 0;

        private bool _InTile;

        public bool InTile
        {
            get { return _InTile; }
            set
            {
                if (value != _InTile && Player.whoAmI == Main.myPlayer)
                {
                    this.CameraShakeSimple(Player.position, Vector2.Zero, 10f, 19, 11, 0);
                    SoundEngine.PlaySound(value ? AudioRegistry.InkEnterTile : AudioRegistry.InkExitTile, null);
                    ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 vel = value ? -Vector2.Normalize(DashVelocity).RotatedByRandom(0.6f) : Vector2.Normalize(DashVelocity).RotatedByRandom(0.6f);
                        Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-16f, 17f)), ModContent.DustType<InkDust>(), vel * Main.rand.NextFloat(1f, 10f), 0, Color.White, 1f);
                        dust.shader = shader;
                    }
                }
                _InTile = value;
            }
        }

        private bool _InkBuffActive;

        public bool InkBuffActive
        {
            get { return _InkBuffActive; }
            set
            {
                if (value != _InkBuffActive && value == false && Player.whoAmI == Main.myPlayer)
                {
                    SoundEngine.PlaySound(AudioRegistry.InkEffectEnd, null);
                }
                _InkBuffActive = value;
            }
        }

        public bool InGhostInk => Player.HasBuff<InkDrugStatBuff>() && InkyArtifact;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (InkKeybindSystem.InkDash.JustPressed && InkDashCooldown > 0 && InGhostInk && !InTile)
            {
                this.CameraShakeSimple(Player.position, Vector2.Zero, 6f, 11, 6, 0);
                InkDashCooldown = 0;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var SendDash = new DashUpdate(Player.whoAmI, InkDashCooldown, DashVelocity);
                    SendDash.Send(ignoreClient: Main.myPlayer, runLocally: false);
                }
                return;
            }
            if (InkKeybindSystem.InkDash.JustPressed && InkDashCooldown == -60 && InGhostInk)
            {
                if (Player.whoAmI == Main.myPlayer)
                    SoundEngine.PlaySound(AudioRegistry.InkDash, null);
                dashOldPos = new Vector2[15];
                if (Player.mount.Active)
                {
                    Player.mount.Dismount(Player);
                }
                InkDashCooldown = 160;
                DashVelocity = Player.velocity;
                if (Player.IsOnGroundPrecise())
                {
                    DashVelocity += Vector2.UnitY * 10f;
                }
                this.CameraShakeSimple(Player.position, Vector2.Zero, 13f, 19, 15, 0);
                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
                for (int i = 0; i < 15; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                    Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-16f, 17f)), ModContent.DustType<InkDust>(), vel * 7f, 0, Color.White, 1f);
                    dust.shader = shader;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var SendDash = new DashUpdate(Player.whoAmI, InkDashCooldown, DashVelocity);
                    SendDash.Send(ignoreClient: Main.myPlayer, runLocally: false);
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
                InkDashCooldown = 90;
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

                DashVelocity += normalized * 1.4f;

                DashVelocity *= 0.92f;

                DashVelocity = DashVelocity.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(DashVelocity.Length(), 4f, 300f);


                Player.velocity = DashVelocity;
                Player.maxFallSpeed *= 20f;

                if (!InTile)
                {
                    DashVelocity.Y += 0.5f;
                }
            }
        }

        public override void PostUpdate()
        {
            InkDashCooldown = Math.Max(-60, InkDashCooldown - 1);
            if (Player.whoAmI == Main.myPlayer)
            {
                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
                if (InkDashCooldown == -1)
                {
                    SoundEngine.PlaySound(AudioRegistry.InkDashEnd, null);
                    for (int i = 0; i < 15; i++)
                    {
                        Vector2 vel = Vector2.Normalize(DashVelocity).RotatedByRandom(0.6f);
                        Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-16f, 17f)), ModContent.DustType<InkDust>(), vel * Main.rand.NextFloat(3f, 13f), 0, Color.White, 1f);
                        dust.shader = shader;
                    }
                }
                if (InkDashCooldown == -52)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                        Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-16f, 17f), Main.rand.NextFloat(-16f, 17f)), ModContent.DustType<InkDust>(), vel * 8f, 0, Color.White, 1f);
                        dust.shader = shader;
                    }
                }
            }

            InkBuffActive = Player.HasBuff<InkDrugStatBuff>() || Player.HasBuff<InkDrugBuff>();
            if (!InkBuffActive || !InkBuffActive && Player.dead)
                Intoxication = MathHelper.Clamp(Intoxication - 0.01f, 0f, 1f);
            if (InkBuffActive)
                Player.aggro = -900;

            if (InGhostInk)
            {
                for (int i = dashOldPos.Length - 2; i >= 0; i--)
                {
                    dashOldPos[i + 1] = dashOldPos[i];
                }
                dashOldPos[0] = Player.Center;

                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());

                if ((Collision.SolidCollision(Player.position, (int)Player.Size.X, (int)Player.Size.Y) ||
                    Collision.WetCollision(Player.position, (int)Player.Size.X, (int)Player.Size.Y)) && InkDashCooldown > 0)
                    InTile = true;
                else
                    InTile = false;
                if (Player.whoAmI == Main.myPlayer)
                {
                    MusicKiller.MuffleFactor = 0.0f;
                    if (InkDashCooldown > 0)
                    {
                        CrashIntoNPCs();
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            var SendDash = new DashUpdate(Player.whoAmI, InkDashCooldown, DashVelocity);
                            SendDash.Send(ignoreClient: Main.myPlayer, runLocally: false);
                        }
                        if (InTile)
                        {
                            if (timer++ >= 20)
                            {
                                SoundEngine.PlaySound(AudioRegistry.InkBurrowing, null);
                                timer = 0;
                            }
                            this.CameraShakeSimple(Player.position, Vector2.Zero, 2.4f, 11, 2, 0);
                        }
                        if (!InTile && Main.rand.NextBool(5) && InkDashCooldown > 0)
                        {
                            Dust dust = Dust.NewDustPerfect(Player.Center + new Vector2(Main.rand.NextFloat(-4f, 5f), Main.rand.NextFloat(-4f, 5f)), ModContent.DustType<InkDust>(), null, 0, Color.White, 1f);
                            dust.shader = shader;
                        }
                    }
                }
                if (InkSystem.AnyActiveInk && InTile)
                    ProduceInkRipples();
            }
            else
            {
                InTile = false;
            }
        }

        public void CrashIntoNPCs()
        {
            if (DashVelocity.Length() < 8f)
                return;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.ModNPC is not ITakeDamageViaInkDash dash)
                    continue;
                if (n.Hitbox.Contains((int)Player.Center.X, (int)Player.Center.Y))
                {
                    int dashDamage = (int)(DashVelocity.Length() * 30);
                    int damage = n.SimpleStrikeNPC(dashDamage, 0, noPlayerInteraction: false);
                    dash.OnDashedInto(damage, Player.velocity, Player);
                    DashVelocity = -DashVelocity * 0.95f;
                    return;
                }
            }
        }

        private void ProduceInkRipples()
        {
            InkRippleSystem.QueueRipple(Vector2.Lerp(Player.Center, dashOldPos[1], 0.5f), 0.95f, Vector2.One * 0.7f, 0.16f);
            InkRippleSystem.QueueRipple(Player.Center, 0.8f, Vector2.One * 0.65f, 0.16f);
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (InkBuffActive && InkSystem.InsideInkTargetDrawnToThisFrame && Player.Center.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2)) < 900)
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
