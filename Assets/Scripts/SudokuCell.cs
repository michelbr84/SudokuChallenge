using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuCell : MonoBehaviour
{
    Board board;

    int row;
    int col;
    int value;
    string id;
    public Text t;

    private RectTransform rectTransform;

    [Header("Audio Settings")]
    [SerializeField, Tooltip("Audio clip to play when filling a cell.")]
    private AudioClip popSound; // <- SOM de pop
    private AudioSource audioSource; // <- Fonte de áudio

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

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
}
