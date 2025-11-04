using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace MoreTeams;

internal class TeamConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [Range(1, 5)]
    [DefaultValue(5)]
    public int TeamPairs { get; set; }
}
