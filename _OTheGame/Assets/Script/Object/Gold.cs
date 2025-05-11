using UnityEngine;

public class Gold : MonoBehaviour
{
   
    
    //---------Variable --------------------------------
    private bool isAnimationPlayed;
    private bool isAnimationEnd;
    private float exitTimer;
 
    //----------Awake / Start / Update-----------------------
    private void Awake()
    {
        isAnimationPlayed = false;
        isAnimationEnd = false;
        exitTimer = 30f;
        
    }

    private void Start()
    {
       isAnimationPlayed = true;
    }

    private void Update()
    {
        //handler gold's animation
        if(isAnimationPlayed){
            Vector3 dir = new Vector3(0,0,-0.1f);
            float moveSpeed = 3.5f;
            transform.position += dir * moveSpeed * Time.deltaTime;

            //stop animation
            Vector3 stopPosition = new Vector3(0.001f,0.121f,-0.113f);
            if(transform.position.z <= stopPosition.z){
                isAnimationPlayed = false;
               isAnimationEnd = true;
              
            }
        }

        if(isAnimationEnd){
            exitTimer-=.1f;
            if(exitTimer<=0){
                Application.Quit();
            }
        }
    }

    
}
