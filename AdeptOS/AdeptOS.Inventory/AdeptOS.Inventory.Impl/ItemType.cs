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
            public string Type { get; private set; }
            public string Subtype { get; private set; }

            public ItemType(string qualifier)
            {
                var terms = qualifier.Split('.');
                if (terms.Length < 1)
                    throw new Exception("Invalid item type qualifier " + qualifier);

                if (terms.Length >= 2)
                {
                    SetTypeSubtype(terms[1], terms[0]);
                    return;
                }

                var term = terms[0];

                var typeMatch = Types.FirstOrDefault(x => x.ToUpper().Contains(term.ToUpper()));
                var subtypeMatch = Subtypes.FirstOrDefault(x => x.ToUpper().Contains(term.ToUpper()));

                if (subtypeMatch != null)
                    Subtype = subtypeMatch;
                else
                    Type = typeMatch;

                if (Type == null && AmbiguosTypes.Contains(Subtype))
                    Type = "Ingot";

                if (Type == null && Subtype == null)
                    throw new Exception("Invalid item type qualifier " + qualifier);
            }

            public ItemType(string subtype, string type)
            {
                SetTypeSubtype(subtype, type);
            }

            void SetTypeSubtype(string subtype, string type)
            {
                Type = type == null ? null : Types.FirstOrDefault(t => t.ToUpper().Contains(type.ToUpper()));
                Subtype = Subtypes.FirstOrDefault(t => t.ToUpper().Contains(subtype.ToUpper()));

                if (Type == null && AmbiguosTypes.Contains(Subtype))
                {
                    Type = "Ingot";
                }
            }

            public ItemType(MyInventoryItem item)
            {
                Type = Types.FirstOrDefault(t => item.Type.ToString().ToUpper().Contains(t.ToUpper()));
                Subtype = Subtypes.FirstOrDefault(t => item.Type.ToString().ToUpper().Contains(t.ToUpper()));

                if (Type == null && AmbiguosTypes.Contains(Subtype))
                {
                    Type = "Ingot";
                }

                if (!AmbiguosTypes.Contains(Subtype) && Subtype != null)
                {
                    Type = null;
                }
            }

            public ItemType(MyItemType item)
            {
                Type = Types.FirstOrDefault(t => item.TypeId.ToUpper().Contains(t.ToUpper()));
                Subtype = Subtypes.FirstOrDefault(t => item.SubtypeId.ToUpper().Contains(t.ToUpper()));

                if (Type == null && AmbiguosTypes.Contains(Subtype))
                {
                    Type = "Ingot";
                }

                if (!AmbiguosTypes.Contains(Subtype) && Subtype != null)
                {
                    Type = null;
                }
            }

            public override string ToString()
            {
                if (Type == null && AmbiguosTypes.Contains(Subtype))
                {
                    return $"Ingot.{Subtype}";
                }

                if (!AmbiguosTypes.Contains(Subtype) && Subtype != null)
                {
                    return $".{Subtype}";
                }

                return $"{Type}.{Subtype}";
            }

            public override bool Equals(object obj) => ToString().Equals(obj.ToString());

            public string ToDisplayString()
            {
                if (AmbiguosTypes.Contains(Subtype))
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

            // TODO: Add all
            public static string[] AllowedTypes = new[]
            {
                "Component",
                "Ingot",
                "Ore",
            };

            public static string[] Components = new[]
            {
                "BulletproofGlass", "Canvas", "Computer", "Construction", "Detector", "Display", "Explosives",
                "Girder", "GravityGenerator", "InteriorPlate", "LargeTube", "Medical", "MetalGrid", "Motor",
                "PowerCell", "RadioCommunication", "Reactor", "SmallTube", "SolarCell", "SteelPlate",
                "Superconductor", "Thrust", "ZoneChip",
            };

            public static string[] Ingots = new[]
            {
                "Cobalt", "Gold", "Iron", "Magnesium", "Nickel", "Platinum", "Silicon", "Silver", "Uranium", "Stone"
            };

            public static string[] Ores = new[]
            {
                "Cobalt", "Gold", "Stone", "Iron", "Magnesium", "Nickel", "Scrap", "Platinum", "Silicon", "Silver",
                "Uranium", "Ice", "Organic", "Stone",
            };

            public static string[] AmbiguosTypes = new[]
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
                "Stone"
            };

            public static string[] Subtypes = new[]
            {
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
            }.Concat(Ingots).Concat(Components).Concat(Ores).Distinct().ToArray();

            public static string[] GetCategorySource(string type)
            {
                string[] categorySource = { };
                switch (type)
                {
                    case "Ore":
                        categorySource = Ores;
                        break;
                    case "Ingot":
                        categorySource = Ingots;
                        break;
                    case "Component":
                        categorySource = Components;
                        break;
                }

                return categorySource;
            }

            public static ItemType[] AllTypes = Subtypes
                .Select(x => new ItemType(x))
                .Concat(AmbiguosTypes.Select(x => new ItemType(x, "Ore")))
                .ToArray();
        }
    }
}
