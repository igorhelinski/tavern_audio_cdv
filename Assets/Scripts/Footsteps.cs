using UnityEngine;
using FMODUnity;
using FMOD.Studio;

/// <summary>
/// Zarządza odtwarzaniem dźwięków kroków, skoków i lądowania w zależności od powierzchni.
/// </summary>
public class Footsteps : MonoBehaviour
{
    // Publiczne referencje do zdarzeń FMOD.
    public EventReference footstepsEvent;
    public EventReference jumpEvent;
    public EventReference landEvent;


    private float lastFootstepTime = 0f;
    private float distToGround;

    [SerializeField]
    private bool isGrounded = true;
    [SerializeField]
    private bool isJumping = false;

    // FMOD - Instancje zdarzeń.
    EventInstance footstepsSoundInstance;
    EventInstance jumpSoundInstance;
    EventInstance landSoundInstance;

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;

        footstepsSoundInstance = RuntimeManager.CreateInstance(footstepsEvent);
        jumpSoundInstance = RuntimeManager.CreateInstance(jumpEvent);
        landSoundInstance = RuntimeManager.CreateInstance(landEvent);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayJump();
        }
    }

    void FixedUpdate()
    {
        HandleFootsteps();
    }

    /// <summary>
    /// Obsługuje logikę odtwarzania dźwięków kroków.
    /// </summary>
    private void HandleFootsteps()
    {
        // Sprawdza, czy gracz się porusza.
        bool isMoving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        // Sprawdza, czy gracz biegnie.
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving && IsGrounded())
        {
            // Ustawia interwał na podstawie tego, czy gracz biegnie.
            float footstepInterval = isRunning ? 0.25f : 0.5f;

            if (Time.time - lastFootstepTime > footstepInterval)
            {
                lastFootstepTime = Time.time;
                PlayFootsteps();
            }
        }
    }

    /// <summary>
    /// Odtwarza dźwięk kroków w zależności od powierzchni.
    /// </summary>
    private void PlayFootsteps()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
        {
            string surfaceTag = hit.collider.tag;
            PlaySurfaceSound(footstepsSoundInstance, footstepsEvent, surfaceTag);
        }
    }

    /// <summary>
    /// Odtwarza dźwięk skoku.
    /// </summary>
    private void PlayJump()
    {
        if (IsGrounded())
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
            {
                string surfaceTag = hit.collider.tag;
                PlaySurfaceSound(jumpSoundInstance, jumpEvent, surfaceTag);
            }
            isGrounded = false;
            isJumping = true;
        }
    }

    /// <summary>
    /// Obsługuje dźwięk lądowania po skoku.
    /// </summary>
    private void OnCollisionEnter(Collision col)
    {
        if (!isGrounded && isJumping)
        {
            PlayLanding();
        }
    }

    /// <summary>
    /// Odtwarza dźwięk lądowania.
    /// </summary>
    private void PlayLanding()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, distToGround + 0.5f))
        {
            string surfaceTag = hit.collider.tag;
            PlaySurfaceSound(landSoundInstance, landEvent, surfaceTag);
        }
        isGrounded = true;
        isJumping = false;
    }

    /// <summary>
    /// Ogólna metoda do odtwarzania dźwięku na podstawie tagu powierzchni.
    /// </summary>
    private void PlaySurfaceSound(EventInstance soundInstance, EventReference eventRef, string surfaceTag)
    {
        // sprawdz czy paramet istnieje
        switch (surfaceTag)
        {
            case "rock":
                break;
            case "wood":
                break;
            default:
                return; // nie znaleziono - wyjdz
        }

        // Jeśli znaleziono pasujący parametr, odtwórz dźwięk.
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        soundInstance.setParameterByNameWithLabel("footsteps", surfaceTag);
        soundInstance.start();
    }

    private void OnDestroy()
    {
        footstepsSoundInstance.release();
        jumpSoundInstance.release();
        landSoundInstance.release();
    }

    /// <summary>
    /// Sprawdza, czy gracz znajduje się na podłożu.
    /// </summary>
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.5f);
    }  
}