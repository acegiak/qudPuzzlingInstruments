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
	public class acegiak_SongBook : IPart
	{
        [NonSerialized]
        public List<acegiak_Song> Songs = new List<acegiak_Song>();

        public acegiak_SongBook(){
            acegiak_Song celebration = new acegiak_Song();
            celebration.Name = "A Celebration Song";
            celebration.Notes = "349.23,0.01,0.24;349.23,0.26,0.25;329.63,0.5,0.5;261.63,1,0.5;293.66,1.5,0.5;261.63,2,1;";
            Songs.Add(celebration);

			acegiak_Song dramatic = new acegiak_Song();
            dramatic.Name = "A Dramatic Song";
            dramatic.Notes = "311.13,0.01,0.5;261.63,0.5,0.5;369.99,1,1.5";
            Songs.Add(dramatic);

			acegiak_Song challenge = new acegiak_Song();
			challenge.Name = "A New Challenge";
			challenge.Notes = "440,0.01,0.2;479.996,0.5,0.2;550,1,1;479.996,2,0.2;440,2.2,0.2;479.996,2.4,0.2;440,2.6,0.5";
			Songs.Add(challenge);

			acegiak_Song rock = new acegiak_Song();
			rock.Name = "I Love My Pet Rock";
			rock.Notes = "440,0.01,0.3;660,0.01,0.3;495,0.4,0.3;742.5,0.4,0.3;693,0.7,0.3;1039.5,0.7,0.3;440,1.1,0.2;660,1.3,0.15;495,1.5,0.2;742.5,1.7,0.15;693,1.9,0.2;693,2.1,0.15;660,2.2,0.15;693,2.3,0.15;495,2.4,0.15;440,2.5,0.3";
			Songs.Add(rock);

			acegiak_Song sleep = new acegiak_Song();
			sleep.Name = "Go To Sleep";
			sleep.Notes = "330,0.01,2;359,0.3,0.15;462,0.45,2;528,0.45,2;220,2.45,1;264,3.45,1;220,4,2";
			Songs.Add(sleep);

		}

        public string ToString(){
            string ret = "SONGS:";
            foreach(acegiak_Song song in Songs){
                ret += "\n"+song.ToString();
            }
            return ret;
        }


        public void BuildSong(){
            if(ParentObject.GetPart<Inventory>() == null){
                
            }
        }

    }
}