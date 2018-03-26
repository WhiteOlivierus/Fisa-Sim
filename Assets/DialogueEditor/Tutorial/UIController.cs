using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

class UIController : MonoBehaviour
{
    public DialogueFile dialogueFile = null;

    public string diagloguePart = "";

    public GameObject Dc;
    public GameObject Vc;
    public GameObject Sc;
    public List<GameObject> Bc = new List<GameObject>();
    public List<AudioClip> Ac = new List<AudioClip>();

    private Text DcText;
    private Image VcImage;
    private List<Text> BcButton = new List<Text>();
    private Slider ScSlider;
    private AudioSource AcSource;

    private DialogueManager manager;
    private Dialogue currentDialogue;
    private Dialogue.Choice currentChoice = null;

    private bool timerStart = false;
    private float timer = 0f;
    private int timeToWait = 0;

    public List<Texture2D> images = new List<Texture2D>();

    void Start()
    {
        DcText = Dc.GetComponent<Text>();
        VcImage = Vc.GetComponent<Image>();
        ScSlider = Sc.GetComponent<Slider>();
        AcSource = GetComponent<AudioSource>();

        for (int i = 0; i < Bc.Count; i++)
        {
            BcButton.Add(Bc[i].GetComponentInChildren<Text>());
        }

        manager = DialogueManager.LoadDialogueFile(dialogueFile);
        currentDialogue = manager.GetDialogue(diagloguePart);
        currentChoice = currentDialogue.GetChoices()[0];
        currentDialogue.PickChoice(currentChoice);

        nextDialogue();
    }

    private void Update()
    {
        if (timerStart)
        {
            timer += Time.deltaTime;
            ScSlider.value = (timer % 60f) / timeToWait;
            int seconds = Convert.ToInt32(timer % 60f);

            if (seconds >= timeToWait + 1)
            {
                timerStart = false;
                Sc.SetActive(false);
                timer = 0;
                seconds = 0;
                nextChoice(0);
            }
        }
    }

    void nextDialogue()
    {
        for (int i = 0; i < Bc.Count; i++)
        {
            Bc[i].SetActive(false);
        }

        VcImage.sprite = null;

        DcText.text = currentChoice.dialogue;

        if (currentDialogue.GetChoices().Length > 1)
        {
            Dialogue.Choice[] list = currentDialogue.GetChoices();

            for (int i = 0; i < list.Length; i++)
            {
                Bc[i].SetActive(true);
                Sc.SetActive(false);
                BcButton[i].text = list[i].dialogue;
                print(list[i].dialogue);
            }
        }
        else if (currentDialogue.GetChoices().Length == 1)
        {
            Sc.SetActive(true);
            timerStart = true;
        }

        string [] properties = currentChoice.userData.Split(';');

        foreach(string prop in properties)
        {

            if (prop.Contains("image:"))
            {
                int imageIndex = Int32.Parse(Regex.Match(prop, @"\d+").Value); 
                Texture2D tex = images[imageIndex];
                VcImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            }

            if(prop.Contains("sec:"))
            {
                timeToWait = Int32.Parse(Regex.Match(prop, @"\d+").Value);
            }
            else if(!prop.Contains("audio:") || prop.Contains("image:"))
            {
                timeToWait = 5;
            }

            if (prop.Contains("audio:"))
            {
                AcSource.clip = Ac[Int32.Parse(Regex.Match(prop, @"\d+").Value)];
                AcSource.Play();
            }
            else if (!prop.Contains("sec:") || prop.Contains("image:"))
            {
                AcSource.Stop();
            }
        }

    }

    public void nextChoice(int choice)
    {
        print(choice);
        currentChoice = currentDialogue.GetChoices()[choice];
        currentDialogue.PickChoice(currentChoice);
        nextDialogue();
    }

}