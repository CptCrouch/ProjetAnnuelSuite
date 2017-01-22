using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadTargetSceneButton : MonoBehaviour {

    public GameObject MenuBase;
    public GameObject Crédits;

    public Image creditImage;
    public Button creditButton;

    public Image fade;
    public float speedFade;

   /* public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;*/

    void Awake()
    {
        StartCoroutine(FadeOutStart());
        //Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

    }

	public void LoadSceneNum(int num)
    {
        if(num < 0 || num >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("Can't load scene num " + ", SceneManager only has " + SceneManager.sceneCountInBuildSettings + " scenes in BuildSettings!");
            return;
        }
        
        LoadingScreenManager.LoadScene(num);
        
    }
    public void LaunchPlayScene(int num)
    {
        StartCoroutine(FadeInLaunchScene(num));
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuSelect");
    }
    public void LaunchScene(int num)
    {
        SceneManager.LoadScene(num);
    }

    public void ActivateCredit()
    {
        MenuBase.SetActive(false);
        Crédits.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuSelect");

    }
    public void DesactivateCredit()
    {
        MenuBase.SetActive(true);
        Crédits.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuBack");
        creditImage.color = creditButton.colors.normalColor;
    }
    public void Quitter()
    {
        StartCoroutine(FadeInQuit());
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuSelect");
    }

    IEnumerator FadeOutStart()
    {
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 1);
        
        while (fade.color.a > 0)
        {
            fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, fade.color.a - speedFade* Time.deltaTime);
            yield return null;
        }
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 0);
        
        fade.gameObject.SetActive(false);
        
    }
    IEnumerator FadeInLaunchScene(int number)
    {
        fade.gameObject.SetActive(true);
       
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 0);
        
        while (fade.color.a < 1)
        {
            fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, fade.color.a + speedFade * Time.deltaTime);
            yield return null;
        }
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 1);
        SceneManager.LoadScene(number);
        
    }

    IEnumerator FadeInQuit()
    {
        fade.gameObject.SetActive(true);

        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 0);

        while (fade.color.a < 1)
        {
            fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, fade.color.a + speedFade * Time.deltaTime);
            yield return null;
        }
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 1);
        Application.Quit();

    }



}
