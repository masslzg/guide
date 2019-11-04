using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HomeController : MonoBehaviour
{
    /** panel control start**/
    public Animator PasswordPanel;    
    bool isPanelOpen = false;

    public void TooglePanel()
    {
        Debug.Log("isPanelOpen "+ isPanelOpen);
        if (!isPanelOpen)
        {
            PasswordPanel.Play("open_panel");            
        }
        else {
            PasswordPanel.Play("close_panel");           
        }
        StartCoroutine(TooglePanelWait());
    }

    IEnumerator TooglePanelWait ()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isPanelOpen)
        {
            isPanelOpen = true;
        }
        else
        {
            isPanelOpen = false;
        }
    }

    /** panel control end**/

    /** password input control start**/
    public Text PasswordText;
    string password = "";
    public GameObject Home;
    public GameObject ProjectSlide;
    public void InputPassword(string str)
    {
        if (password.Length >= 10) return;
        password += str;
        PasswordText.text = password;
        if (password == "1234")
        {
            Home.SetActive(false);
            ProjectSlide.SetActive(false);
            ClearPassword();
            isPanelOpen = false;
        }
    }

    public void ClearPassword()
    {
        password = "";
        PasswordText.text = password;
    }

    /** password input control end**/

}
