using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;
    [SerializeField] float normalCharDelay = 0.05f;
    [SerializeField] float acceleratedCharDelay = 0.01f;
    [SerializeField] float timeBtwWords = 0.5f;
    [SerializeField] GameObject arrowIndicator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] typingSounds; // Agora são vários sons!
    [SerializeField] float minPitch = 0.95f;
    [SerializeField] float maxPitch = 1.05f;
    [SerializeField] float minSoundInterval = 0.03f; // intervalo mínimo entre sons

    private float lastSoundTime = 0f;
    public string[] stringArray;
    private int index = -1;
    private bool isTyping = false;
    private bool accelerate = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        arrowIndicator.SetActive(false);
        NextLine();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                accelerate = true;
            }
            else
            {
                arrowIndicator.SetActive(false);
                NextLine();
            }
        }
    }

    void NextLine()
    {
        index++;
        if (index < stringArray.Length)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TextVisible(stringArray[index]));
        }
        else
        {
            Debug.Log("Fim das falas, Mestre Marcelly.");
            arrowIndicator.SetActive(false);
        }
    }

    private IEnumerator TextVisible(string line)
    {
        isTyping = true;
        accelerate = false;
        arrowIndicator.SetActive(false);

        _textMeshPro.text = line;
        _textMeshPro.ForceMeshUpdate();

        int totalVisibleCharacters = _textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (counter <= totalVisibleCharacters)
        {
            _textMeshPro.maxVisibleCharacters = counter;
            counter++;

            if (typingSounds.Length > 0 && audioSource && counter <= totalVisibleCharacters)
            {
                if (Time.time - lastSoundTime >= minSoundInterval)
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
}
