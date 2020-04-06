using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.Rules;
using XRL.UI;
using Qud.API;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_SongSecondaryEffectAdventure : acegiak_SongEffect
	{
		public bool DisableUnlost;

		public long LostOn;

		public List<string> Visited = new List<string>();

		public acegiak_SongSecondaryEffectAdventure()
		{
			base.DisplayName = "&Binspired to the horizon";
		}

		public acegiak_SongSecondaryEffectAdventure(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "You glean secrets from the land as you travel.";
		}

		public override bool SameAs(Effect e)
		{
			return false;
		}


		public override void Register(GameObject Object)
		{
			Object.RegisterEffectEvent(this, "CheckLostChance");
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			Object.UnregisterEffectEvent(this, "CheckLostChance");
			base.Unregister(Object);
		}


		public override bool FireEvent(Event E)
		{
			if (E.ID == "CheckLostChance")
			{
				if(Stat.Roll("1d20")+(Object.StatMod("Intelligence")/2)>19){
                    Popup.Show("As you travel you glean secrets from the land.");
                    JournalAPI.RevealRandomSecret();
                }
			}
			return base.FireEvent(E);
		}
	}
}
