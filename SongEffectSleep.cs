using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.UI;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_SongEffectSleep : acegiak_SongEffect
	{
		public acegiak_SongEffectSleep()
		{
			base.DisplayName = "&Clulling";
		}

		public acegiak_SongEffectSleep(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "sending things to sleep";

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
					List<GameObject> list = currentCell.ParentZone.FastSquareSearch(currentCell.X, currentCell.Y, radius, "Combat");
					foreach (GameObject item in list)
					{
						if (Object.DistanceTo(item) <= radius && item.pBrain != null)
						{
							int magnitude = Stat.Roll("1d12") + 2;
                            if(item == Object){
                                magnitude = magnitude/4;
                            }
							int num = Stats.GetCombatMA(item) + item.Stat("Level") + 5;
							magnitude = item.ResistMentalIntrusion("Beguiling", magnitude, Object);
                            //IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.GetVerb("is")+" lulled...");
							if (magnitude < num ){
                                item.ForceApplyEffect(new Asleep(20+ Stat.Random(0, 9),true));
                                if(item.IsPlayer()){
					                Popup.Show("You fall asleep!");
                                }else{
                                    IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.GetVerb("fall")+" asleep!");
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
