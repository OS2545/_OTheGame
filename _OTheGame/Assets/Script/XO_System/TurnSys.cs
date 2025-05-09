using UnityEngine;
using System;

public class TurnSys : MonoBehaviour
{
    public static TurnSys Instance{get; private set;} //Only one system in run

    //----------Event Handlers --------------------------------
    public event EventHandler OnTurnChanged; 

    //-------------Variables--------------------------------
    private int roundNumber;
    private bool isPlayerTurn;

     //----------Awake / Start / Update-----------------------
    private void Awake(){
        if(Instance!=null){ 
            Debug.LogError("THERE ARE MORE THAN ONE TurnSys CLASS! "+transform+"-"+Instance+" ");
            Destroy(gameObject);
            return;
        }
         Instance = this;
         isPlayerTurn = true;
    }

    //--------------------------------
    public void NextTurn(){
        isPlayerTurn = !isPlayerTurn;
        OnTurnChanged?.Invoke(this,EventArgs.Empty);
    }

    public bool IsPlayerTurn(){
        return isPlayerTurn;
    }

}
