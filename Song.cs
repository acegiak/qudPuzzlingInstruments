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

    public string _name;
    public string Name{
        get{
            if(_name == null){
                if(ParentObject != null){
                    _name = ParentObject.GetBlueprint().Name;
                }else{
                    _name = "a song";
                }
            }
            return _name;
        }
        set{
            _name = value;
        }
    }
    public string Notes;

    public string ToString(){
        return Name+": "+Notes;
    }

}
}