using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Rules;
using XRL.UI;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_HunterSong : acegiak_SongEffect
	{
		public bool DisableUnlost;

		public long LostOn;

		public List<string> Visited = new List<string>();

		public acegiak_HunterSong()
		{
			base.DisplayName = "Hunter's Song";
		}

		public acegiak_HunterSong(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "You detect the presence of nearby creatures.";
		}

		public override bool SameAs(Effect e)
		{
			return false;
		}


		public override void Register(GameObject Object)
		{
			Object.RegisterEffectEvent(this, "EndTurn");
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			Object.UnregisterEffectEvent(this, "EndTurn");
			base.Unregister(Object);
		}


		public override bool FireEvent(Event E)
		{
			if (E.ID == "EndTurn")
			{
				Cell currentCell = Object.CurrentCell;
				if (currentCell != null)
				{
					int radius = 40;
					List<GameObject> list = currentCell.ParentZone.FastSquareSearch(currentCell.X, currentCell.Y, radius, "Combat");
					foreach (GameObject item in list)
					{
						if (Object.DistanceTo(item) <= radius && !item.HasEffect("HeightenedHearingEffect"))
						{
							item.ApplyEffect(new HeightenedHearingEffect(1, Object));
						}
					}
				}
			}
			return base.FireEvent(E);
		}
	}
}
