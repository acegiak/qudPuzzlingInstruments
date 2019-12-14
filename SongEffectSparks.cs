using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.UI;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_SongEffectSparks : acegiak_SongEffect
	{
		public acegiak_SongEffectSparks()
		{
			base.DisplayName = "&Csparksong";
		}

		public acegiak_SongEffectSparks(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "inspiring to the insight";

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

                                item.ApplyEffect(new acegiak_SongEffectSpark(10* Stat.Random(1, 10)));
                                if(item.IsPlayer()){
					                Popup.Show("You gain an air of insight.");
                                }else{
                                    IPart.AddPlayerMessage(item.The+item.DisplayNameOnly+item.Is+" inspired to insight.");
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
