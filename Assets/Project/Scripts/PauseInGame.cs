using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseInGame : MonoBehaviour {
    [SerializeField]
    private GameObject sliderAireForce;
    [SerializeField]
    private GameObject parentImageFb;

    [SerializeField]
    private GameObject menuPauseObject;
    [SerializeField]
    private KeyCode keyMenuPause= KeyCode.Escape;
    [SerializeField]
    private UnityStandardAssets.Characters.FirstPerson.FirstPersonController firstPersonController;

    [HideInInspector]
    public bool isActive;

    private PunchHexagon punchHexa;
    private bool iPushedOnQuitButton;

    public Image fade;
    public float speedFade;

    // Use this for initialization
    void Start () {
        punchHexa = FindObjectOfType<PunchHexagon>();
	}
	
	// Update is called once per frame
	void Update () {
        if(isActive == true && iPushedOnQuitButton == false)
        {
            menuPauseObject.SetActive(true);
            //if(punchHexa.punchAireForceActivate == true)
            //sliderAireForce.SetActive(false);
            //parentImageFb.SetActive(false);
            //firstPersonController.m_MouseLook.CursorIsLocked = false;
            //firstPersonController.m_MouseLook.UpdateCursorLock();

            Time.timeScale = 0;
        }
        else if(iPushedOnQuitButton == false)
        {
            menuPauseObject.SetActive(false);
            //if (punchHexa.punchAireForceActivate == true)
            //sliderAireForce.SetActive(true);
            //parentImageFb.SetActive(true);
            //firstPersonController.m_MouseLook.CursorIsLocked = true;
            //firstPersonController.RotateView();
            

            Time.timeScale = 1;
        }
	
        if(Input.GetKeyDown(keyMenuPause))
        {
            ResumeButton();
        }

	}

    public void ResumeButton()
    {
        isActive = !isActive;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuSelect");

    }
    public void ReloadScene(int index)
    {
        SceneManager.LoadScene(index);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuSelect");
    }
    public void Quit()
    {
        Application.Quit();
    }





    public void LaunchPlayScene(int num)
    {
        StartCoroutine(FadeInLaunchScene(num));
        iPushedOnQuitButton = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuSelect");
    }


    public void Quitter()
    {
        StartCoroutine(FadeInQuit());
        iPushedOnQuitButton = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menuSelect");
    }

    IEnumerator FadeOutStart()
    {
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 1);
        Time.timeScale = 1;
        while (fade.color.a > 0)
        {
            fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, fade.color.a - speedFade * Time.deltaTime);
            yield return null;
        }
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 0);

        fade.gameObject.SetActive(false);

    }
    IEnumerator FadeInLaunchScene(int number)
    {
        fade.gameObject.SetActive(true);
        Time.timeScale = 1;
        fade.color = new Vector4(fade.color.r, fade.color.g, fade.color.b, 0);
        Debug.Log(fade.color.a);
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
        Time.timeScale = 1;
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
