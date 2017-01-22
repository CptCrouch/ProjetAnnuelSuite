using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSoundManager : MonoBehaviour {

    public string rolling = "event:/char_rolling";
    public FMOD.Studio.EventInstance ballRolling;

    FMOD.Studio.ParameterInstance volumeBallRolling;
    FMOD.Studio.ParameterInstance speedBallRolling;

   

    //public FMODUnity.StudioEventEmitter myBallEvent;


   DestructionBehavior destroyBehavior;
    // Use this for initialization
    void Start () {
        destroyBehavior = FindObjectOfType<DestructionBehavior>();

        ballRolling = FMODUnity.RuntimeManager.CreateInstance(rolling);
        ballRolling.getParameter("Volume", out volumeBallRolling);
        ballRolling.getParameter("Speed", out speedBallRolling);
        speedBallRolling.setValue(0.5f);
        
        ballRolling.start();

        
        
    }
	
	// Update is called once per frame
	void Update () {
		
        
        
        
       

	}
}
