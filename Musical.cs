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
using ConsoleLib.Console;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Musical : IPart
	{
        public string SoundName;
        public string NoteSequence;

		public bool Generate = false;


		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			if(Generate){
				BuildRandom();
			}
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandPlayTune");
			Object.RegisterPartEvent(this, "InvCommandComposeTune");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "GetInventoryActions")
			{
				E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("PlayTune", 'p', false, "&Wp&ylay a tune", "InvCommandPlayTune", 10);
				E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("ComposeTune", 'C', false, "&WC&yompose a tune", "InvCommandComposeTune", 10);
			}
			if (E.ID == "InvCommandPlayTune")
			{
                PlaySong(E.GetGameObjectParameter("Owner"));
				musicalParticles();
				E.RequestInterfaceExit();
			}
			if (E.ID == "InvCommandComposeTune")
			{
                // PlaySong(E.GetGameObjectParameter("Owner"));
				// musicalParticles();
				Compose(E.GetGameObjectParameter("Owner"));
				E.RequestInterfaceExit();
			}
			return base.FireEvent(E);
		}


        public void PlaySong(GameObject player)
        {
			GameObject song = ChooseSong(player);

			List<UnityEngine.GameObject> GOs = new List<UnityEngine.GameObject>();
			foreach(string voice in SoundName.Split(';')){
				string[] bits = voice.Split(':');

				UnityEngine.GameObject gameObject;
				gameObject = new UnityEngine.GameObject();
				gameObject.transform.position = new Vector3(0f, 0f, 1f);
				gameObject.name = "MusicPlayer";
				gameObject.AddComponent<acegiak_AudioSequencer>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);

				acegiak_AudioSequencer component = gameObject.GetComponent<acegiak_AudioSequencer>();
				
				if(bits.Length >1){
					component.Read(bits[0],song.GetPart<acegiak_Song>().Notes, bits[1]);
				}else{
					component.Read(bits[0],song.GetPart<acegiak_Song>().Notes, String.Empty);
				}
				GOs.Add(gameObject);
			}
			foreach(UnityEngine.GameObject GO in GOs){
				GO.GetComponent<acegiak_AudioSequencer>().Play();
			}
			IPart.AddPlayerMessage((player.IsPlayer()?"You play":player.The+player.DisplayNameOnly+player.GetVerb("play"))+" "+song.GetPart<acegiak_Song>().Name+" on "+ParentObject.the+ParentObject.DisplayNameOnly);
        }


        public void Compose(GameObject who)
        {

			List<UnityEngine.GameObject> GOs = new List<UnityEngine.GameObject>();
			foreach(string voice in SoundName.Split(';')){
				string[] bits = voice.Split(':');

				UnityEngine.GameObject gameObject;
				gameObject = new UnityEngine.GameObject();
				gameObject.transform.position = new Vector3(0f, 0f, 1f);
				gameObject.name = "MusicPlayer";
				gameObject.AddComponent<acegiak_AudioSequencer>();
				gameObject.GetComponent<acegiak_AudioSequencer>().recordVoice = bits[0];
				if(bits.Length>1){
					gameObject.GetComponent<acegiak_AudioSequencer>().recordVolume = Int32.Parse(bits[1])/100f;
				}
				UnityEngine.Object.DontDestroyOnLoad(gameObject);

				GOs.Add(gameObject);
			}

			foreach(UnityEngine.GameObject GO in GOs){
				acegiak_AudioSequencer component = GO.GetComponent<acegiak_AudioSequencer>();
				component.Record();
			}

			acegiak_ScreenBufferMaker p = delegate(ScreenBuffer sb, int charcode)
				{
					ConsoleChar c = new ConsoleChar();
					c.Tile = "Tiles/sw_box.bmp";
					sb[1,1] = c;
					ConsoleChar f = new ConsoleChar();
					f.Tile = "Tiles/sw_box.bmp";
					sb[3,3] = f;
					//IPart.AddPlayerMessage("Boxy?");
					return sb;
				};
			acegiak_CustomPopup.CustomRender(p,20,10);
			string songname = Popup.AskString("Name this song. (leave blank to forget)","",140);
			if(songname != null && songname.Length > 0){
				acegiak_SongBook book = who.GetPart<acegiak_SongBook>();
				if(book == null){
					Popup.Show(who.The+who.DisplayNameOnly+" can't remember songs.");
				}else{
					acegiak_Song song = new acegiak_Song();
					song.Name = songname;
					song.Notes = GOs[0].GetComponent<acegiak_AudioSequencer>().Print();


					GameObject GO = GameObject.create("Song");
					GO.AddPart(song);

					book.Songs.Add(GO);
				}
				
			}
            
        }

		public void musicalParticles(){
			Cell currentCell = ParentObject.GetCurrentCell();
			if (currentCell != null && currentCell.IsVisible())
			{
				int x2 = currentCell.X;
				int y = currentCell.Y;
				ParticleManager particleManager = XRLCore.ParticleManager;
				particleManager.AddSinusoidal("&W"+ '\u000e', x2, y, 1.5f * (float)Stat.Random(1, 6), 0.1f * (float)Stat.Random(1, 60), 0.1f + 0.025f * (float)Stat.Random(0, 4), 1f, 0f, 0f, -0.1f - 0.05f * (float)Stat.Random(1, 6), 999);
				particleManager.AddSinusoidal("&Y"+ '\u000d', x2, y, 1.5f * (float)Stat.Random(1, 6), 0.1f * (float)Stat.Random(1, 60), 0.1f + 0.025f * (float)Stat.Random(0, 4), 1f, 0f, 0f, -0.1f - 0.05f * (float)Stat.Random(1, 6), 999);
				particleManager.AddSinusoidal("&W" + '\u000d', x2, y, 1.5f * (float)Stat.Random(1, 6), 0.1f * (float)Stat.Random(1, 60), 0.1f + 0.025f * (float)Stat.Random(0, 4), 1f, 0f, 0f, -0.1f - 0.05f * (float)Stat.Random(1, 6), 999);
				particleManager.AddSinusoidal("&Y" + '\u000e', x2, y, 1.5f * (float)Stat.Random(1, 6), 0.1f * (float)Stat.Random(1, 60), 0.1f + 0.025f * (float)Stat.Random(0, 4), 1f, 0f, 0f, -0.1f - 0.05f * (float)Stat.Random(1, 6), 999);
				
			}
		}

		public GameObject ChooseSong(GameObject who){
			acegiak_SongBook part2 = who.GetPart<acegiak_SongBook>();
			if(!who.IsPlayer()){
				return part2.Songs.GetRandomElement();
			}

			//IPart.AddPlayerMessage(part2.ToString());
            List<GameObject> ObjectChoices = new List<GameObject>();
            List<string> ChoiceList = new List<string>();
            List<char> HotkeyList = new List<char>();
            char ch = 'a';
			if(part2 == null || part2.Songs.Count <= 0){
				Popup.Show(who.The+who.DisplayNameOnly+who.GetVerb("don't")+" know any songs");
				return null;
			}
			foreach(GameObject song in part2.Songs){
                    ObjectChoices.Add(song);
                    HotkeyList.Add(ch);
                    ChoiceList.Add(song.GetPart<acegiak_Song>().Name);
                    ch = (char)(ch + 1);
            }
            if (ObjectChoices.Count == 0)
            {
                Popup.Show(who.The+who.DisplayNameOnly+who.GetVerb("don't")+" know any songs");
                return null;
            }
            int num12 = Popup.ShowOptionList(string.Empty, ChoiceList.ToArray(), HotkeyList.ToArray(), 0, "Select a song to play.", 60, bRespectOptionNewlines: false, bAllowEscape: true);
            if (num12 < 0)
            {
                return null;
            }
			return ObjectChoices[num12];
		}

		public void BuildRandom(string faction = null){

			List<string> tiles = new List<string>{"items/horn_01.png","items/percussion_01.png","items/rattle_01.png","items/stringed_01.png","items/wind_01.png"};
			List<string> voices = new List<string>{"oboe","inst1_breath","inst2_high"};

			ParentObject.pRender.Tile = tiles.GetRandomElement();
			Double d = Stat.Rnd2.NextDouble()*3;
			this.SoundName = "";
			for(int i = 0; i<d;i++){
				if(i >0){
					this.SoundName += ";";
				}
				this.SoundName += voices.GetRandomElement();
				int tvol = Stat.Rnd2.Next(50)+25;
				this.SoundName = this.SoundName+":"+tvol.ToString();
				Debug.Log(this.SoundName);
			}
		}
	}
}
