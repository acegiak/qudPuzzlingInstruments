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


namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_SongBook : IPart
	{
        [NonSerialized]
        public List<GameObject> Songs = new List<GameObject>();

        public long lastPlayed =0;

        public acegiak_SongBook(){
            if(GameObjectFactory.Factory == null || GameObjectFactory.Factory.BlueprintList == null){
                return;
            }
			foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.BlueprintList)
			{
				if (!blueprint.IsBaseBlueprint() && blueprint.DescendsFrom("Song"))
				{
					GameObject sample = GameObjectFactory.Factory.CreateSampleObject(blueprint.Name);
                    if(sample.GetPart<acegiak_Song>() != null){
                        Songs.Add(sample);
                    }
				}
			}

		}

        public string ToString(){
            string ret = "SONGS:";
            foreach(GameObject song in Songs){
                ret += "\n"+song.GetPart<acegiak_Song>().ToString();
            }
            return ret;
        }



		public override bool AllowStaticRegistration()
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "AIBored");
        }


		public override bool FireEvent(Event E)
		{
            if(E.ID=="AIBored"){
                if(ParentObject.GetPart<Inventory>() != null){
                    if(XRLCore.Core.Game.TimeTicks - lastPlayed > 12){
                        lastPlayed = XRLCore.Core.Game.TimeTicks;
                    
                        foreach(GameObject GO in ParentObject.GetPart<Inventory>().GetObjects()){
                            if(GO.GetPart<acegiak_Musical>() != null){
                                GO.FireEvent(XRL.World.Event.New("InvCommandPlayTune", "Owner", ParentObject,"Object",GO));
                            }
                        }
                    }
                }
            }
            return base.FireEvent(E);
        }


        public static List<string> FactionTags(string factionName){
            GameObject sample = EncountersAPI.GetASampleCreatureFromFaction(factionName);
            return FromCreatureTags(sample);
        }

        public static List<string> FromCreatureTags(GameObject sample){
            List<string> tags = new List<string>();
            if(sample == null){
                return tags;
            }

            if(sample.GetPart<Inventory>() != null){
                GameObject samplePossession = sample.GetPart<Inventory>().GetObjects().GetRandomElement();
                if(samplePossession != null && samplePossession.pPhysics != null){
                    tags.Add(samplePossession.pPhysics.Category);

                }
            }

            if(sample.GetPart<Body>() != null && sample.GetPart<Body>().Part){
                BodyPart samplePart = sample.GetPart<Body>().GetParts().GetRandomElement();
                if(samplePart != null){
                    tags.Add(samplePart.Type);
                }
            }

            tags.Add(sample.pRender.GetForegroundColor());
            tags.Add(sample.pRender.DetailColor);

        }

    }
}