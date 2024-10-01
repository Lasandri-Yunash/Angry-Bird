using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlingShotArea : MonoBehaviour
{

    [SerializeField] private LayerMask _slingShortAreaMask;        // Layer mask for the slingshot area

    public bool IsWithinSlingShotArea()
    {

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);   // Convert mouse position to world coordinates

        if (Physics2D.OverlapPoint(worldPosition, _slingShortAreaMask))         // Check if the mouse position is within the slingshot area
        {

            return true;
        }
         else
        {
            return false;
        }
    }
}
