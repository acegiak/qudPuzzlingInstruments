using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.UI;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_SongEffectCamo : acegiak_SongEffect
	{
		public acegiak_SongEffectCamo()
		{
			base.DisplayName = "&Cshadowsong";
		}

		public acegiak_SongEffectCamo(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "inspiring to shadows";

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
							
                                acegiak_SongSecondaryEffectCamoflage e = new acegiak_SongSecondaryEffectCamoflage();
                                e.Duration = 10* Stat.Random(1, 10);
                                item.ApplyEffect(e);
                                if(item.IsPlayer()){
					                Popup.Show("You are inspired to shadows.");
                                }else{
                                    IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.Is+" inspired to shadows.");
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
