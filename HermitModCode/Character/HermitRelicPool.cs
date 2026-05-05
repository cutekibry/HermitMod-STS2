using BaseLib.Abstracts;
using HermitMod.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;

namespace HermitMod.Character;

public class HermitRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Hermit.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
