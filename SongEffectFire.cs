using System;
using System.Collections.Generic;
using XRL.World;
using XRL.Rules;
using XRL.World.Parts;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_SongEffectFire : Effect
	{
		public acegiak_SongEffectFire()
		{
			base.DisplayName = "&Raflame";
		}

		public acegiak_SongEffectFire(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "setting nearby on fire";

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
                            item.FireEvent(Event.New("TemperatureChange", "Amount", 400, "Owner", Object));

                            // Damage damage = new Damage(Stat.Roll("1d4"));
                            // damage.AddAttribute("Fire");
                            // damage.AddAttribute("Heat");
                            // Event @event = Event.New("TakeDamage");
                            // @event.AddParameter("Damage", damage);
                            // @event.AddParameter("Owner", Object);
                            // @event.AddParameter("Attacker", Object);
                            // @event.AddParameter("Message", "from %o flames!");
                            // item.FireEvent(@event);
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
