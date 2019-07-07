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

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
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
            UnityEngine.GameObject gameObject;
            gameObject = new UnityEngine.GameObject();
            gameObject.transform.position = new Vector3(0f, 0f, 1f);
            gameObject.name = "MusicPlayer";
            gameObject.AddComponent<acegiak_AudioSequencer>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            acegiak_AudioSequencer component = gameObject.GetComponent<acegiak_AudioSequencer>();
			acegiak_Song song = ChooseSong(player);
            component.Read(SoundName,song.Notes);
            component.Play();
            
        }


        public void Compose(GameObject who)
        {
            UnityEngine.GameObject gameObject;
            gameObject = new UnityEngine.GameObject();
            gameObject.transform.position = new Vector3(0f, 0f, 1f);
            gameObject.name = "MusicPlayer";
            gameObject.AddComponent<acegiak_AudioSequencer>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);

            acegiak_AudioSequencer component = gameObject.GetComponent<acegiak_AudioSequencer>();
			component.Record();

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
					song.Notes = component.Print();
					book.Songs.Add(song);
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

		public acegiak_Song ChooseSong(GameObject who){
			acegiak_SongBook part2 = who.GetPart<acegiak_SongBook>();
			IPart.AddPlayerMessage(part2.ToString());
            List<acegiak_Song> ObjectChoices = new List<acegiak_Song>();
            List<string> ChoiceList = new List<string>();
            List<char> HotkeyList = new List<char>();
            char ch = 'a';
			if(part2 == null || part2.Songs.Count <= 0){
				Popup.Show(who.The+who.DisplayNameOnly+who.GetVerb("don't")+" know any songs");
				return null;
			}
			foreach(acegiak_Song song in part2.Songs){
                    ObjectChoices.Add(song);
                    HotkeyList.Add(ch);
                    ChoiceList.Add(song.Name);
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
	}
}
