﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Content.Items.Dyes;
using WizenkleBoss.Content.Buffs;

namespace WizenkleBoss.Content.Items.Accessories
{
    public class ExtraDimensionalArtifact : BaseInkItem
    {
        public override void SetDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Overlay");
            }
            Item.rare = ModContent.RarityType<InkRarity>();
            Item.width = 30;
            Item.height = 24;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 60);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<InkPlayer>().InkyArtifact = true;
            if (player.HasBuff<InkDrugStatBuff>() && player.GetModPlayer<InkPlayer>().InkyArtifact)
            {
                player.statDefense *= 3.5f;
                player.moveSpeed *= 3.5f;
                player.frogLegJumpBoost = true;
                if (player.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
                {
                    if (player.GetModPlayer<InkPlayer>().InTile)
                        player.gravity = 0f;
                    else
                        player.gravity *= 0.5f;
                    player.dash = 0;
                    Player.jumpHeight = 0;
                    player.dashType = 0;
                    player.noKnockback = true;
                }
            }
        }
    }
}
