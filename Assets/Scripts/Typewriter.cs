using System.Collections;
using UnityEngine;

/// <summary>
/// Prints letters out one by one. Requires text to be already written out
/// in the TMPro.TextMeshProUGUI otherwise, it'll just print empty characters.
/// 
/// By Default we require an AudioSource and TMPro.TextMeshProUGUI.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class Typewriter : MonoBehaviour
{
    #region Private Variables

    private const string DEFAULT_TEXT = "TEST";

    private string m_wordToPrint = DEFAULT_TEXT;
    private char[] m_letters;
    private IEnumerator m_coroutine;

    private TMPro.TextMeshProUGUI m_textType;

    private AudioSource m_audioSource;

    #endregion

    #region Public Variables

    public float printSpeed = 0.1f;
    public float timeToLive = .5f;

    #endregion

    #region Unity Methods

    // Triggered when this component is enabled
    private void OnEnable()
    {
        // Only want to assign this once if it is null.
        if (m_audioSource == null)
        {
            // AudioSource is a required component, so it should come with this script automatically.
            m_audioSource = GetComponent<AudioSource>();
        }

        m_coroutine = LoopThroughLetters();
        StartCoroutine(m_coroutine);
    }

    // Triggered when this component is disabled
    private void OnDisable()
    {
        m_textType.text = string.Empty;
        StopCoroutine(m_coroutine);
        m_coroutine = null;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns a char array if a string can be found.
    /// </summary>
    /// <returns>An array of characters.</returns>
    private char[] GetCharArray()
    {
        // Make sure there is a TextMeshPro component
        m_textType = GetComponent<TMPro.TextMeshProUGUI>();
        if (m_textType != null)
        {
            // Don't want to override the word to print unless it is the default string
            if (m_wordToPrint == DEFAULT_TEXT)
            {
                // Sets the word to print to whatever is in the text component
                m_wordToPrint = m_textType.text;
                m_textType.text = "";
            }
            //returns each letter in the string as an array of characters
            return m_letters = m_wordToPrint.ToCharArray();
        }

        // Null if none of the above works
        return null;
    }

    /// <summary>
    /// Loops through the character array and prints out each letter
    /// in the full word based on the time you give it.
    /// </summary>
    /// <returns>IEnumerator (typically "yield return null" or something similar.</returns>
    private IEnumerator LoopThroughLetters()
    {
        // Stores the character array
        m_letters = GetCharArray();

        // If for some reason it is null then don't bother continuing the Coroutine. Exit here.
        if (m_letters == null) yield break;

        // Create a starting index for looping
        int i = 0;

        string appendedLetters = string.Empty;

        // While the index is lower than the amount of letters in the character array
        // keep looping. Otherwise, if it exceeds the length of the character array
        // then break out of the while loop.
        while (i < m_letters.Length)
        {
            if(Input.GetMouseButtonDown(0) == true)
            {
                i = 0;
                m_textType.text = m_wordToPrint;
                yield break;
            }

            // Wait for the number of seconds specified in the inspector
            yield return new WaitForSeconds(printSpeed);

            if (Input.GetMouseButtonDown(0) == true)
            {
                i = 0;
                m_textType.text = m_wordToPrint;
                yield break;
            }

            // Start building a new string one letter at a time.
            appendedLetters += m_letters[i];

            m_audioSource.Play();

            // set the text component's text value equal to the letter-by-letter string
            // being built over time
            m_textType.text = appendedLetters;

            // Increase the index
            i++;
        }

        if(timeToLive < 0)
        {
            yield break;
        }

        if (timeToLive >= 0)
        {
            yield return new WaitForSeconds(timeToLive);

            this.gameObject.SetActive(false);
        }
    }

    #endregion
}