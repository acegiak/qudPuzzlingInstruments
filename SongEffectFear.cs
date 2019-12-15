using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_SongEffectFear : acegiak_SongEffect
	{
		public acegiak_SongEffectFear()
		{
			base.DisplayName = "&Rterrible";
		}

		public acegiak_SongEffectFear(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "terrifying those nearby";

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
							int mod = 1;
							if(Object.Statistics["Ego"].Modifier > 0){
								mod += Object.Statistics["Ego"].Modifier;
							}
                            Fear.ApplyFearToObject(mod.ToString()+"d8", Stat.Roll("1d20")+mod, item, Object);

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
