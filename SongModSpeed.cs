using System;
using UnityEngine;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using XRL.World;

namespace XRL.World.Parts
{
    public class acegiak_SongModSpeed:acegiak_SongMod{
        public override List<List<float>> AlterNotes(List<List<float>> notes){
            //IPart.AddPlayerMessage("Clipping Notes");
            foreach(List<float> note in notes){
                if(note.Count >2){
                    note[1] = note[1]*this.AlterAmount;
                    note[2] = note[2]*this.AlterAmount;
                }
            }
            return notes;
        }
    }

}