using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Instancia única del UIManager
    public static UIManager Instance;

    // Referencias a los botones del UI
    public Button moveButton;
    public Button attackButton;
    public Button passTurnButton;

    // Referencias a los textos de salud de los personajes
    public TextMeshProUGUI[] characterHealthTexts;

    // Referencia al movimiento del jugador actual
    private PlayerMove currentPlayerMove;

    void Awake()
    {
        // Implementación del patrón Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Añade listeners a los botones para que llamen a los métodos correspondientes al hacer clic
        moveButton.onClick.AddListener(OnMoveButtonClicked);
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        passTurnButton.onClick.AddListener(OnPassTurnButtonClicked);
        TogglePlayerControls(false); // Desactiva los controles del jugador al inicio
    }

    // Establece el movimiento del jugador actual
    public void SetCurrentPlayerMove(PlayerMove playerMove)
    {
        currentPlayerMove = playerMove;
    }

    // Método llamado al hacer clic en el botón de mover
    void OnMoveButtonClicked()
    {
        if (currentPlayerMove != null)
        {
            currentPlayerMove.SetActionMove();
            Debug.Log("Current Player Move set to: " + (currentPlayerMove != null ? currentPlayerMove.name : "null"));
        }
    }

    // Activa o desactiva los controles del jugador
    public void TogglePlayerControls(bool enable)
    {
        moveButton.interactable = enable;
        attackButton.interactable = enable;
        passTurnButton.interactable = enable;
    }

    // Método llamado al hacer clic en el botón de atacar
    void OnAttackButtonClicked()
    {
        if (currentPlayerMove != null)
        {
            Debug.Log("Attack button clicked, currentPlayerMove: " + currentPlayerMove.name);
            currentPlayerMove.SetActionAttack();
        }
        else
        {
            Debug.Log("Attack button clicked, but currentPlayerMove is null");
        }
    }

    // Método llamado al hacer clic en el botón de pasar turno
    void OnPassTurnButtonClicked()
    {
        if (currentPlayerMove != null)
        {
            currentPlayerMove.EndTurn();
        }
    }

    // Método para actualizar la salud de un personaje
    public void UpdateCharacterHealth(int characterIndex, int health)
    {
        if (characterIndex >= 0 && characterIndex < characterHealthTexts.Length)
        {
            characterHealthTexts[characterIndex].text = health.ToString();
        }
    }


}
