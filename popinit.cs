using XRL.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XRL.Language;
using System;


namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_musicpopinit : IPart
	{
		public string Mods = string.Empty;

		public string Tiers = string.Empty;

		public acegiak_musicpopinit()
		{
			base.Name = "acegiak_musicpopinit";

            // AddToPopTable("RandomLiquid", new PopulationObject { Blueprint = "SomeLiquid" });
            // AddToPopTable("RandomFaction", new PopulationObject { Blueprint = "SomeFaction" });


             PopulationGroup group = new PopulationGroup{ Style="pickeach", Weight=5};
             group.Items.Add(new PopulationObject{ Blueprint="Furniture Instrument" });
             group.Items.Add(new PopulationObject{ Blueprint="Floor Cushion" });

            AddToPopTable("Villages_BuildingContents_Dwelling_House_*Default", new PopulationObject{ Blueprint="Furniture Instrument" , Chance="10"});

            AddToPopTable("SultanDungeons_Furnishings_*Default",group);
            AddToPopTable("CommonOddEncounters",group);
            // PopulationGroup group2 = new PopulationGroup{ Style="pickeach"};
            // group2.Items.Add(new PopulationObject{ Blueprint="OperatingTable" });
            // group2.Items.Add(new PopulationObject{ Blueprint="Bloodsplatter", Number="1-8" });

            // group2.Weight = 5;
            // AddToPopTable("CommonOddEncounters",group2);

		}
    

        public static bool AddToPopTable(string table, params PopulationItem[] items) {
            PopulationInfo info;
            if (!PopulationManager.Populations.TryGetValue(table, out info))
                return false;
                
            // If this is a single group population, add to that group.
            if (info.Items.Count == 1 && info.Items[0] is PopulationGroup) { 
                var group = info.Items[0] as PopulationGroup;
                group.Items.AddRange(items);
                return true;
            }

            info.Items.AddRange(items);
            return true;
        }
    }
}