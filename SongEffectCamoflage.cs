using System;
using XRL.Core;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_SongSecondaryEffectCamoflage : ICamouflageEffect
	{
		public acegiak_SongSecondaryEffectCamoflage()
		{
			base.DisplayName = "&Kinspired to shadows";
		}

		public override bool Render(RenderEvent E)
		{
			int currentFrameLong = XRLCore.CurrentFrameLong10;
			if (currentFrameLong >= 1000 && currentFrameLong < 3000)
			{
				E.ColorString = "&b";
				E.DetailColor = "K";
			}
			else if (currentFrameLong >= 7000 && currentFrameLong < 9000)
			{
				E.ColorString = "&K";
				E.DetailColor = "b";
			}
			return true;
		}

		public override bool EnablesCamouflage(GameObject GO)
		{
			return true;
		}
	}
}
