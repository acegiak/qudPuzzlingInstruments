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
using Qud.API;
using XRL.World;
using System.Linq;
using HistoryKit;


namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_SongBook : IPart
	{
        [NonSerialized]
        public List<acegiak_Song> Songs = new List<acegiak_Song>();

        public long lastPlayed =0;

        public bool learnedFrom = false;

        public bool made = false;

        public Dictionary <string,List<string>> _factionMusicTags = new Dictionary <string,List<string>>();

        public acegiak_SongBook(){
		}

        public static Dictionary <string,List<string>> factionMusicTags (){
            if(XRLCore.Core.Game.Player == null || 
            XRLCore.Core.Game.Player.Body == null ||
            XRLCore.Core.Game.Player.Body.GetPart<acegiak_SongBook>() == null
            ){ return null;}
            return XRLCore.Core.Game.Player.Body.GetPart<acegiak_SongBook>()._factionMusicTags;
        }

        public string ToString(){
            string ret = "SONGS:";
            foreach(acegiak_Song song in Songs){
                ret += "\n"+song.ToString();
            }
            return ret;
        }
        public void Make(){
            if(!made){
                made = true;
                SongsIKnow();
            }
        }
        public void SongsIKnow(){
            if(ParentObject.IsPlayer()){
                // Songs = GetBaseSongs().Select(b=>new acegiak);
                return;
            }
            for(int i = Stat.Rnd2.Next(3);i>0;i--){
                acegiak_Song song = SongOfMyPeople();
                if(song != null){
                    Songs.Add(song);
                }
            }
        }

        public acegiak_Song SongOfMyPeople(){
            if(ParentObject == null || ParentObject.GetPart<Brain>() == null || ParentObject.GetPart<Brain>().GetPrimaryFaction() == null){
                return null;
            }
            List<string> tags = FactionTags(ParentObject.GetPart<Brain>().GetPrimaryFaction());
            List<acegiak_Song> ElligbleSongs = new List<acegiak_Song>();
            foreach (GameObject item in GetBaseSongs()){
                if(item.HasTag("musictags")){
                    List<string> songtags = item.GetTag("musictags").Split(',').ToList();
                    if(tags.Where(b=>songtags.Contains(b)).Any()){
                        acegiak_Song Song = new acegiak_Song();
                        Song.Notes = item.GetTag("musicnotes");
                        Song.Name = item.GetBlueprint().Name;
                        Song.Effect = item.GetTag("musiceffect");
                        
                        ElligbleSongs.Add(Song);
                    }
                }
            }
            if(ElligbleSongs.Count() <= 0){
                return null;
            }
            return MakeItCultural(ElligbleSongs.GetRandomElement(),ParentObject.GetPart<Brain>().GetPrimaryFaction());
        }

        public acegiak_Song MakeItCultural(acegiak_Song song, string faction){
            return song;
        }

        public List<GameObject> GetBaseSongs(){
            List<GameObject> BaseSongs = new List<GameObject>();
            if(GameObjectFactory.Factory == null || GameObjectFactory.Factory.BlueprintList == null){
                return BaseSongs;
            }
			foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.BlueprintList)
			{
				if (!blueprint.IsBaseBlueprint() && blueprint.DescendsFrom("Song"))
				{
					GameObject sample = GameObjectFactory.Factory.CreateSampleObject(blueprint.Name);
                    if(sample.HasTag("musicnotes")){
                        BaseSongs.Add(sample);
                    }
				}
			}
            return BaseSongs;
        }

		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "AIBored");

			Object.RegisterPartEvent(this, "VisitConversationNode");
			Object.RegisterPartEvent(this, "ShowConversationChoices");

			Object.RegisterPartEvent(this,"VisibleStatusColor");
			Object.RegisterPartEvent(this,"GetDisplayName");
			Object.RegisterPartEvent(this,"GetShortDisplayName");
			base.Register(Object);
        }


		public override bool BeforeRender(Event E){
			Make();
			return base.BeforeRender(E);
		}

		public override bool FireEvent(Event E)
		{
            if(E.ID=="AIBored"){
                if(ParentObject.GetPart<Inventory>() != null){
                    if(XRLCore.Core.Game.TimeTicks - lastPlayed > 20){
                        lastPlayed = XRLCore.Core.Game.TimeTicks;
                    
                        foreach(GameObject GO in ParentObject.GetPart<Inventory>().GetObjects()){
                            if(GO.GetPart<acegiak_Musical>() != null){
                                GO.FireEvent(XRL.World.Event.New("InvCommandPlayTune", "Owner", ParentObject,"Object",GO));
                            }
                        }
                    }
                }
            }



			if(E.ID == "ShowConversationChoices" ){
				if(XRLCore.Core.Game.Player.Body.GetPart<acegiak_SongBook>()!= null ){
					if(this.Songs.Count > 0 && !this.learnedFrom ){
					
						
						if(E.GetParameter<ConversationNode>("CurrentNode") != null && E.GetParameter<ConversationNode>("CurrentNode") is WaterRitualNode){
							WaterRitualNode wrnode = E.GetParameter<ConversationNode>("CurrentNode") as WaterRitualNode;
							List<ConversationChoice> Choices = E.GetParameter<List<ConversationChoice>>("Choices") as List<ConversationChoice>;

							if(Choices.Where(b=>b.ID == "LearnSong").Count() <= 0){

								bool canlearn = XRLCore.Core.Game.PlayerReputation.get(ParentObject.pBrain.GetPrimaryFaction()) >50;

								ConversationChoice conversationChoice = new ConversationChoice();
								conversationChoice.Text = (canlearn?"&G":"&K")+"Teach me to play &W"+this.Songs[0].Name+(canlearn?"&g":"&K")+" ["+(canlearn?"&C":"&r")+"-50"+(canlearn?"&g":"&K")+" reputation]";
								conversationChoice.GotoID = "End";
								conversationChoice.ParentNode = wrnode;
								conversationChoice.ID = "LearnSong";
								conversationChoice.onAction = delegate()
								{
									if(!canlearn){
										Popup.Show("You do not have enough reputation.");
										return false;
									}
                                    XRLCore.Core.Game.Player.Body.GetPart<acegiak_SongBook>().Songs.Add(this.Songs[0]);
									this.learnedFrom = true;
									Popup.Show("You learned to play "+this.Songs[0].Name);
									XRLCore.Core.Game.PlayerReputation.modify(Factions.FactionList[ParentObject.pBrain.GetPrimaryFaction()].Name, -50,false);

									return true;
								};
								Choices.Add(conversationChoice);
								Choices.Sort(new ConversationChoice.Sorter());
								// wrnode.Choices.Add(conversationChoice);
								// wrnode.SortEndChoicesToEnd();
								E.SetParameter("CurrentNode",wrnode);
							}
						}
					
					}
				}
			}
            return base.FireEvent(E);
        }


        public static List<string> FactionTags(string factionName){
            if(factionMusicTags() == null){
                return new List<string>();
            }
            if(factionMusicTags().ContainsKey(factionName)){
                return factionMusicTags()[factionName];
            }
            GameObject sample = EncountersAPI.GetASampleCreatureFromFaction(factionName);
            //IPart.AddPlayerMessage(factionName);
            List<string> tags = FromCreatureTags(sample);
            factionMusicTags()[factionName] = tags;
            return tags;
        }

        public static List<string> FromCreatureTags(GameObject sample){
            List<string> tags = new List<string>();
            if(sample == null){
                return tags;
            }
            for(int i = 0; i<2;i++){
                if(sample.GetPart<Inventory>() != null){
                    GameObject samplePossession = sample.GetPart<Inventory>().GetObjects().GetRandomElement();
                    if(samplePossession != null && samplePossession.pPhysics != null){
                        tags.Add(samplePossession.pPhysics.Category);

                    }
                }
            }
            for(int i = 0; i<2;i++){
                if(sample.GetPart<Body>() != null){
                    BodyPart samplePart = sample.GetPart<Body>().GetParts().Where(p=>!p.Extrinsic && !p.Abstract).GetRandomElement();
                    if(samplePart != null){
                        tags.Add(samplePart.Type);
                    }
                }
            }

            tags.Add(sample.pRender.GetForegroundColor());
            tags.Add(sample.pRender.DetailColor);
            return tags;

        }

    }
}