using System;
using System.Linq;
using UnityEngine;

public class XO_Item : MonoBehaviour
{
    //----------SerializeField---------------------
   [SerializeField] private Vector2 XO_Position;

   //------------Variables---------------------------
   private bool isPlayerXO;
   private bool isOccupied;
   private bool isSpecialTile;
   private Special_Tile specialTile;
   
   //-------------Awake/Start/Update----------------------
   private void Start()
   {
    isOccupied = false;
   }


    public void isPressed(){
        if(isOccupied) return; //if is taken return;
        isPlayerXO = TurnSys.Instance.IsPlayerTurn();
        if(XOSys.Instance.TryXO(this)){
            //Have been add into the active list
            isOccupied=true;
            return;
        }
   }

   public Vector2 GetPosition(){
    return XO_Position;
   }
   public bool IsPlayerXO(){
    return isPlayerXO;
   }

   public void SetSpecialTile(bool _specialTile, Special_Tile _special_Tile){
        isSpecialTile = _specialTile;
        specialTile = _special_Tile;
   }

   public Special_Tile GetSpecialTile(){
    return specialTile;
   }

   public bool IsSpecialTile(){
        return isSpecialTile;
   }

    public void Reset()
    {
        isOccupied=false;
        isPlayerXO = new bool();
        isSpecialTile = false;
        specialTile = null;
    }

    public void SetIsPlayer(bool _isPlayer){
        isPlayerXO = _isPlayer;
    }

}
