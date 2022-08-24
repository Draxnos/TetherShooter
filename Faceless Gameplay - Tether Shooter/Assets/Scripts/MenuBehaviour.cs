using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{
    public KeyCode pause = KeyCode.Escape;
    public bool toggleSprint = false;
    public bool toggleCrouch = false;

    public List<string> keyNames = new List<string>() { "Forward", "Backward", "Left", "Right", "Sprint", "Crouch", "Jump", "SendTether", "ReelTether", "UnreelTether", "Pause" , "PrimaryAttack", "Reload"};
    public List<KeyCode> keys = new List<KeyCode>() { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.Space, KeyCode.E, KeyCode.C, KeyCode.V, KeyCode.Escape, KeyCode.Mouse0, KeyCode.R};
    public List<KeyCode> defControls;
    public List<Text> labels;

    public string sens = "Sensitivity";
    public Slider sensitivitySlider;
    public Text sensObj;
    public float sensitivity;

    public GameObject[] setKeys;

    public GameObject pauseMenu;

    public GameObject optionsMenu;

    public GameObject visualsMenu;

    public GameObject controlsMenu;
    public Event keyEvent;

    public int menu = -1;

    public bool settingKeys = false;

    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        defControls = keys;
        KeyCheck();
        SensitivityChange();
    }

    // Update is called once per frame
    void Update()
    {
        if (!settingKeys)
        {
            if (Input.GetKeyDown(pause))
            {
                Back();
            }
        }
    }

    void KeyCheck()
    {
        for (int i = 0; i < keyNames.Count; i++)
        {
            int key = PlayerPrefs.GetInt(keyNames[i], -1);

            if (key != -1)
            {
                keys[i] = (KeyCode)key;
                labels[i].text = keys[i].ToString();
            }
        }

        float se = PlayerPrefs.GetFloat(sens);

        if (se > 0)
        {
            sensitivity = se;
            sensObj.text = sensitivity.ToString();
        }
    }

    public void Pause()
    {
        if (menu != 0)
        {
            paused = true;
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            menu = 0;
        }
        else if (menu == 0)
        {
            paused = false;
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;

            menu = -1;
        }

        if (paused == false)
        {
            optionsMenu.SetActive(false);
            visualsMenu.SetActive(false);
            controlsMenu.SetActive(false);
        }
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
        menu = 1;
    }

    public void Controls()
    {
        controlsMenu.SetActive(true);
        optionsMenu.SetActive(false);
        menu = 2;
    }

    public void SetKeyStart(GameObject obj)
    {
        StartCoroutine(SetKey(obj.name, obj.transform.Find("Key").gameObject.GetComponent<Text>()));
    }

    private void OnGUI()
    {
        keyEvent = Event.current;
    }

    IEnumerator SetKey(string keytype, Text txt)
    {

        int mouse = 0;
        bool mouseUp = false;

        while (mouse < 2 && keyEvent.keyCode == KeyCode.None)
        {

            if (keyEvent.type == EventType.MouseDown || keyEvent.type == EventType.MouseDrag)
            {

                if (mouseUp)
                {
                    mouse++;
                }
                else if (mouse == 1)
                {
                    mouse = 1;
                }
                else
                {
                    mouse = 0;
                }

                mouseUp = false;

            }


            else if (keyEvent.type == EventType.MouseUp)
            {
                mouse = 1;
                mouseUp = true;

            }

            yield return null;

        }

        int index = keyNames.IndexOf(keytype);

        if (keyEvent.keyCode != KeyCode.None && !keyEvent.isMouse)
        {
            keys[index] = keyEvent.keyCode;
            txt.text = keys[index].ToString();
        }
        else
        {
            keys[index] = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Mouse" + keyEvent.button);
            txt.text = keys[index].ToString();
        }
    }

    public void SaveControls()
    {
        for (int i = 0; i < keyNames.Count; i++)
        {
            PlayerPrefs.SetInt(keyNames[i], (int)keys[i]);
            PlayerPrefs.SetFloat(sens, sensitivity);
        }
    }

    public void Visuals()
    {
        visualsMenu.SetActive(true);
        optionsMenu.SetActive(false);
        menu = 3;
    }

    public void Back()
    {
        switch (menu)
        {
            case 0:
            case 1:
                optionsMenu.SetActive(false);
                Pause();
                break;

            case 2:
            case 3:
                controlsMenu.SetActive(false);
                visualsMenu.SetActive(false);
                Options();
                break;

            default:
                Pause();
                break;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SensitivityChange()
    {
        sensitivity = sensitivitySlider.value;
        sensObj.text = sensitivity.ToString();
    }
}
