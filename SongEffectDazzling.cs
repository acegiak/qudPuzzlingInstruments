using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.UI;
using XRL.Language;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_SongEffectDazzling : acegiak_SongEffect
	{
		public acegiak_SongEffectDazzling()
		{
			base.DisplayName = "&Cdazzlingsong";
		}

		public acegiak_SongEffectDazzling(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "dazzling";

		}

		public override bool CanApplyToStack()
		{
			return true;
		}

		public override bool Apply(GameObject Object)
		{
            //Object.ApplyEffect(new Emboldened(5*Stat.Roll("1d10"),"Ego",2));
            int radius = 20;
            Cell currentCell = Object.CurrentCell;
				if (currentCell != null)
				{
					List<GameObject> list = currentCell.ParentZone.FastSquareSearch(currentCell.X, currentCell.Y, radius, "Combat", null);
					foreach (GameObject item in list)
					{
						if (Object.DistanceTo(item) <= radius && item.pBrain != null)
						{
                            if(item.pBrain.GetOpinion(Object) != Brain.CreatureOpinion.hostile){

                                int magnitude = Stat.Roll("1d12") + 2;
                                if(item == Object){
                                    magnitude = magnitude/4;
                                }
                                int num = Stats.GetCombatMA(item) + item.Stat("Level") + 5;
                                magnitude = item.ResistMentalIntrusion("Beguiling", magnitude, Object);
                                //IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.GetVerb("is")+" lulled...");
                                if (magnitude < num ){
                                    item.pBrain.AdjustFeeling(Object,5);
                                        IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.Is+" thrilled by "+(Object.IsPlayer()?"your":Object.the+Grammar.MakePossessive(Object.DisplayNameOnly))+" song.");
                                }else{
                                    item.pBrain.AdjustFeeling(Object,-10);
                                        IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.Is+" annoyed by "+(Object.IsPlayer()?"your":Object.the+Grammar.MakePossessive(Object.DisplayNameOnly))+" song.");
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
