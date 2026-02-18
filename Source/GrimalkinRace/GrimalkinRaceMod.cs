using HarmonyLib;
using System.Linq;
using Verse;

namespace GrimalkinRace
{
    public class GrimalkinRaceMod : Mod
    {
        public GrimalkinRaceMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("com.MousekinRace_Grimalkin");
            harmony.PatchAll();

            Log.Message($"Mousekin Race - Grimalkin Race submodule :: successfully applied {harmony.GetPatchedMethods().Select(Harmony.GetPatchInfo).SelectMany(selector: p => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count(predicate: p => p.owner == harmony.Id)} patches with Harmony.");
        }
    }
}
