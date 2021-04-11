using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginCanvas : MonoBehaviour {

	public InputField user;
	public InputField pass;

	public Text ResponseText;

	bool doneCommunicating = true;
	public CanvasGroup MaskCanvasGroup;

    public Image background;

    public AudioClip TitleBGM;

    public CanvasGroup Form;
    public float FormShowSpeed = 0.3f;

	// Use this for initialization
	void Start () {
		//MaskCanvasGroup.gameObject.SetActive (false);	
		if (PlayerPrefs.HasKey ("lastUser")) {
			user.text = PlayerPrefs.GetString ("lastUser");
			pass.text = PlayerPrefs.GetString ("lastPass");
		}

        iTween.MoveBy(background.gameObject, new Vector3(50.0f, 0, 0), 250.0f);
        SoundManager.instance.ChangeBGM(TitleBGM, true);
        StartCoroutine(showForm());
	}

    IEnumerator showForm()
    {
        while(Form.alpha < 1.0f)
        {
            Form.alpha += FormShowSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
	
	// Update is called once per frame
	void Update () {		

	}

	public Dictionary<string, object> getValues()
	{
		Dictionary<string, object> retVal = new Dictionary<string, object> ();
		retVal.Add ("userName", user.text);
		retVal.Add ("password", pass.text);
		retVal.Add ("version", Registry.assets.VersionInfo["version"]);
		return retVal;
	}
	IEnumerator LoginRoutine(string user)
	{
		ResponseText.text = "Logging in as " + user + "...";
		yield return new WaitForSeconds (1.5f);
		PlayerPrefs.SetString ("user", user);
		SceneManager.LoadScene (1);
	}

	public void Login() {

		if (string.IsNullOrEmpty (user.text) || string.IsNullOrEmpty (pass.text)) {
			PopupManager.ShowPopup ("Error!", "Username and password must be entered before attempting a login.", (t, b) => b.Close());
			return;
		}

		LoadingPopup popup = PopupManager.ShowPopup<LoadingPopup> ("LoadingPopup", "Connection", "Connecting to server...",
			(s, p) => {});

		doneCommunicating = false;
		Bridge.POST (Bridge.url + "Auth", getValues (), (s) => {
			Debug.Log(s);
			ServerResponse resp = new ServerResponse(s);
			if (resp.status == ServerResponse.ResultType.Success)
			{
				string scene = (resp.message == "Welcome") ? "CharCreation" : "Map";
				PlayerPrefs.SetString("lastUser", user.text);
				PlayerPrefs.SetString("lastPass", pass.text);
			//	MaskCanvasGroup.gameObject.SetActive(true);
				MaskCanvasGroup.alpha = 0.55f;
				PlayerPrefs.SetString("user", user.text);
				LoadingPopup loggingInPopup = PopupManager.ShowPopup<LoadingPopup>("LoadingPopup", "Success!", 
					"Logging in as " + user.text, (t, b) => { }, () => SceneManager.LoadScene(scene));
				loggingInPopup.UseMask = false;
				MaskCanvasGroup.transform.SetAsLastSibling();
				loggingInPopup.StartLoading( () => {
					if (MaskCanvasGroup.alpha >= 1.0f)
						return true;
					MaskCanvasGroup.alpha += 0.5f * Time.deltaTime;
					return false;
				});
			} else
			{
				Debug.Log("Errorcode: " + resp.errorCode);
				string popupText = "";
				if (resp.errorCode == "501")
				{
					popupText = "Username or password incorrect. Did you forget your password?";
				} 
				else if (resp.errorCode == "503")
				{
					popupText = "A new version is required in order to login. You can get version " + resp.GetIncomingDictionary()["version"].ToString() + " from " + resp.GetIncomingDictionary()["downloadLink"].ToString() + ".";
				}
				else
				{
					popupText = "Communication with server failed. Check your internet connection and retry.";
				}
				PopupManager.ShowPopup("Error :(", popupText, (t, b) => b.Close());
			}
			doneCommunicating = true;
		});


		popup.StartLoading (() => doneCommunicating);


	}
	public void Register() {
		if (string.IsNullOrEmpty (user.text) || string.IsNullOrEmpty (pass.text)) {
			PopupManager.ShowPopup ("Error!", "Username and password must be entered before registering.", (t, b) => b.Close());
			return;
		}

		LoadingPopup popup = PopupManager.ShowPopup<LoadingPopup> ("LoadingPopup", "Connection", "Connecting to server...",
			(s, p) => {});

		doneCommunicating = false;
		Bridge.POST (Bridge.url + "Register", getValues (), (s) => {
			ServerResponse resp = new ServerResponse(s);
			if (resp.status == ServerResponse.ResultType.Success)
			{				
				PopupManager.ShowPopup("Success!", "Your account has been created succesfully!", (t, b) => { b.Close(); });
			} else
			{
				string popupText = "";
				if (resp.errorCode == "502")
				{
					popupText = "That username is already taken!";
				} else
				{
					popupText = "Communication with server failed. Check your internet connection and retry.";
				}
				PopupManager.ShowPopup("Error :(", popupText, (t, b) => b.Close());
			}
			doneCommunicating = true;
		});


		popup.StartLoading (() => doneCommunicating);
	}
}
