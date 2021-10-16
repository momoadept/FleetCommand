using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class ItemType
        {
            public string Type { get; }
            public string Subtype { get; }

            public ItemType(string qualifier)
            {
                var terms = qualifier.Split('.');
                if (terms.Length < 1)
                    throw new Exception("Invalid item type qualifier " + qualifier);

                var type = terms.Length > 1 ? terms[0] : null;
                var subtype = terms.Last();

                Type = type == null ? null : Types.FirstOrDefault(t => t.ToUpper().Contains(type.ToUpper()));
                Subtype = Subtypes.FirstOrDefault(t => t.ToUpper().Contains(subtype.ToUpper()));

                if (Type == null && NotUniqueTypes.Contains(Subtype))
                {
                    Type = "Ingot";
                }
            }

            public ItemType(string subtype, string type)
            {
                Type = type == null ? null : Types.FirstOrDefault(t => t.ToUpper().Contains(type.ToUpper()));
                Subtype = Subtypes.FirstOrDefault(t => t.ToUpper().Contains(subtype.ToUpper()));

                if (Type == null && NotUniqueTypes.Contains(Subtype))
                {
                    Type = "Ingot";
                }
            }

            public ItemType(MyInventoryItem item)
            {
                Type = Types.FirstOrDefault(t => item.Type.ToString().ToUpper().Contains(t.ToUpper()));
                Subtype = Subtypes.FirstOrDefault(t => item.Type.ToString().ToUpper().Contains(t.ToUpper()));

                if (Type == null && NotUniqueTypes.Contains(Subtype))
                {
                    Type = "Ingot";
                }
            }

            public override string ToString()
            {
                if (Type == null && NotUniqueTypes.Contains(Subtype))
                {
                    return $"Ingot.{Subtype}";
                }

                return $"{Type}.{Subtype}";
            }

            public string ToDisplayString()
            {
                if (NotUniqueTypes.Contains(Subtype))
                {
                    return $"{Subtype} ({Type ?? "Ingot"})";
                }

                return $"{Subtype}";
            }

            public static string[] Types = new[]
            {
                "AmmoMagazine",
                "Component",
                "Ingot",
                "Ore",
                "ConsumableItem",
                "PhysicalGunObject",
                "OxygenContainerObject",
                "GasContainerObject",
            };

            public static string[] NotUniqueTypes = new[]
            {
                "Cobalt",
                "Gold",
                "Iron",
                "Magnesium",
                "Nickel",
                "Platinum",
                "Silicon",
                "Silver",
                "Uranium",
            };

            public static string[] Subtypes = new[]
            {
                "BulletproofGlass",
                "Canvas",
                "Computer",
                "Construction",
                "Detector",
                "Display",
                "Explosives",
                "Girder",
                "GravityGenerator",
                "InteriorPlate",
                "LargeTube",
                "Medical",
                "MetalGrid",
                "Motor",
                "PowerCell",
                "RadioCommunication",
                "Reactor",
                "SmallTube",
                "SolarCell",
                "SteelPlate",
                "Superconductor",
                "Thrust",
                "ZoneChip",
                "Hydrogen",
                "Oxygen",
                "Cobalt",
                "Gold",
                "Stone",
                "Iron",
                "Magnesium",
                "Nickel",
                "Scrap",
                "Platinum",
                "Silicon",
                "Silver",
                "Uranium",
                "Ice",
                "Organic",
                "Stone",
                "ClangCola",
                "CosmicCoffee",
                "Medkit",
                "Powerkit",
                "SpaceCredit",
                "AngleGrinder4Item",
                "HandDrill4Item",
                "Welder4Item",
                "AngleGrinder2Item",
                "HandDrill2Item",
                "Welder2Item",
                "AngleGrinderItem",
                "HandDrillItem",
                "HydrogenBottle",
                "AutomaticRifleItem",
                "UltimateAutomaticRifleItem",
                "RapidFireAutomaticRifleItem",
                "PreciseAutomaticRifleItem",
                "OxygenBottle",
                "AdvancedHandHeldLauncherItem",
                "AngleGrinder3Item",
                "HandDrill3Item",
                "Welder3Item",
                "BasicHandHeldLauncherItem",
                "SemiAutoPistolItem",
                "ElitePistolItem",
                "FullAutoPistolItem",
                "WelderItem",
                "Missile200mm",
                "NATO_25x184mm",
                "NATO_5p56x45mm",
                "AutomaticRifleGun_Mag_20rd",
                "UltimateAutomaticRifleGun_Mag_30rd",
                "RapidFireAutomaticRifleGun_Mag_50rd",
                "PreciseAutomaticRifleGun_Mag_5rd",
                "SemiAutoPistolMagazine",
                "ElitePistolMagazine",
                "FullAutoPistolMagazine",
            };
        }
    }
}
