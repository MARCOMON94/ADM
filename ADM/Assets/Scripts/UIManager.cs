using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Button moveButton;
    public Button attackButton;
    public Button passTurnButton;

    public TextMeshProUGUI[] characterHealthTexts;
    private PlayerMove currentPlayerMove;
    public TextMeshProUGUI roundText;

    
    public Image[] characterFrames;

    void Awake()
    {
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
        moveButton.onClick.AddListener(OnMoveButtonClicked);
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        passTurnButton.onClick.AddListener(OnPassTurnButtonClicked);
        HidePlayerControls(); 
    }

    public void SetCurrentPlayerMove(PlayerMove playerMove)
    {
        currentPlayerMove = playerMove;
    }

    public PlayerMove GetCurrentPlayerMove()
    {
        return currentPlayerMove;
    }

    void OnMoveButtonClicked()
    {
        if (currentPlayerMove != null)
        {
            currentPlayerMove.SetActionMove();
            Debug.Log("Current Player Move set to: " + (currentPlayerMove != null ? currentPlayerMove.name : "null"));
        }
    }

    public void TogglePlayerControls(bool enable)
    {
        moveButton.interactable = enable;
        attackButton.interactable = enable;
        passTurnButton.interactable = enable;
    }

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

    void OnPassTurnButtonClicked()
    {
        if (currentPlayerMove != null)
        {
            currentPlayerMove.EndTurn();
        }
    }

    public void UpdateCharacterHealth(int characterIndex, int health)
    {
        if (characterIndex >= 0 && characterIndex < characterHealthTexts.Length)
        {
            characterHealthTexts[characterIndex].text = health.ToString();
        }
    }

    public void ShowPlayerControls()
    {
        moveButton.gameObject.SetActive(true);
        attackButton.gameObject.SetActive(true);
        passTurnButton.gameObject.SetActive(true);
    }

    public void HidePlayerControls()
    {
        moveButton.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
        passTurnButton.gameObject.SetActive(false);
    }

    // Añadimos un método para actualizar los marcos
    public void UpdateTurnFrame(int characterIndex)
    {
        for (int i = 0; i < characterFrames.Length; i++)
        {
            if (i == characterIndex)
            {
                characterFrames[i].gameObject.SetActive(true);
            }
            else
            {
                characterFrames[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateRoundText(int round)
    {
        roundText.text = "" + round;
    }
}
