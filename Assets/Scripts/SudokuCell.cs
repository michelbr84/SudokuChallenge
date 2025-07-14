using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SudokuCell : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Board board;

    int row;
    int col;
    int value;
    string id;
    public Text t;
    [Header("Pencil Marks")]
    public Text pencilText;
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
    private Image bgImage;

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
            t.text = value.ToString();
        }
        else
        {
            t.text = " ";
        }

        pencilMarks.Clear();
        UpdatePencilDisplay();
        SetErrorState(false);

        if (value != 0)
        {
            GetComponentInParent<Button>().enabled = false;
        }
        else
        {
            t.color = new Color32(0, 102, 187, 255);
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
            t.text = value.ToString();
            pencilMarks.Clear();
            UpdatePencilDisplay();
        }
        else
        {
            t.text = "";
        }

        board.UpdatePuzzle(row, col, value);

        if (value != 0)
        {
            StartCoroutine(PlayPopAnimation());
        }
        SetErrorState(false);
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
    }

    private void UpdatePencilDisplay()
    {
        if (pencilText == null) return;
        if (pencilMarks.Count == 0)
        {
            pencilText.text = "";
        }
        else
        {
            pencilText.text = string.Join(" ", pencilMarks);
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
        if (t != null)
            t.color = isError ? errorColor : new Color32(0, 102, 187, 255);
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
}
