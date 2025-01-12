using Microsoft.Xna.Framework.Graphics;
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
using WizenkleBoss.Content.Items.Dyes;
using WizenkleBoss.Content.Buffs;
using Terraria.Localization;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Content.Items.Accessories
{
    public class ExtraDimensionalArtifact : BaseInkItem
    {
        private static bool HasRun = false;
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
            if (InkKeybindSystem.InkDash.GetAssignedKeys().Count == 0 && !HasRun)
            {
                HasRun = true;
                Main.NewText(Language.GetTextValue("Mods.WizenkleBoss.Items.ExtraDimensionalArtifact.ChatMessage", InkKeybindSystem.InkDash.DisplayName.ToString()));
            }
            player.GetModPlayer<InkPlayer>().InkyArtifact = true;
            if (player.GetModPlayer<InkPlayer>().InGhostInk)
            {
                player.lavaImmune = true;
                player.shimmerImmune = true;
                player.shimmerWet = false;
                player.suffocating = false;
                player.suffocateDelay = 0;
                player.breath = player.breathMax;

                player.statDefense *= 3.5f;
                player.moveSpeed *= 3.5f;
                player.frogLegJumpBoost = true;
                player.noFallDmg = true;

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
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "Tooltip1")
                {
                    line.Text = Language.GetTextValue("Mods.WizenkleBoss.Items.ExtraDimensionalArtifact.ReplacementTooltip", InkKeybindSystem.InkDash.GetAssignedKeys().FirstOrDefault());
                }
            }
        }
    }
}
