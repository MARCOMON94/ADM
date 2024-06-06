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

    public Image[] turnIndicators; // Array público para asignar en el Inspector
    private Dictionary<string, int> characterTurnIndexMap = new Dictionary<string, int>();

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

    public void UpdateTurnFrame(string characterName)
    {
        int characterIndex;
        if (characterTurnIndexMap.TryGetValue(characterName, out characterIndex))
        {
            for (int i = 0; i < turnIndicators.Length; i++)
            {
                turnIndicators[i].gameObject.SetActive(i == characterIndex);
            }
        }
    }

    public void DeactivateTurnFrame(string characterName)
    {
        int characterIndex;
        if (characterTurnIndexMap.TryGetValue(characterName, out characterIndex))
        {
            turnIndicators[characterIndex].gameObject.SetActive(false);
        }
    }

    public void RegisterCharacterTurnIndicator(string characterName, int index)
    {
        if (!characterTurnIndexMap.ContainsKey(characterName))
        {
            characterTurnIndexMap.Add(characterName, index);
        }
    }

    public void RemoveTurnIndicator(string characterName)
    {
        if (characterTurnIndexMap.ContainsKey(characterName))
        {
            int index = characterTurnIndexMap[characterName];
            turnIndicators[index].gameObject.SetActive(false);
            characterTurnIndexMap.Remove(characterName);
        }
    }

    public void UpdateRoundText(int round)
    {
        roundText.text = "" + round;
    }
}
