using UnityEngine;

public class SoundVFXManager : MonoBehaviour
{
     public static SoundVFXManager Instance{get; private set;} //Only one system in run

     [SerializeField]private AudioSource soundFbxObj;

      private void Awake()
    {
       if(Instance!=null){ 
            Debug.LogError("THERE ARE MORE THAN ONE SoundVFXManager CLASS! "+transform+"-"+Instance+" ");
            Destroy(gameObject);
            return;
        }
         Instance = this;
         
    }

    public void PlaySoundFileClip(AudioClip audioClip, Transform spawnTransform, float volume){
        //Spawn game object and play audio clip
        AudioSource audioSource = Instantiate(soundFbxObj,spawnTransform.position,Quaternion.identity); //spawn
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject,clipLength); //destroy when done
    }
}
