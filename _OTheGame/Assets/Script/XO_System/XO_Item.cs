using System;
using System.Linq;
using UnityEngine;

public class XO_Item : MonoBehaviour
{
   [SerializeField] private Vector2 XO_Position;
   private bool isPlayerXO;


    public void isPressed(){
        if(XOSys.Instance.CanAddXO(this)){
            isPlayerXO = TurnSys.Instance.IsPlayerTurn();
            TurnSys.Instance.NextTurn(); //Next turn
            return;
        }
   }

   public Vector2 GetPosition(){
    return XO_Position;
   }
   public bool IsPlayerXO(){
    return isPlayerXO;
   }
   
}
