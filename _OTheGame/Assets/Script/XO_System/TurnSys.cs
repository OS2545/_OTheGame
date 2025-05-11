using UnityEngine;
using System;

public class TurnSys : MonoBehaviour
{
    public static TurnSys Instance{get; private set;} //Only one system in run

    //----------Event Handlers --------------------------------
    public event EventHandler OnTurnChanged; 
    public event EventHandler OnRoundFinish;
    public event EventHandler OnRoundStarted;

    //----------SerialField--------------------------------
    [SerializeField]private GameObject Gold;

    //-------------Variables--------------------------------
    private int roundNumber;
    private int maxRound=3;
    private bool isPlayerTurn;
    private bool isBetweenRound;
    private bool isGameEnd;

     //----------Awake / Start / Update-----------------------
    private void Awake(){
        if(Instance!=null){ 
            Debug.LogError("THERE ARE MORE THAN ONE TurnSys CLASS! "+transform+"-"+Instance+" ");
            Destroy(gameObject);
            return;
        }
         Instance = this;
         isPlayerTurn = true;
         roundNumber=0;
         isBetweenRound = false;
         isGameEnd = false;
    }

    private void Start()
    {
        //Subscribe
        XOSys.Instance.OnFinishRound += XOSys_OnFinishRound;
        UISys.Instance.OnUINextRound += UISys_OnUINextRound;
        
    }

   
    //--------------------------------
    public void NextTurn(bool isEnd){
        //Move on to next turn
        if(isEnd) isGameEnd = true; //Game end no next round
        isPlayerTurn = !isPlayerTurn;
        OnTurnChanged?.Invoke(this,EventArgs.Empty);
    }

    public bool IsPlayerTurn(){
        return isPlayerTurn;
    }

    public int GetRound(){
        return roundNumber;
    }


    public bool IsBetweenRound(){
        return isBetweenRound;
    }

    public int GetMaxRound(){
        return maxRound;
    }

    public bool IsGameEnd(){
        return isGameEnd;
    }

    //-------------Subscribe
    private void UISys_OnUINextRound(object sender, EventArgs e){
        if(isPlayerTurn && roundNumber<maxRound){
            //On To Next Round
            OnRoundStarted?.Invoke(this,EventArgs.Empty);
            isBetweenRound = false;
            return;
        }

        else if(!isPlayerTurn){
            //Fail the game
            //Check UI Sys
            return;
        }

        else {
            //Win the game
            Gold.gameObject.SetActive(true);
        }
    }
    private void XOSys_OnFinishRound(object sender, EventArgs e){
        roundNumber++;
        OnRoundFinish?.Invoke(this,EventArgs.Empty);
        //UICanvas.gameObject.SetActive(true);
        isBetweenRound = true;
        //UIisActivate = true;
    }

}
