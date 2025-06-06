using System.Collections;
using UnityEngine.UI; // importante
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private GameObject arrowIndicator;
    [SerializeField] private GameObject dialogWindow;

    [Header("Portraits")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite defaultPortrait;
    [SerializeField] private Sprite blinkingPortrait;
    [SerializeField] private Sprite happyPortrait;
    [SerializeField] private Sprite preocupiedPortrait;
    [SerializeField] private Sprite shockedPortrait;


    [Header("Typewriter Settings")]
    [SerializeField] private float normalCharDelay = 0.05f;
    [SerializeField] private float acceleratedCharDelay = 0.01f;
    [SerializeField] private float timeBtwWords = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] typingSounds;
    [SerializeField] private float minPitch = 0.95f;
    [SerializeField] private float maxPitch = 1.05f;
    [SerializeField] private float minSoundInterval = 0.03f;

    private DialogData currentDialog;
    private int index = -1;
    private bool isTyping = false;
    private bool accelerate = false;
    private Coroutine typingCoroutine;
    private float lastSoundTime = 0f;

    void Start()
    {
        dialogWindow.SetActive(false);
        arrowIndicator.SetActive(false);
    }

    void Update()
    {
        if (!dialogWindow.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
                accelerate = true;
            else
            {
                arrowIndicator.SetActive(false);
                NextLine();
            }
        }
    }

    private Sprite GetPortraitForName(string name)
    {
        switch (name.ToLower())
        {
            case "blinking":
                return blinkingPortrait;
            case "shocked":
                return shockedPortrait;
            case "happy":
                return happyPortrait;
            case "preoccupied":
                return preocupiedPortrait;
            case "default":
            default:
                return defaultPortrait;
        }
    }

    public void PlayDialog(DialogData dialog)
    {
        if (dialog == null) return;

        currentDialog = dialog;
        index = -1;
        dialogWindow.SetActive(true);
        arrowIndicator.SetActive(false);

        portraitImage.sprite = GetPortraitForName(dialog.spriteName);

        NextLine();
    }

    private void NextLine()
    {
        index++;
        if (currentDialog != null && index < currentDialog.lines.Length)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TextVisible(currentDialog.lines[index]));
        }
        else
        {
            EndDialog();
        }
    }

    private IEnumerator TextVisible(string line)
    {
        isTyping = true;
        accelerate = false;
        arrowIndicator.SetActive(false);

        textMesh.text = line;
        textMesh.ForceMeshUpdate();

        int totalVisibleCharacters = textMesh.textInfo.characterCount;
        int counter = 0;

        while (counter <= totalVisibleCharacters)
        {
            textMesh.maxVisibleCharacters = counter;
            counter++;

            if (typingSounds.Length > 0 && audioSource && counter <= totalVisibleCharacters)
            {
                char currentChar = textMesh.text[Mathf.Clamp(counter - 1, 0, textMesh.text.Length - 1)];
                if (char.IsLetterOrDigit(currentChar) && (Time.time - lastSoundTime >= minSoundInterval))
                {
                    AudioClip clip = typingSounds[Random.Range(0, typingSounds.Length)];
                    audioSource.pitch = Random.Range(minPitch, maxPitch);
                    audioSource.PlayOneShot(clip);
                    lastSoundTime = Time.time;
                }
            }

            float delay = accelerate ? acceleratedCharDelay : normalCharDelay;
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(timeBtwWords);
        isTyping = false;
        arrowIndicator.SetActive(true);
    }

    private void EndDialog()
    {
        dialogWindow.SetActive(false);
        arrowIndicator.SetActive(false);
        currentDialog = null;
    }

    public bool IsDialogActive()
    {
        return dialogWindow.activeSelf;
    }
}
