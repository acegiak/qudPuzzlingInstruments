using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.UI;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_SongEffectAdventure : Effect
	{
		public acegiak_SongEffectAdventure()
		{
			base.DisplayName = "&Cinspiring to horizons";
		}

		public acegiak_SongEffectAdventure(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "inspiring to horizons";

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
                            if(item.pBrain.GetOpinion(Object) == Brain.CreatureOpinion.allied){
							
                                item.ApplyEffect(new acegiak_SongSecondaryEffectAdventure(600* Stat.Random(1, 6)));
                                if(item.IsPlayer()){
					                Popup.Show("You are lured to the horizon!");
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
