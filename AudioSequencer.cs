using System;
using UnityEngine;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.Generic;


public class acegiak_Note{
    public static float basevolume = 2f;
    public string sample;
    public float pitch;
    public float begin;
    public float length;

    public float volume = basevolume;


    private GameObject _voiceHolder;
    public AudioSource voice{
        get{
            if(_voiceHolder == null){
                UnityEngine.GameObject gameObject;
                gameObject = new UnityEngine.GameObject();
                gameObject.transform.position = new Vector3(0f, 0f, 1f);
                gameObject.name = "PuzzlingInstrumentSound";
                gameObject.AddComponent<AudioSource>();
                gameObject.AddComponent<AudioLowPassFilter>();
                gameObject.GetComponent<AudioLowPassFilter>().cutoffFrequency = 22000f * 0.7f;
                gameObject.GetComponent<AudioSource>().clip = SoundManager.GetClip("Level_Up_Other");
                gameObject.GetComponent<AudioSource>().pitch = 1f;
                gameObject.GetComponent<AudioSource>().volume = basevolume;
                gameObject.GetComponent<AudioSource>().loop = true;
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                _voiceHolder = gameObject;
            }
            return _voiceHolder.GetComponent<AudioSource>();
        }
    }
    
    public acegiak_Note(string Sample,float Pitch,float Begin,float Length){
        this.sample = Sample;
        this.pitch = Pitch;
        this.begin = Begin;
        this.length = Length;
    }

    public void Play(){
        voice.clip = SoundManager.GetClip(sample);
        voice.pitch = this.pitch;
        voice.loop = true;
        voice.volume = this.volume;
        voice.Play();
    }

    public void Stop(){
        voice.Stop();
        UnityEngine.Object.Destroy(_voiceHolder);
        _voiceHolder = null;
    }

    public string ToString(){
        return Math.Round(acegiak_AudioSequencer.baseHz*pitch,3).ToString()+","+Math.Round(begin,3).ToString()+","+Math.Round(length,3).ToString()+";";
    }
}


public class acegiak_AudioSequencer : MonoBehaviour
{
    public static float baseHz=220f;
    public List<acegiak_Note> notes;
    public float? time = null;
    public float? Rtime = null;
    public GameObject GO;


    Dictionary<string,float> keynotes = new Dictionary<string,float>(){
        {"1",174.61f},
        {"2",196.00f},
        {"3",220.00f},
        {"4",246.94f},
        {"5",261.63f},
        {"6",293.66f},
        {"7",329.63f},
        {"8",349.23f},
        {"9",392.00f},
        {"0",440.00f},
        {"-",493.88f},
        {"=",523.25f}
    };

    public string recordVoice = "oboe";
    public float recordVolume = acegiak_Note.basevolume;
    public void Play(){
        time = 0;
    }

    public void Record(){
        Rtime = 0;
        notes = new List<acegiak_Note>();
        Debug.Log("STARTED RECORDING");
    }
    public string Print(){
        string ret = "";
        foreach(acegiak_Note note in notes){
            ret += note.ToString();
        }
        return ret;
    }
    
    public void Update()
    {
        if(time != null && notes != null){
            foreach(acegiak_Note note in notes){
                if(note.begin > time && note.begin < time+Time.deltaTime){
                    Debug.Log("start note");
                    note.Play();
                }
                if(note.begin+note.length > time && note.begin+note.length < time+Time.deltaTime){
                    Debug.Log("stop note");
                    note.Stop();
                }
            }

            time += Time.deltaTime;
        }
        
        

        if(Rtime != null){
            foreach(string i in keynotes.Keys.ToList()){
                float rtime = Rtime ?? 0f;
                if (Input.GetKeyDown(i)){
                    if(notes.Count == 0){
                        Rtime = 0.01f;
                        rtime = 0.01f;
                    }
                    acegiak_Note note =  new acegiak_Note(recordVoice,HzToMulti((keynotes[i]*2)),rtime,0f);
                    note.volume = recordVolume;
                    note.Play();
                    notes.Add(note);
                    Debug.Log("START: "+i );
                }
                if (Input.GetKeyUp(i)){
                    foreach(acegiak_Note note in notes){
                        if(note.pitch == HzToMulti((keynotes[i]*2)) && note.length == 0){
                            note.length = rtime - note.begin;
                            note.Stop();

                            Debug.Log("END: "+i );
                        }
                    }
                }
            }
            if (Input.GetKeyDown("return") || Input.GetKeyDown("escape")){
                Rtime = null;
            }

            Rtime += Time.deltaTime;
        }

    }

    public void Read(string SampleName, string NoteSequence, string _volume){
        float volume = 1f;
        if(_volume != null && _volume != String.Empty){
            volume = Int32.Parse(_volume)/100f;
        }
        this.notes = new List<acegiak_Note>();
        foreach(string note in NoteSequence.Split(';')){
            if(note != null && note.Length > 0){
                string[] parts = note.Split(',');
                acegiak_Note noot = new acegiak_Note(SampleName,HzToMulti(ParseFloat(parts[0])),ParseFloat(parts[1]),ParseFloat(parts[2]));
                if(parts.Length > 3){
                    noot.volume = volume*ParseFloat(parts[3]);
                }else{
                    noot.volume = volume;
                }
                this.notes.Add(noot);
            }
        }
    }

    public static float ParseFloat(string f){
        //Debug.Log("PARSEFLOAT:"+f);
        return (float)double.Parse(f,System.Globalization.NumberStyles.AllowDecimalPoint);
    }
    public static float HzToMulti(float targetHz){
        return targetHz/baseHz;
    }
}