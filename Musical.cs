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
using System.Linq;
using HistoryKit;
using XRL.Language;
using System.Text;


namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_Musical : IPart
	{
        public string SoundName;
        public string NoteSequence;

		public string instrumentName;

		public string backupDescription;

		public string Faction = null;

		public bool Generate = false;



		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandPlayTune");
			Object.RegisterPartEvent(this, "InvCommandComposeTune");
			Object.RegisterPartEvent(this,"VisibleStatusColor");
			Object.RegisterPartEvent(this,"GetDisplayName");
			Object.RegisterPartEvent(this,"GetShortDisplayName");
			Object.RegisterPartEvent(this,"IdleQuery");
			Object.RegisterPartEvent(this,"CanSmartUse");
			Object.RegisterPartEvent(this,"CommandSmartUse");
			Object.RegisterPartEvent(this,"ExamineSuccess");
			
			
			base.Register(Object);
		}

		public void Make(GameObject GO = null){
			if(Faction == null && Generate){
				Generate = false;
				//IPart.AddPlayerMessage("MAKE INSTRUMENT");
				this.Faction = Factions.GetRandomFactionWithAtLeastOneMember().Name;

				if(GO == null){

					if(ParentObject.Equipped != null){
						GO = ParentObject.Equipped;
					}
					if(ParentObject.InInventory != null){
						GO = ParentObject.InInventory;
					}

				}

				if(GO != null && GO.GetPart<Brain>() != null && GO.GetPart<Brain>().GetPrimaryFaction() != null){
					this.Faction = GO.GetPart<Brain>().GetPrimaryFaction();
				}

				BuildRandom(acegiak_SongBook.FactionTags(this.Faction));
			}
		}

		public override bool BeforeRender(Event E){
			Make();
			return base.BeforeRender(E);
		}

		public override bool FireEvent(Event E)
		{
			Make();
			if (E.ID == "GetInventoryActions" && XRLCore.Core.Game.Player.Body.HasSkill("acegiak_Customs_Music"))
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
			// if(E.ID=="BeginBeingTaken"){
			// 	Make(E.GetGameObjectParameter("TakingObject"));
			// }

			if (E.ID == "IdleQuery")
			{
				GameObject gameObjectParameter = E.GetGameObjectParameter("Object");
				if (gameObjectParameter.HasPart("Brain") && !gameObjectParameter.HasPart("Robot") && gameObjectParameter.DistanceTo(ParentObject) <= 1 && Stat.Random(1, 10) == 1)
				{
					PlaySong(gameObjectParameter);
				}
			}
			if (E.ID == "CanSmartUse")
			{
				return false;
			}
			if (E.ID == "CommandSmartUse")
			{
				//if(E.GetGameObjectParameter("User").GetPart<acegiak_SongBook>() != null){
					if(E.GetGameObjectParameter("User").GetPart<acegiak_SongBook>().Songs.Count() >0){
						PlaySong(E.GetGameObjectParameter("User"));
					}else if(E.GetGameObjectParameter("User").IsPlayer()){
						Compose(E.GetGameObjectParameter("User"));
					}
				//}
			}
			if(E.ID == "ExamineSuccess"){
				// ParentObject.GetPart<Description>().Short = this.backupDescription;
				// ParentObject.MakeUnderstood();
				ParentObject.GetPart<Examiner>().AlternateDescription = this.backupDescription;
			}
			

			
			 
			return base.FireEvent(E);
		}


        public void PlaySong(GameObject player)
        {
			acegiak_Song song = ChooseSong(player);

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
					component.Read(bits[0],song.Notes, bits[1]);
				}else{
					component.Read(bits[0],song.Notes, String.Empty);
				}
				GOs.Add(gameObject);
			}
			foreach(UnityEngine.GameObject GO in GOs){
				GO.GetComponent<acegiak_AudioSequencer>().Play();
			}
			IPart.AddPlayerMessage((player.IsPlayer()?"You play":player.The+player.DisplayNameOnly+player.GetVerb("play"))+" "+song.Name+" on "+ParentObject.the+ParentObject.DisplayNameOnly);
			if(song.Effect != null){
                    //IPart.AddPlayerMessage("Effect:"+song.Effect);
                    Effect effect = Activator.CreateInstance(Type.GetType(song.Effect)) as Effect;
					//effect.Duration = Stat.Rnd2.Next(100);
                    //IPart.AddPlayerMessage("Effect:"+effect.DisplayName);
					player.ApplyEffect(effect);
			}
			player.FireEvent(Event.New("PlayedSong", "Object", ParentObject));

		
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
			Popup.AskString("Play with 0-9","",300);

			// acegiak_ScreenBufferMaker p = delegate(ScreenBuffer sb, int charcode)
			// 	{
			// 		ConsoleChar c = new ConsoleChar();
			// 		c.Tile = "Tiles/sw_box.bmp";
			// 		sb[1,1] = c;
			// 		ConsoleChar f = new ConsoleChar();
			// 		f.Tile = "Tiles/sw_box.bmp";
			// 		sb[3,3] = f;
			// 		//IPart.AddPlayerMessage("Boxy?");
			// 		return sb;
			// 	};
			// acegiak_CustomPopup.CustomRender(p,20,10);


			string songname = Popup.AskString("Name this song. (leave blank to forget)","",140);
			if(songname != null && songname.Length > 0){
				acegiak_SongBook book = who.GetPart<acegiak_SongBook>();
				if(book == null){
					Popup.Show(who.The+who.DisplayNameOnly+" can't remember songs.");
				}else{
					acegiak_Song song = new acegiak_Song();
					song.Name = songname;
					song.Notes = GOs[0].GetComponent<acegiak_AudioSequencer>().Print();



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
			if(!who.IsPlayer()){
				return part2.Songs.GetRandomElement();
			}

			//IPart.AddPlayerMessage(part2.ToString());
            List<acegiak_Song> ObjectChoices = new List<acegiak_Song>();
            List<string> ChoiceList = new List<string>();
            List<char> HotkeyList = new List<char>();
            char ch = 'a';
			if(part2 == null || part2.Songs.Count <= 0){
				Popup.Show(who.The+who.DisplayNameOnly+who.GetVerb("doesn't")+" know any songs");
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
                Popup.Show(who.The+who.DisplayNameOnly+who.GetVerb("doesn't")+" know any songs");
                return null;
            }
            int num12 = Popup.ShowOptionList(string.Empty, ChoiceList.ToArray(), HotkeyList.ToArray(), 0, "Select a song to play.", 60, bRespectOptionNewlines: false, bAllowEscape: true);
            if (num12 < 0)
            {
                return null;
            }
			return ObjectChoices[num12];
		}

		public void BuildRandom(List<string> fromtags){
			//IPart.AddPlayerMessage(String.Join(", ",fromtags.ToArray()));

			List<string> tiles = new List<string>();
			List<string> voices = new List<string>();
			List<string> prefixes = new List<string>{"mus","song","ton"};
			List<string> infixes = new List<string>{"a","i","o"};
			List<string> postfixes = new List<string>{"phone","tone"};
			List<string> verbs = new List<string>{"believe in"};
			List<string> parts = new List<string>{"body"};
			List<string> descriptors = new List<string>{"musical"};

			List<string> colors = fromtags.Where(b=>b.Length == 1).ToList();

 			if(GameObjectFactory.Factory == null || GameObjectFactory.Factory.BlueprintList == null){
                return;
            }
			foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.BlueprintList)
			{
				if (!blueprint.IsBaseBlueprint() && blueprint.DescendsFrom("InstrumentBit"))
				{
					//IPart.AddPlayerMessage(blueprint.Name);
					GameObject sample = GameObjectFactory.Factory.CreateSampleObject(blueprint.Name);
                    if(sample.HasTag("musictags")){
						List<string> tags = sample.GetTag("musictags").Split(',').ToList();
						//IPart.AddPlayerMessage(sample.GetTag("musictags"));

						if(fromtags.Where(b=>tags.Contains(b)).Any()){

							//IPart.AddPlayerMessage("do");
							if(sample.HasTag("musictiles")){
								tiles = sample.GetTag("musictiles").Split(',').ToList().Union(tiles).ToList();
							}
							if(sample.HasTag("musicsounds")){
								voices = sample.GetTag("musicsounds").Split(',').ToList().Union(voices).ToList();
							}
							if(sample.HasTag("prefixes")){
								prefixes = sample.GetTag("prefixes").Split(',').ToList().Union(prefixes).ToList();
							}
							if(sample.HasTag("infixes")){
								infixes = sample.GetTag("infixes").Split(',').ToList().Union(infixes).ToList();
							}
							if(sample.HasTag("postfixes")){
								postfixes = sample.GetTag("postfixes").Split(',').ToList().Union(postfixes).ToList();
							}
							if(sample.HasTag("parts")){
								parts = sample.GetTag("parts").Split(',').ToList().Union(parts).ToList();
							}
							if(sample.HasTag("verbs")){
								verbs = sample.GetTag("verbs").Split(',').ToList().Union(verbs).ToList();
							}
							if(sample.HasTag("descriptors")){
								descriptors = sample.GetTag("descriptors").Split(',').ToList().Union(descriptors).ToList();
							}
						}

					}
				}
			}


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


			if(colors.Count > 0){
				ParentObject.pRender.TileColor = "&"+colors.GetRandomElement();
			}
			if(colors.Count > 1){
				ParentObject.pRender.DetailColor = colors.Where(b=>b!= ParentObject.pRender.TileColor.Replace("&","")).GetRandomElement();
			}

			string newname = prefixes.GetRandomElement();
			newname += infixes.GetRandomElement();
			newname += postfixes.GetRandomElement();
			while(Stat.Rnd2.NextDouble()<0.2f){
				newname = prefixes.GetRandomElement()+infixes.GetRandomElement()+newname;
			}


			ParentObject.GetPart<Description>().Short = "A "+descriptors.GetRandomElement()+" instrument favoured by "+FactionInfo.getFormattedName(Faction)+". To play it, one "+
			verbForm(verbs.GetRandomElement())+((Stat.Rnd2.NextDouble()<0.5f)?(" and "+verbForm(verbs.GetRandomElement())):"")+
			" the "+parts.GetRandomElement()+((Stat.Rnd2.NextDouble()<0.5f)?(" and the "+parts.GetRandomElement()):"");
			if(Stat.Rnd2.NextDouble() < 0.5f){
				ParentObject.GetPart<Description>().Short += " and "+verbForm(verbs.GetRandomElement())+((Stat.Rnd2.NextDouble()<0.5f)?(" and "+verbForm(verbs.GetRandomElement())):"")+
			" the "+parts.GetRandomElement()+((Stat.Rnd2.NextDouble()<0.5f)?(" and the "+parts.GetRandomElement()):"");
			}
			ParentObject.GetPart<Description>().Short += ".";

			ParentObject.GetPart<Description>().Short = Grammar.ConvertAtoAn(ParentObject.GetPart<Description>().Short);


			ParentObject.pRender.DisplayName = newname;
			this.backupDescription = ParentObject.GetPart<Description>().Short;
			this.instrumentName = newname;
		}

		public string verbForm(string verb){
			string[] words = verb.Split(' ');
			words[0] = ParentObject.GetVerb(words[0]);
			return String.Join(" ",words);
			
		}
	}
}
