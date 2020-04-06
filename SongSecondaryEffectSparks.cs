using System;
using System.Text;
using XRL.Rules;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_SongEffectSpark : acegiak_SongEffect
	{


		public acegiak_SongEffectSpark():base()
		{
			base.DisplayName = "&Csparksong";
		}

		public acegiak_SongEffectSpark(int d):base()
		{
            base.Duration = d;
			base.DisplayName = "&Csparksong";
		}

		

		public override string GetDetails()
		{
			return base.GetDetails()+"\nAids in examining and disassembling artifacts.";
		}


		public override bool Apply(GameObject Object)
		{
			if (Object != null)
			{
				Object.ModIntProperty("InspectorEquipped", 3,  true);
				Object.ModIntProperty("DisassembleBonus", 3,  true);
			}
			return base.Apply(Object);
		}

		public override void Remove(GameObject Object)
		{
			if (Object != null)
			{
				Object.ModIntProperty("InspectorEquipped", -3,  true);
				Object.ModIntProperty("DisassembleBonus", -3,  true);
			}
			base.Remove(Object);
		}



	}
	}