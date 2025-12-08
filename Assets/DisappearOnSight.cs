using UnityEngine;
using System.Collections;

public class DisappearOnSight : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float viewAngleThreshold = 30f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private LayerMask obstacleLayer;
    
    [Header("References")]
    [SerializeField] private GameObject figureVisual;
    [SerializeField] private Renderer figureRenderer; // MeshRenderer, SkinnedMeshRenderer, or SpriteRenderer
    
    [Header("Stutter Settings")]
    [SerializeField] private float timeBeforeStutter = 0.1f;
    [SerializeField] private int stutterCount = 5; // How many flickers
    [SerializeField] private float stutterMinInterval = 0.05f;
    [SerializeField] private float stutterMaxInterval = 0.15f;
    [SerializeField] private bool useAlphaFlicker = true; // Fade flicker vs on/off flicker
    
    [Header("Audio")]
    [SerializeField] private AudioSource disappearSound;
    [SerializeField] private AudioClip[] stutterSounds; // Random glitch sounds

    [Header("Prompt Message")]
    [SerializeField] private string promptMessage = "Weird... She looks like Sarah";
    [SerializeField] private float promptDisplayDuration = 3f;

    public Camera playerCamera;
    private Aim aimScript;
    private float lookTimer = 0f;
    private bool hasDisappeared = false;
    private bool isStuttering = false;
    private bool hasShownPrompt = false;
    private Material material;
    private Color originalColor;
    
    void Start()
    {
        if (figureVisual == null)
            figureVisual = gameObject;

        // Auto-find renderer if not assigned
        if (figureRenderer == null)
            figureRenderer = GetComponentInChildren<Renderer>();

        // Cache material for alpha flickering
        if (figureRenderer != null)
        {
            material = figureRenderer.material;
            originalColor = material.color;
        }

        // Find Aim script from the Camera GameObject
        if (playerCamera != null)
        {
            aimScript = playerCamera.GetComponent<Aim>();
            if (aimScript == null)
            {
                Debug.LogWarning("Aim script not found on Camera GameObject!");
            }
        }
    }
    
    void Update()
    {
        if (hasDisappeared || isStuttering) return;

        if (IsPlayerLookingAtMe())
        {
            // Show prompt message on first look
            if (!hasShownPrompt && aimScript != null)
            {
                hasShownPrompt = true;
                StartCoroutine(ShowPromptMessage());
            }

            lookTimer += Time.deltaTime;

            if (lookTimer >= timeBeforeStutter)
            {
                StartCoroutine(StutterDisappear());
            }
        }
        else
        {
            lookTimer = 0f;
        }
    }
    
    private bool IsPlayerLookingAtMe()
    {
        if (playerCamera == null)
        {
            Debug.Log("Player camera not found.");
            return false;
        }
        
        Vector3 directionToFigure = (transform.position - playerCamera.transform.position).normalized;
        float distance = Vector3.Distance(playerCamera.transform.position, transform.position);
        
        if (distance > maxDistance) return false;
        
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToFigure);
        if (angle > viewAngleThreshold) return false;
        
        float dot = Vector3.Dot(playerCamera.transform.forward, directionToFigure);
        if (dot < 0) return false;
        
        if (Physics.Raycast(playerCamera.transform.position, directionToFigure, out RaycastHit hit, distance, obstacleLayer))
        {
            if (hit.transform != transform && !hit.transform.IsChildOf(transform))
                return false;
        }
        
        return true;
    }
    
    private IEnumerator ShowPromptMessage()
    {
        aimScript.promptText.text = promptMessage;
        yield return new WaitForSecondsRealtime(promptDisplayDuration);        
        aimScript.promptText.text = "";
    }

    private IEnumerator StutterDisappear()
    {
        isStuttering = true;
        
        for (int i = 0; i < stutterCount; i++)
        {
            // Play random glitch sound
            PlayRandomStutterSound();
            
            if (useAlphaFlicker && material != null)
            {
                // Alpha flicker - more ghostly
                float randomAlpha = Random.Range(0f, 0.5f);
                material.color = new Color(originalColor.r, originalColor.g, originalColor.b, randomAlpha);
                
                yield return new WaitForSeconds(Random.Range(stutterMinInterval, stutterMaxInterval));
                
                // Briefly visible again
                material.color = originalColor;
            }
            else
            {
                // On/off flicker - more jarring
                figureVisual.SetActive(false);
                
                yield return new WaitForSeconds(Random.Range(stutterMinInterval, stutterMaxInterval));
                
                // Chance to stay visible briefly (makes it unpredictable)
                if (Random.value > 0.3f)
                {
                    figureVisual.SetActive(true);
                    yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
                }
            }
        }
        
        // Final disappear
        FinalDisappear();
    }
    
    private void FinalDisappear()
    {
        hasDisappeared = true;
        
        if (disappearSound != null)
            disappearSound.Play();
        
        if (material != null)
            material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        figureVisual.SetActive(false);
    }
    
    private void PlayRandomStutterSound()
    {
        if (stutterSounds != null && stutterSounds.Length > 0 && disappearSound != null)
        {
            AudioClip clip = stutterSounds[Random.Range(0, stutterSounds.Length)];
            disappearSound.PlayOneShot(clip, Random.Range(0.5f, 1f));
        }
    }
    
    public void ResetFigure()
    {
        hasDisappeared = false;
        isStuttering = false;
        hasShownPrompt = false;
        lookTimer = 0f;

        if (material != null)
            material.color = originalColor;

        figureVisual.SetActive(true);
    }
}