using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SudokuCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Board board;

    int row;
    int col;
    int value;
    string id;
    public Text t;
    [SerializeField] private TMP_Text tTMP;
    [Header("Pencil Marks")]
    public Text[] pencilTexts = new Text[9]; // Assign in Inspector: Pencil1 to Pencil9
    [SerializeField] private TMP_Text[] pencilTMPTexts = new TMP_Text[9];
    [SerializeField] private GameObject pencilContainer; // Parent object that holds the 3x3 pencil grid
    private List<int> pencilMarks = new List<int>();

    private RectTransform rectTransform;

    [Header("Audio Settings")]
    [SerializeField, Tooltip("Audio clip to play when filling a cell.")]
    private AudioClip popSound; // <- SOM de pop
    private AudioSource audioSource; // <- Fonte de áudio

    [Header("UI Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.85f, 0.92f, 1f);
    [SerializeField] private Color clickColor = new Color(0.7f, 0.8f, 1f);
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private Color entryColor = new Color32(0, 102, 187, 255);
    private Image bgImage;

    [Header("Error Animation")]
    [SerializeField] private bool enableErrorShake = true;
    [SerializeField, Range(0.05f, 0.5f)] private float shakeDuration = 0.15f;
    [SerializeField, Range(1f, 20f)] private float shakeMagnitude = 6f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        bgImage = GetComponent<Image>();
        if (bgImage != null) bgImage.color = normalColor;

        // Adiciona automaticamente o AudioSource se não tiver
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void SetValues(int _row, int _col, int value, string _id, Board _board)
    {
        row = _row;
        col = _col;
        id = _id;
        board = _board;

        if (value != 0)
        {
            SetMainText(value.ToString());
        }
        else
        {
            SetMainText(" ");
        }

        pencilMarks.Clear();
        UpdatePencilDisplay();
        SetErrorState(false);
        UpdatePencilVisibility();

        if (value != 0)
        {
            GetComponentInParent<Button>().enabled = false;
        }
        else
        {
            SetMainColor(entryColor);
        }
    }

    public void ButtonClicked()
    {
        InputButton.instance.ActivateInputButton(this);
    }

    public void UpdateValue(int newValue)
    {
        value = newValue;

        if (value != 0)
        {
            SetMainText(value.ToString());
            pencilMarks.Clear();
            UpdatePencilDisplay();
        }
        else
        {
            SetMainText("");
        }

        board.UpdatePuzzle(row, col, value);

        if (value != 0)
        {
            StartCoroutine(PlayPopAnimation());
        }
        SetErrorState(false);
        UpdatePencilVisibility();
    }

    public void AddOrRemovePencilMark(int mark)
    {
        if (mark < 1 || mark > 9) return;
        if (pencilMarks.Contains(mark))
            pencilMarks.Remove(mark);
        else
            pencilMarks.Add(mark);
        pencilMarks.Sort();
        UpdatePencilDisplay();
        SetErrorState(false);
    }

    public void ClearPencilMarks()
    {
        pencilMarks.Clear();
        UpdatePencilDisplay();
        SetErrorState(false);
    }

    public void ClearCell()
    {
        // Only allow clearing if the cell was not a prefilled puzzle value (button enabled indicates editable)
        Button parentButton = GetComponentInParent<Button>();
        if (parentButton != null && parentButton.enabled)
        {
            UpdateValue(0);
        }
    }

    private void UpdatePencilDisplay()
    {
        for (int i = 0; i < 9; i++)
        {
            string s = pencilMarks.Contains(i + 1) ? (i + 1).ToString() : "";
            if (pencilTexts != null && pencilTexts.Length == 9 && pencilTexts[i] != null)
            {
                pencilTexts[i].text = s;
            }
            if (pencilTMPTexts != null && pencilTMPTexts.Length == 9 && pencilTMPTexts[i] != null)
            {
                pencilTMPTexts[i].text = s;
            }
        }
    }

    /// <summary>
    /// Coroutine para criar o efeito de "pop" com som e tempo variado.
    /// </summary>
    private IEnumerator PlayPopAnimation()
    {
        float duration = Random.Range(0.09f, 0.12f); // <- Variação natural
        Vector3 originalScale = Vector3.one;
        Vector3 popScale = originalScale * 1.2f;

        // Toca o som de pop se existir
        if (popSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(popSound, 0.7f); // Volume 70%
        }

        // Escala para cima
        float elapsed = 0f;
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, popScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = popScale;

        // Escala para baixo
        elapsed = 0f;
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(popScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = originalScale;
    }

    public void SetErrorState(bool isError)
    {
        SetMainColor(isError ? errorColor : entryColor);
        if (isError && enableErrorShake)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeCoroutine());
        }
    }

    // UI Feedback
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (bgImage != null) bgImage.color = hoverColor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (bgImage != null) bgImage.color = normalColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (bgImage != null) bgImage.color = clickColor;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (bgImage != null) bgImage.color = hoverColor;
    }

    // Helpers to support both Text and TMP_Text
    private void SetMainText(string s)
    {
        if (t != null) t.text = s;
        if (tTMP != null) tTMP.text = s;
    }

    private void SetMainColor(Color c)
    {
        if (t != null) t.color = c;
        if (tTMP != null) tTMP.color = c;
    }

    private void UpdatePencilVisibility()
    {
        if (pencilContainer == null) return;
        // Hide notes when a main value is set; show when empty
        bool hasMain = value != 0;
        pencilContainer.SetActive(!hasMain);
    }

    private System.Collections.IEnumerator ShakeCoroutine()
    {
        Vector3 originalPos = rectTransform.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * (shakeMagnitude * 0.5f);
            rectTransform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = originalPos;
    }
}
