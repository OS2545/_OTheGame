using UnityEngine;
using System;

public class PolicyPaper_Item : MonoBehaviour
{
    //----------EventHandler-------------------------------
    public event EventHandler OnPolicyPaperAnimationStarted; 
    public event EventHandler OnCheckBoxInteract;

    //---------SerializeField --------------------------------
    [SerializeField] private bool StartTheGame;   
     //----------Awake / Start / Update-----------------------
   
    public void Activate(){
        //Enter the game OR Exit
        OnCheckBoxInteract?.Invoke(this,EventArgs.Empty);
        if(StartTheGame) PlayAnimation();
        else Exit();
        
    }
    private void Exit(){
        //Exit the game
        Application.Quit();
    }

    private void PlayAnimation(){
        //Start the game, Play animation 
        OnPolicyPaperAnimationStarted?.Invoke(this,EventArgs.Empty);
    }

    
}
