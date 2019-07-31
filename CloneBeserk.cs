using System;
using XRL.Core;
using XRL.Messages;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_CloneBeserk : Effect
	{
		public acegiak_CloneBeserk()
		{
			base.DisplayName = "berserk";
		}

		public acegiak_CloneBeserk(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override bool SameAs(Effect e)
		{
			return false;
		}

		public override string GetDetails()
		{
			return "100% chance to dismember with axes.";
		}

		public override bool Apply(GameObject Object)
		{
			return true;
		}

		public override void Remove(GameObject Object)
		{
			base.Remove(Object);
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterEffectEvent(this, "BeginTakeAction");
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			Object.UnregisterEffectEvent(this, "BeginTakeAction");
			base.Unregister(Object);
		}

		public override bool Render(RenderEvent E)
		{
			if (Duration == 0)
			{
				return true;
			}
			int num = XRLCore.CurrentFrame % 60;
			if (num > 45 && num < 55)
			{
				E.Tile = null;
				E.RenderString = "!";
				E.ColorString = "&R";
			}
			return true;
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "BeginTakeAction")
			{
				if (Duration > 0)
				{
					Duration--;
				}
				if (Duration > 0 && Object.IsPlayer())
				{
					//MessageQueue.AddPlayerMessage(Duration + " turns remain until your berserker rage ends.");
				}
				return true;
			}
			return base.FireEvent(E);
		}
	}
}
