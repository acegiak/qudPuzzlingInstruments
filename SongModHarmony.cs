using System;
using UnityEngine;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;
using XRL.World;

namespace XRL.World.Parts
{
    public class acegiak_SongModHarmony:acegiak_SongMod{
        public override List<List<float>> AlterNotes(List<List<float>> notes){
            //IPart.AddPlayerMessage("Clipping Notes");
            List<List<float>> newNotes = new List<List<float>>();
            foreach(List<float> note in notes){
                if(note.Count >2){
                    newNotes.Add(new List<float>{note[0]*AlterAmount,note[1],note[2]});
                }
            }
            foreach(List<float> note in newNotes){
                notes.Add(note);
            }
            return notes;
        }
    }

}