using UnityEngine;

public class MenuChange : MonoBehaviour
{
    public Transform optionsMenu;
    public Transform pauseMenu;

    public void MoveChildToTop(string childName)
    {
        if (childName == "Options")
            optionsMenu.SetAsLastSibling();
        else if (childName == "Pause")
           pauseMenu.SetAsLastSibling();
    }
}
