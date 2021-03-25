using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;
    public Text m_MessageText;
    public List<TankManager> m_Tanks;
    public TankManager manager;


    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;  
    [SyncVar]
    private TankManager m_RoundWinner;
    [SyncVar]
    private TankManager m_GameWinner;       


    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        StartGame();
    }

    [Command(requiresAuthority = false)]
    public void AddNewPlayer(TankManager newPlayer)
    {
        m_Tanks.Add(newPlayer);
        m_Tanks[m_Tanks.Count - 1].m_PlayerNumber = m_Tanks.Count;
        m_Tanks[m_Tanks.Count - 1].Setup();
        SharePlayer(m_Tanks);
    }

    [Server]
    public void StartGame()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            EndGame();
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }

    [ClientRpc]
    private void EndGame()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_RoundNumber++;
        RefreshText("ROUND " + m_RoundNumber);

        while (!MoreThanOneTankLeft())
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();
        RefreshText(string.Empty);

        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        RefreshText(message);

        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }
    private bool MoreThanOneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft > 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    [ClientRpc]
    private void RefreshText(string message)
    {
        m_MessageText.text = message;
    }

    [ClientRpc]
    private void SharePlayer(List<TankManager> playerList)
    {
        m_Tanks = playerList;
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].Setup();
        }
    }

    [ClientRpc]
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].Reset();
        }
    }

    [ClientRpc]
    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }

    [ClientRpc]
    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
    

}