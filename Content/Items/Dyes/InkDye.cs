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
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Content.Items.Dyes
{
    public class InkDye : BaseInkItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GameShaders.Armor.BindShader(
                    Item.type,
                    new InkDyeArmorShaderData(ModContent.Request<Effect>("WizenkleBoss/Assets/Effects/Shaders/BarrierButSingleLayer"), "AutoloadPass")
                );
            }

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Overlay");
            }
            int dye = Item.dye;
            Item.CloneDefaults(ItemID.GelDye);
            Item.rare = ModContent.RarityType<InkRarity>();
            Item.width = 16;
            Item.height = 24;
            Item.dye = dye;
        }
    }
    public class InkDyeArmorShaderData : ArmorShaderData
    {
        public InkDyeArmorShaderData(Asset<Effect> shader, string passName)
            : base(shader, passName)
        {
        }
        public override void Apply(Entity entity, DrawData? drawData = null)
        {
            Shader.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());

                // For whatever reason doing it like this doesnt break the dust rendering?
            if (drawData.HasValue)
            {
                DrawData value = drawData.Value;
                Shader.Parameters["Size"]?.SetValue(new Vector2(value.texture.Width, value.texture.Height));
            }

            Shader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            base.Apply(entity, drawData);
        }
    }
}
