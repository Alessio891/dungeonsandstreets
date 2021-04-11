using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{

    public AudioSource BGMusic;
    public AudioSource UI;

    public AudioClip DayMapMusic;
    public AudioClip NightMapMusic;

    public AudioClip BattleMusic;
    public AudioClip BattleWonMusic;

    public AudioClip OpenMenu;
    public AudioClip MenuPick;
    public AudioClip TabPick;
    public AudioClip ItemSelection;
    public AudioClip Close;

    public AudioClip DiceRoll;
    public AudioClip DiceRollEnd;

    public AudioClip LevelUP;

    public AudioClip SwordSlash;

    public float BGMVolume = 0.8f;

    public static SoundManager instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public void ChangeBGM(AudioClip newClip, bool instant = false)
    {    
        if (!instant)
            StartCoroutine(changeBGM(newClip));
        else
        {
            BGMusic.Stop();
            BGMusic.clip = newClip;
            BGMusic.volume = BGMVolume;
            BGMusic.Play();
        }
    }

    public void PlayUI(AudioClip clip, bool loop = false)
    {
        UI.loop = loop;
        if (loop)
        {
            UI.clip = clip;
            UI.Play();
        } else
            UI.PlayOneShot(clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Map")
            ChangeBGM(DayMapMusic, true);
    }

    IEnumerator changedebug()
    {
        yield return new WaitForSeconds(10.0f);
        ChangeBGM(NightMapMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeBGM(NightMapMusic);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeBGM(DayMapMusic);
        }
    }

    IEnumerator changeBGM(AudioClip newClip)
    {
        while(BGMusic.volume > 0)
        {
            BGMusic.volume -= 0.5f * Time.deltaTime;
            yield return null;
        }
        BGMusic.Stop();
        BGMusic.clip = newClip;
        BGMusic.Play();
        while(BGMusic.volume < BGMVolume)
        {
            BGMusic.volume += 0.7f * Time.deltaTime;
            yield return null;
        }
    }

    public void MenuPickSound()
    {
        PlayUI(MenuPick);
    }
    public void TabPickSound()
    {
        PlayUI(TabPick);
    }
    public void ItemSelectionSound()
    {
        PlayUI(ItemSelection);
    }
    public void CloseSound()
    {
        PlayUI(Close);
    }
}
