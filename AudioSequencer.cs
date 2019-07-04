using System;
using UnityEngine;
using RuntimeAudioClipLoader;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;


public class acegiak_Note{
    public string sample;
    public float pitch;
    public float begin;
    public float length;

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
                gameObject.GetComponent<AudioLowPassFilter>().cutoffFrequency = 22000f * (0.15f + 0.55f) * 1f;
                gameObject.GetComponent<AudioSource>().clip = SoundManager.GetClip("Level_Up_Other");
                gameObject.GetComponent<AudioSource>().pitch = 1f;
                gameObject.GetComponent<AudioSource>().volume = 1f;
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
        voice.volume = 1f;
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
            for(int i=0;i<10;i++){
                float rtime = Rtime ?? 0f;
                if (Input.GetKeyDown(i.ToString())){
                    if(notes.Count == 0){
                        Rtime = 0.01f;
                        rtime = 0.01f;
                    }
                    acegiak_Note note =  new acegiak_Note("oboe",HzToMulti((i+1)*110f),rtime,0f);
                    note.Play();
                    notes.Add(note);
                    Debug.Log("START: "+i.ToString() );
                }
                if (Input.GetKeyUp(i.ToString())){
                    foreach(acegiak_Note note in notes){
                        if(note.pitch == HzToMulti((i+1)*110f) && note.length == 0){
                            note.length = rtime - note.begin;
                            note.Stop();

                            Debug.Log("END: "+i.ToString() );
                        }
                    }
                }
            }
            if (Input.GetKeyDown("return")){
                Rtime = null;
            }

            Rtime += Time.deltaTime;
        }




    }

    public void Read(string SampleName, string NoteSequence){
        this.notes = new List<acegiak_Note>();
        foreach(string note in NoteSequence.Split(';')){
            if(note != null && note.Length > 0){
                string[] parts = note.Split(',');
                this.notes.Add(new acegiak_Note(SampleName,HzToMulti(ParseFloat(parts[0])),ParseFloat(parts[1]),ParseFloat(parts[2])));
            }
        }
    }

    public float ParseFloat(string f){
        return (float)double.Parse(f,System.Globalization.NumberStyles.AllowDecimalPoint);
    }
    public float HzToMulti(float targetHz){
        return targetHz/baseHz;
    }
}