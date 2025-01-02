using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public NodeManager nodeManager;
    public Button showHeuristicButton;
    private bool isShowing = false;

    void Start()
    {
        showHeuristicButton.onClick.AddListener(ToggleHeuristicDisplay);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleHeuristicDisplay();
        }
    }

    // Method to toggle heuristic display
    public void ToggleHeuristicDisplay()
    {
        if (isShowing)
            nodeManager.HideFScoreOnNodes();
        else
            nodeManager.ShowFScoreOnNodes();
        isShowing = !isShowing;
    }
}
