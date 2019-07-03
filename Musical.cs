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
                PlaySound("Level_Up_Other",1f,1f,1.5f);
				E.RequestInterfaceExit();
			}
			return base.FireEvent(E);
		}


        public void PlaySound(string Name, float Volume, float LowPass, float Pitch)
        {
            IPart.AddPlayerMessage("Playing: "+Name);
            UnityEngine.GameObject gameObject;
            // if (SoundManager.AudioSourcePool.Count > 0)
            // {
            //     gameObject = SoundManager.AudioSourcePool.Dequeue();
            // }
            // else
            // {
                gameObject = new UnityEngine.GameObject();
                gameObject.transform.position = new Vector3(0f, 0f, 1f);
                gameObject.name = "PooledWorldSound";
                gameObject.AddComponent<AudioSource>();
                gameObject.AddComponent<AudioLowPassFilter>();
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            //}
            AudioSource component = gameObject.GetComponent<AudioSource>();
            AudioLowPassFilter component2 = gameObject.GetComponent<AudioLowPassFilter>();
            component.clip = SoundManager.GetClip(Name);
            component.volume = Volume;
            component.pitch = Pitch;
            component2.cutoffFrequency = 22000f * LowPass * Volume;
            //SoundManager.PlayingAudioSources.Enqueue(gameObject);
            component.Play();
        }
	}
}
