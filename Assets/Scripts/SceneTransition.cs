using System;
using UnityEngine;


public class SceneTransition : MonoBehaviour
{
    public void LoadScene()
    {
        // Load the specified scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void Update()
    {
        HandleMouseClick();
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                LoadScene();
            }
        }
    }
}
