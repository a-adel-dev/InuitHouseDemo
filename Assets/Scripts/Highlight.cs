using UnityEngine;

public class Highlight : MonoBehaviour
{
    private GameObject currentTarget;
    private int actorMask;
    private int highlightMask;
    private GameObject target;

    private void Awake()
    {
        actorMask = LayerMask.NameToLayer("Interactable");
        highlightMask = LayerMask.NameToLayer("Highlight");
        
    }

    void Update()
    {
        RaycastHit info;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, 2.0f,
                LayerMask.GetMask("Interactable", "Highlight")))
        {
            target = info.collider.gameObject;
            Debug.Log("interactable");

            if (currentTarget != target)
            {
                // OnHoverNewObject?.Invoke(target);
                currentTarget = target;
                currentTarget.layer = LayerMask.NameToLayer("Highlight");
            }
        }
        else if (currentTarget != null)
        {
            currentTarget.layer = actorMask;
            // OnExitHover?.Invoke(currentTarget);
            currentTarget = null;
        }
    }
}
