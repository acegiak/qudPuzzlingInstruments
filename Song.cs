using System;
using UnityEngine;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;

[Serializable]
public class acegiak_Song{
    public string Name;
    public string Notes;

    public string ToString(){
        return Name+": "+Notes;
    }

}