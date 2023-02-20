using StateManager;
using UnityEngine;
namespace KI_Project
{
    public class MouseLockScript : MonoBehaviour
    {
        bool cursorVisible = true;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (cursorVisible)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    cursorVisible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    cursorVisible = true;
                }
            }
        }
        
    }
}