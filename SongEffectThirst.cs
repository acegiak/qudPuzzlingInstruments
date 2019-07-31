using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.UI;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_SongEffectThirstLess : Effect
	{
		public acegiak_SongEffectThirstLess()
		{
			base.DisplayName = "&Cquenched";
		}

		public acegiak_SongEffectThirstLess(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "reduces thirst";

		}

		public override bool CanApplyToStack()
		{
			return true;
		}

		public override bool Apply(GameObject Object)
		{
            int radius = 20;
            Cell currentCell = Object.CurrentCell;
				if (currentCell != null)
				{
					List<GameObject> list = currentCell.ParentZone.FastSquareSearch(currentCell.X, currentCell.Y, radius, "Combat", null);
					foreach (GameObject item in list)
					{
						if (Object.DistanceTo(item) <= radius && item.pBrain != null)
						{
                            if(item.GetPart<Stomach>() != null){
                                if(item.GetPart<Stomach>().Water < 30000
                                && item.GetPart<Stomach>().HungerLevel < 2){
                                    item.GetPart<Stomach>().Water += 5000;
                                    item.GetPart<Stomach>().HungerLevel++;
                                    if(item.IsPlayer()){
                                        IPart.AddPlayerMessage("You feel refreshed.");
                                    }else{
                                        IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.GetVerb("feel")+" refreshed.");
                                    }
                                }
                            }
                        }
                    }
                }
			return true;
		}

		public override void Register(GameObject Object)
		{
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			base.Unregister(Object);
		}

		public override bool FireEvent(Event E)
		{
			return true;
		}
	}
}
