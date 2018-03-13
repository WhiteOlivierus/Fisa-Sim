using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class Tutorial : MonoBehaviour
{
    public DialogueFile dialogueFile = null;

    private Canvas Mc;
    private GameObject Dc;
    private Text DcText;
    private GameObject Vc;
    private Image VcImage;

    private DialogueManager manager;
    private Dialogue currentDialogue;
    private Dialogue.Choice currentChoice = null;

    public List<Texture2D> images = new List<Texture2D>();
    public Font mainFont;



    void Start()
    {
        Mc = GameObject.Find("Canvas").GetComponent<Canvas>();

        Dc = new GameObject();
        Vc = new GameObject();

        Dc.name = "Dialogue";
        Vc.name = "Visuals";

        manager = DialogueManager.LoadDialogueFile(dialogueFile);
        currentDialogue = manager.GetDialogue("TutorialStart");
        currentChoice = currentDialogue.GetChoices()[0];
        currentDialogue.PickChoice(currentChoice);
    }

    void Update()
    {
        if (!Dc.GetComponent<Text>())
        {
            DcText = Dc.AddComponent<Text>();
        }
        
        DcText.text = currentChoice.dialogue;
        DcText.font = mainFont;
        Dc.transform.SetParent(Mc.transform);

        RectTransform DcRectTrans = Dc.GetComponent<RectTransform>();

        DcRectTrans.anchorMin = new Vector2(0, 0);
        DcRectTrans.anchorMax = new Vector2(1, 0);
        DcRectTrans.pivot = new Vector2(0.5f, 0);
        DcRectTrans.sizeDelta = new Vector2(0,250);
        DcRectTrans.position = new Vector2(0, 0);
        DcRectTrans.offsetMin = new Vector2(0, 0);
        DcRectTrans.offsetMax = new Vector2(0, 250);

        if (currentDialogue.GetChoices().Length > 1)
        {
            Dialogue.Choice[] list = currentDialogue.GetChoices();
            System.Array.Sort(list, (o1, o2) => o1.userData.CompareTo(o2.userData));

            foreach (Dialogue.Choice choice in list)
            {
                if (GUILayout.Button(choice.dialogue))
                {
                    currentDialogue.PickChoice(choice);
                    currentChoice = choice;
                }
            }
        }
        else if (currentDialogue.GetChoices().Length == 1)
        {
            if (GUILayout.Button("Next"))
            {
                currentChoice = currentDialogue.GetChoices()[0];
                currentDialogue.PickChoice(currentChoice);
            }

            // check if we need to display an image
            if (currentChoice.userData.IndexOf("image:") == 0)
            {
                Debug.Log(currentChoice.userData.Substring(6));
                int imageIndex = int.Parse(currentChoice.userData.Substring(6));
                Texture2D tex = images[imageIndex];
                //GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex);
                if (!Vc.GetComponent<Image>())
                {
                    VcImage = Dc.AddComponent<Image>();
                }

                VcImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
                Vc.transform.SetParent(Mc.transform);
                Dc.transform.SetParent(Vc.transform);

                RectTransform VcRectTrans = Vc.GetComponent<RectTransform>();

                VcRectTrans.anchorMin = new Vector2(0, 0);
                VcRectTrans.anchorMax = new Vector2(1, 1);
                VcRectTrans.pivot = new Vector2(0.5f, 0.5f);
                //VcRectTrans.sizeDelta = new Vector2(0, 250);
                //VcRectTrans.position = new Vector2(0, 0);
                //VcRectTrans.offsetMin = new Vector2(0, 0);
                //VcRectTrans.offsetMax = new Vector2(0, 250);
            }
        }
        else
        {
            // end of tutorial
        }


    }
}