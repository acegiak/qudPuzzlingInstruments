using System;
using UnityEngine;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;
using XRL.World;

namespace XRL.World.Parts
{
public class acegiak_SongModPitchShift:acegiak_SongMod{
    public override List<List<float>> AlterNotes(List<List<float>> notes){
        //IPart.AddPlayerMessage("Pitch Shifting Notes");
        foreach(List<float> note in notes){
            if(note.Count >2){
                note[0] = note[0]*this.AlterAmount;
            }
        }
        return notes;
    }
}

}