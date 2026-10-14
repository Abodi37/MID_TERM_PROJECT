using System.Collections;
using System.Collections.Generic;   
using UnityEngine;

public interface IInteractable
{
    void Interact();
}
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange ;
    public LayerMask InteractableLayer;

    private List<IInteractable> inventory = new List<IInteractable>();
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteracte();
        }
    }

    private void CheckInteracte()
    {
        Collider[] colliders = Physics.OverlapSphere(InteractorSource.position, InteractRange, InteractableLayer);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IInteractable>(out  var interactObj))
            {
                float distance = Vector3.Distance(InteractorSource.position, collider.transform.position);
                if (distance <= InteractRange && !inventory.Contains(interactObj))
                {
                    interactObj.Interact();
                    inventory.Add(interactObj);
                }
            }
        }
    }

}
