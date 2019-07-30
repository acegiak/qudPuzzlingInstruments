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

    public string Effect;

    public string ToString(){
        return Name+": "+Notes;
    }

}
}