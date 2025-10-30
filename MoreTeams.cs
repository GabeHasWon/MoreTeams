global using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Gamepad;

namespace MoreTeams;

public class MoreTeams : Mod
{
    private static Asset<Texture2D> Icons = null;
    private static Asset<Texture2D> Swords = null;

    public override void Load()
    {
        Icons = ModContent.Request<Texture2D>("MoreTeams/TeamIcons");
        Swords = ModContent.Request<Texture2D>("MoreTeams/SwordIcons");

        On_Main.DrawPVPIcons += DrawNewPvPIcons;
    }

    private void DrawNewPvPIcons(On_Main.orig_DrawPVPIcons orig)
    {
        orig();

        int size = (int)(52f * Main.inventoryScale);
        int x = 707 - size * 4 + Main.screenWidth - 800;
        int y = 114 + GetMH(null) + size * 2 + size / 2 - 12;

        if (Main.EquipPage == 2)
            x += size + size / 2;

        int swordBaseFrame = (Main.LocalPlayer.hostile ? 2 : 0);

        if (Main.mouseX > x - 7 && Main.mouseX < x + 25 && Main.mouseY > y - 2 && Main.mouseY < y + 37 && !PlayerInput.IgnoreMouseInterface)
        {
            Main.LocalPlayer.mouseInterface = true;

            if (Main.teamCooldown == 0)
                swordBaseFrame++;
        }

        Rectangle rectangle = Swords.Frame(4, 8);
        rectangle.Location = new Point(rectangle.Width * swordBaseFrame, rectangle.Height * (Main.LocalPlayer.team - 6));
        rectangle.Width -= 2;
        rectangle.Height--;

        if (Main.LocalPlayer.team > 5)
        {
            Main.spriteBatch.Draw(Swords.Value, new Vector2(x - 10, y), rectangle, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }

        x -= 10;
        y += 120;

        rectangle = Icons.Frame(6);
        Rectangle r = rectangle;

        for (int i = 0; i < ModContent.GetInstance<TeamConfig>().TeamPairs * 2; i++)
        {
            int teamId = i + 6;
            r.Location = new Point(x + i % 2 * 20, y + i / 2 * 20);
            rectangle.X = r.Width * i;
            bool flag = false;

            if (r.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;

                if (Main.teamCooldown == 0)
                    flag = true;

                if (Main.mouseLeft && Main.mouseLeftRelease && Main.LocalPlayer.team != teamId && Main.teamCooldown == 0)
                {
                    if (!Main.LocalPlayer.TeamChangeAllowed())
                        Main.NewText(Lang.misc[84].Value, byte.MaxValue, 240, 20);
                    else
                    {
                        Main.teamCooldown = Main.teamCooldownLen;
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        Main.LocalPlayer.team = teamId;
                        SyncTeam(Main.myPlayer);
                    }
                }
            }

            r.Width -= 2;

            if (flag)
                Main.spriteBatch.Draw(TextureAssets.Pvp[2].Value, r.Location.ToVector2() + new Vector2(-2f), Color.White);

            Rectangle value = rectangle;
            value.Width -= 2;
            Main.spriteBatch.Draw(Icons.Value, r.Location.ToVector2(), new Rectangle(18 * i, 0, 16, 16), Color.White);
            UILinkPointNavigator.SetPosition(1550 + i + 1, r.Location.ToVector2() + r.Size() * 0.75f);
        }
    }

    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "mH")]
    private static extern ref int GetMH(Main main);

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        byte type = reader.ReadByte();

        if (type == 0)
        {
            int who = reader.ReadByte();

            if (Main.netMode == NetmodeID.Server)
                who = whoAmI;

            int teamToJoin = reader.ReadByte();
            Player plr = Main.player[who];
            int team = plr.team;
            plr.team = teamToJoin;
            Color color = Main.teamColor[teamToJoin];

            if (Main.netMode != NetmodeID.Server)
                return;

            SyncTeam(who, whoAmI);
            LocalizedText localizedText = Language.GetText("Mods.MoreTeams.JoinedTeam");

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (i == whoAmI || team > 0 && Main.player[i].team == team || teamToJoin > 0 && Main.player[i].team == teamToJoin)
                {
                    string teamName = Language.GetTextValue("Mods.MoreTeams.Teams." + (teamToJoin - 6));
                    ChatHelper.SendChatMessageToClient(NetworkText.FromKey(localizedText.Key, plr.name, teamName), color, i);
                }
            }

            return;
        }
    }

    public static void SyncTeam(int who, int ignoreWho = -1)
    {
        ModPacket packet = ModContent.GetInstance<MoreTeams>().GetPacket(3);
        packet.Write((byte)0);
        packet.Write((byte)who);
        packet.Write((byte)Main.player[who].team);
        packet.Send(-1, ignoreWho);
    }
}
