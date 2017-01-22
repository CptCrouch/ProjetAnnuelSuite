using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public FMOD.Studio.EventInstance iDestroy;
    FMOD.Studio.ParameterInstance volumeBallRolling;

    [FMODUnity.EventRef]
    public string destruction = "event:/SFX/destroyChain";
    [FMODUnity.EventRef]
    public string glowEvent = "event:/SFX/tileGlow";


    //public FMODUnity.StudioEventEmitter ekez;


    public GameObject player;


    // Use this for initialization
    void Start () {

        player = GameObject.FindGameObjectWithTag("Player");
        //iDestroy = 
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Sound
    public void EmittGrowSound(int altitudeTarget)
    {
        if(altitudeTarget == 1)
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/tileUp1");
        if (altitudeTarget == 2)
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/tileUp2");
        if (altitudeTarget == 3)
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/tileUp3");
    }


    public FMOD.Studio.EventInstance StartGlowSound()
    {
        FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(glowEvent);
        eventInstance.start();
        return eventInstance;
    }
    public void StopGlowSound(FMOD.Studio.EventInstance glowEvent)
    {
        glowEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void EmittDestroySound(float parameter, int cellAltitude)
    { 
        FMOD.Studio.EventInstance eventInstance = FMODUnity.RuntimeManager.CreateInstance(destruction);
        FMOD.Studio.ParameterInstance parameterEventChain;
        FMOD.Studio.ParameterInstance parameterEventAltitude;
        eventInstance.getParameter("chainValue", out parameterEventChain);
        eventInstance.getParameter("tileHigh", out parameterEventAltitude);

        
        parameterEventChain.setValue(parameter);
        parameterEventAltitude.setValue(cellAltitude);
        eventInstance.start();
        


        //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/destroySingle", playerPos,float);
    }
    #endregion
}
