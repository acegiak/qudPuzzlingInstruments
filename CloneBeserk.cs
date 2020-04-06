using System;
using XRL.Core;
using XRL.Messages;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_CloneBeserk : Effect
	{
		public acegiak_CloneBeserk()
		{
			base.DisplayName = "inspired to battle";
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
			return "&W+1 to hit&y";
		}

		// public override bool Apply(GameObject Object)
		// {
		// 	return true;
		// }

		// public override void Remove(GameObject Object)
		// {
		// 	base.Remove(Object);
		// }

		public override void Register(GameObject Object)
		{
			Object.RegisterEffectEvent(this, "AttackerRollMeleeToHit");
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			Object.UnregisterEffectEvent(this, "AttackerRollMeleeToHit");
			base.Unregister(Object);
		}

		// public override bool Render(RenderEvent E)
		// {
		// 	if (Duration == 0)
		// 	{
		// 		return true;
		// 	}
		// 	int num = XRLCore.CurrentFrame % 60;
		// 	if (num > 45 && num < 55)
		// 	{
		// 		E.Tile = null;
		// 		E.RenderString = "!";
		// 		E.ColorString = "&R";
		// 	}
		// 	return true;
		// }




		public override bool Apply(GameObject Object)
		{
			// Object.RegisterEffectEvent(this, "AttackerRollMeleeToHit");
			if (Object.IsPlayer())
			{
				//Popup.Show(wellFedMessage + "\n\n&W+1 to hit for the rest of the day&y");
			}
			return true;
		}

		public override void Remove(GameObject Object)
		{
			// Object.UnregisterEffectEvent(this, "AttackerRollMeleeToHit");
			base.Remove(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "AttackerRollMeleeToHit")
			{
				E.AddParameter("Result", (int)E.GetParameter("Result") + 1);
				return true;
			}
			return base.FireEvent(E);
		}


	}
}
