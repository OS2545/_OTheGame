using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    

    //----SerializeField
    [SerializeField]private LayerMask MenuPlane; // The layer for interacting

    //----Variable
    private static MouseWorld instance;

     private void Awake()
    {
       
        instance = this;
    }

    private void Update()
    {
        //Update the position of mouse
        transform.position = MouseWorld.GetPosition();
        if(Input.GetMouseButtonDown(0)){
            MouseInteract();
        }
    }

     public static Vector3 GetPosition(){
        //1.The ray of camera will hit something before sending it(true of false depending whether it has object/collider or not) 
        //1.5back to camera/system so ray would be perfect map for click and move  
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit,float.MaxValue,instance.MenuPlane); 
        return raycastHit.point;
    }

    private void MouseInteract(){
        //Interacting with physical object
        if(TurnSys.Instance.IsBetweenRound()) return; //disable interaction when is in between round
        if(!TurnSys.Instance.IsPlayerTurn()) return; //disable interaction when is enemy turn

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Check if hit the paper
        bool isHitMenuLayerMask = Physics.Raycast(ray, out RaycastHit raycastHit,float.MaxValue,MenuPlane); 
        if(isHitMenuLayerMask) {
            if(raycastHit.transform.TryGetComponent<PolicyPaper_Item>(out PolicyPaper_Item policyPaper_item)){
                policyPaper_item.Activate();
            }
            if(raycastHit.transform.TryGetComponent<XO_Item>(out XO_Item xO_Item)){
                xO_Item.isPressed();
            }    
        }
    }
}
