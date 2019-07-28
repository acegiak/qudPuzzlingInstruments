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
        }

        public string ToString(){
            string ret = "SONGS:";
            foreach(acegiak_Song song in Songs){
                ret += "\n"+song.ToString();
            }
            return ret;
        }
    }
}