using BaseLib.Abstracts;
using BaseLib.Extensions;
using HermitMod.Extensions;
using Godot;

namespace HermitMod.Powers;

public abstract class HermitPower : CustomPowerModel
{
    private string IconFileName =>
        Id.Entry.RemovePrefix().ToLowerInvariant().Replace("_", "") + ".png";

    public override string? CustomPackedIconPath
    {
        get
        {
            var path = IconFileName.PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "concentration.png".PowerImagePath();
        }
    }

    public override string? CustomBigIconPath
    {
        get
        {
            var path = IconFileName.BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "concentration.png".BigPowerImagePath();
        }
    }
}
