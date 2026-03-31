using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using HermitMod.Epochs;
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

    public override IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)
    {
        var list = AllRelics.ToList();

        if (!unlockState.IsEpochRevealed<Hermit3Epoch>())
        {
            list.RemoveAll(r => Hermit3Epoch.Relics.Any(relic => relic.Id == r.Id));
        }

        if (!unlockState.IsEpochRevealed<Hermit6Epoch>())
        {
            list.RemoveAll(r => Hermit6Epoch.Relics.Any(relic => relic.Id == r.Id));
        }

        return list;
    }
}
