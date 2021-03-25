using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;
    public Vector3 m_SpawnPoint;
    public int m_PlayerNumber;             
    public string m_ColoredPlayerText;
    public GameObject m_Instance;          
    public int m_Wins;                     


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;

    public TankManager()
    {
        m_PlayerColor = new Color();
        m_SpawnPoint = Vector3.zero;
        m_ColoredPlayerText = "name";
        m_Instance = null;
        m_Wins = 0;
    }

    public TankManager(Color color, Vector3 spawnPoint, string name, GameObject instance)
    {
        m_PlayerColor = color;
        m_SpawnPoint = spawnPoint;
        m_ColoredPlayerText = name;
        m_Instance = instance;
        m_Wins = 0;
    }

    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
