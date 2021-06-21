using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
   [SerializeField] float rcsThrust = 100f;
   [SerializeField] float mainThrust = 100f;
   [SerializeField] float levelLoadDelay = 2f;
   [SerializeField] AudioClip mainEngine;
   [SerializeField] AudioClip death;
   [SerializeField] AudioClip success;

   [SerializeField] ParticleSystem mainEngineParticles;
   [SerializeField] ParticleSystem deathParticles;
   [SerializeField] ParticleSystem successParticles;
    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;

    bool collisionsDisabled = false;

	// Use this for initialization
	void Start () 
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        // todo make the sound stop playing after death
        if (!isTransitioning)
        {
        RespondToRotateInput();
        RespondToThrustInput(); 
        }
        
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        
	}   

    void RespondToDebugKeys() 
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C)) {
            collisionsDisabled = !collisionsDisabled; //toggle on and off basically
        }
    }

    void OnCollisionEnter(Collision collision) 
    {
        if (isTransitioning || collisionsDisabled) { return; } //ignore collisions when dead. return makes function not proceed.
        switch(collision.gameObject.tag) 
        {
            case "Friendly":
            print("ok!");
            break;

            case "Hostile":
            DeathSequence();
            break;

            case "Finish":
            SuccessSequence();
            break;
        }
    }

    void DeathSequence()
    {
            isTransitioning = true;
            audioSource.Stop();
            audioSource.PlayOneShot(death);
            deathParticles.Play();
            Invoke("LoadFirstLevel", levelLoadDelay);

    }

    void SuccessSequence()
    {
            isTransitioning = true;
            audioSource.Stop();
            audioSource.PlayOneShot(success);
            successParticles.Play();
            Invoke("LoadNextLevel", levelLoadDelay);
    }

    void LoadNextLevel() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) //if last scene index == scene count in build settings return to index 0 (level 1)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex); 
    }

    void LoadFirstLevel() 
    {
        SceneManager.LoadScene(0);
        
    }
    void RespondToThrustInput() 
    {
         if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }
    }

    void StopApplyingThrust()
    {
       audioSource.Stop();
       mainEngineParticles.Stop();
    }

    void ApplyThrust()
    {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); //Time.deltaTime);
            if (!audioSource.isPlaying) // so it doesn't layer
            {
                audioSource.PlayOneShot(mainEngine);
            }

            mainEngineParticles.Play();
    }
        private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero; //remove rotation due to physics 

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

    }
}
