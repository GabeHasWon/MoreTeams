using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace MoreTeams;

internal class TeamConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Range(1, 4)]
    [DefaultValue(4)]
    public int TeamPairs { get; set; }
}
