using Terraria;
using Terraria.ModLoader;

namespace MoreTeams;

internal class ExpandTeamColors : ModSystem
{
    public static readonly Color Gray = new(82, 76, 105);
    public static readonly Color Orange = new(188, 114, 58);
    public static readonly Color Teal = new(97, 167, 147);
    public static readonly Color Pink = new(255, 155, 182);
    public static readonly Color Black = new(15, 15, 15);
    public static readonly Color Lime = new(159, 255, 66);
    public static readonly Color Brown = new(173, 115, 84);
    public static readonly Color Aquamarine = new(44, 140, 158);

    public override void ResizeArrays() => Main.teamColor = [.. Main.teamColor, Gray, Orange, Teal, Pink, Black, Lime, Brown, Aquamarine];

    public override void Unload()
    {
        ref Color[] teamColor = ref Main.teamColor;

        teamColor = new Color[6];
        teamColor[0] = Color.White;
        teamColor[1] = new Color(218, 59, 59);
        teamColor[2] = new Color(59, 218, 85);
        teamColor[3] = new Color(59, 149, 218);
        teamColor[4] = new Color(242, 221, 100);
        teamColor[5] = new Color(224, 100, 242);
    }
}
