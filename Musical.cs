using System;
using System.Collections.Generic;
using XRL.UI;
using XRL.World.Parts.Effects;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XRL;
using XRL.Core;
using XRL.Rules;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Musical : IPart
	{
        public string SoundName;
        public string NoteSequence;

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandPlayTune");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "GetInventoryActions")
			{
				E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("PlayTune", 'p', false, "&Wp&ylay a tune", "InvCommandPlayTune", 10);
			}
			if (E.ID == "InvCommandPlayTune")
			{
                PlaySound();
				E.RequestInterfaceExit();
			}
			return base.FireEvent(E);
		}


        public void PlaySound()
        {
            UnityEngine.GameObject gameObject;
            gameObject = new UnityEngine.GameObject();
            gameObject.transform.position = new Vector3(0f, 0f, 1f);
            gameObject.name = "MusicPlayer";
            gameObject.AddComponent<acegiak_AudioSequencer>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            acegiak_AudioSequencer component = gameObject.GetComponent<acegiak_AudioSequencer>();
            component.Read(SoundName,NoteSequence);
            component.Play();
            //}
            
        }
	}
}
