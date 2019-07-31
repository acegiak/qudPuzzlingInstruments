using System;
using UnityEngine;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;
using XRL.World;

namespace XRL.World.Parts
{
[Serializable]
public class acegiak_Song: IPart{

    public string Name;
    
    public string Notes;

    public string Faction;

    public string Effect;

    public List<string> Themes;

    public string ToString(){
        return Name+": "+Notes;
    }

}

public class acegiak_SongMod{

    public float AlterAmount = 0.5f;
    public virtual List<List<float>> AlterNotes(List<List<float>> notes){
        return notes;
    }
}

}