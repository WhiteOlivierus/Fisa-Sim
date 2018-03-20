using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class UIController : MonoBehaviour
{
    public DialogueFile dialogueFile = null;

    public string diagloguePart;

    public GameObject Dc;
    public GameObject Vc;
    public List<GameObject> Bc = new List<GameObject>();

    private Text DcText;
    private Image VcImage;
    private List<Text> BcButton = new List<Text>();

    private DialogueManager manager;
    private Dialogue currentDialogue;
    private Dialogue.Choice currentChoice = null;

    public List<Texture2D> images = new List<Texture2D>();

    void Start()
    {
        DcText = Dc.GetComponent<Text>();
        VcImage = Vc.GetComponent<Image>();

        for(int i = 0; i < Bc.Count; i++)
        {
            BcButton.Add(Bc[i].GetComponentInChildren<Text>());
        }

        manager = DialogueManager.LoadDialogueFile(dialogueFile);
        currentDialogue = manager.GetDialogue(diagloguePart);
        currentChoice = currentDialogue.GetChoices()[0];
        currentDialogue.PickChoice(currentChoice);

        nextDialogue();
    }

    void nextDialogue()
    {
        for(int i = 0; i < Bc.Count; i++)
        {
            Bc[i].SetActive(false);
        }

        VcImage.sprite = null;

        DcText.text = currentChoice.dialogue;

        if (currentDialogue.GetChoices().Length > 1)
        {
            Dialogue.Choice[] list = currentDialogue.GetChoices();

            for(int i = 0; i < list.Length; i++)
            {
                Bc[i].SetActive(true);
                BcButton[i].text = list[i].dialogue;
                print(list[i].dialogue);
            }
        }
        else if (currentDialogue.GetChoices().Length == 1)
        {
            Bc[4].SetActive(true);
            BcButton[4].text = "Next";
        }

        if (currentChoice.userData.IndexOf("image:") == 0)
        {
            int imageIndex = int.Parse(currentChoice.userData.Substring(6));
            Texture2D tex = images[imageIndex];
            VcImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
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