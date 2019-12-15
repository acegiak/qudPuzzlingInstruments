using System;
using System.Text;
using XRL.Rules;
using XRL.World.Parts;
using Qud.API;
using XRL.UI;
using XRL.Core;

namespace XRL.World.Parts.Effects
{
	[Serializable]
	public class acegiak_SongSecondaryEffectWhisper : acegiak_SongEffect
	{


		public acegiak_SongSecondaryEffectWhisper():base()
		{

		}
		public acegiak_SongSecondaryEffectWhisper(int d):base()
		{
            base.Duration = d;
		}
		public override void Register(GameObject Object)
		{
			Object.RegisterEffectEvent(this, "AccomplishmentAdded");
			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
			Object.UnregisterEffectEvent(this, "AccomplishmentAdded");
			base.Unregister(Object);
		}

		public override string GetDetails()
		{
			return base.GetDetails()+"\nGrants strange insights into the meanings within words.";
		}

        public override bool FireEvent(Event E){
            if (E.ID == "AccomplishmentAdded")
			{

				string text = E.GetStringParameter("Text");
				if(text.Contains("You read ")){

					if(Object.IsPlayer() && Stat.Roll("1d20") + Object.StatMod("Intelligence")>18){
                    	JournalAPI.RevealRandomSecret();
					}
				}

				return true;
			}

            return base.FireEvent(E);
        }

		



	}
	}