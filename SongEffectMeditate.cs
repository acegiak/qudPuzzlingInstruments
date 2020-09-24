using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.UI;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_SongEffectMeditate : acegiak_SongEffect
	{
		public acegiak_SongEffectMeditate()
		{
			base.DisplayName = "&Cfocussong";
		}

		public acegiak_SongEffectMeditate(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "inspiring to meditation";
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
                            if(item.pBrain.GetOpinion(Object) == Brain.CreatureOpinion.allied){

								List<Effect> remove = new List<Effect>();
								foreach (Effect effect in item.Effects)
								{
									if(effect is acegiak_SongEffect){
										remove.Add(effect);
									}
								}
								foreach(Effect effect in remove){
									item.RemoveEffect(effect);
								}

                                item.ApplyEffect(new Meditating(20* Stat.Random(1, 10)));
                                if(item.IsPlayer()){
					                Popup.Show("You enter a meditative state.");
                                }else{
                                    IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.GetVerb("enter")+" a meditative state.");
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
