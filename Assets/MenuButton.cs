using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "GameScene";
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}