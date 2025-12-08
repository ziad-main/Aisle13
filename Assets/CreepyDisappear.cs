using UnityEngine;

public class CreepyDisappear : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private Renderer figureRenderer;
    
    private Material material;
    private bool isFading = false;
    
    void Start()
    {
        if (figureRenderer != null)
        {
            // Create instance of material to not affect other objects
            material = figureRenderer.material;
        }
    }
    
    public void StartFade()
    {
        if (!isFading)
            StartCoroutine(FadeOut());
    }
    
    private System.Collections.IEnumerator FadeOut()
    {
        isFading = true;
        float elapsed = 0f;
        Color startColor = material.color;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
}