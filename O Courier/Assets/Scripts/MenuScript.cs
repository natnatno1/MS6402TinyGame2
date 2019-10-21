using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Image ButtonSelect1;
    public Image ButtonSelect2;
    public bool Button1;


    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            ButtonSelect1.gameObject.SetActive(true);
            ButtonSelect2.gameObject.SetActive(false);
            Button1 = true;
        }

        else if (Input.GetKeyDown(KeyCode.D))
        {
            ButtonSelect2.gameObject.SetActive(true);
            ButtonSelect1.gameObject.SetActive(false);
            Button1 = false;        }

        if (ButtonSelect1.enabled == true)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                SceneManager.LoadScene(1);
            }
        }

        else if (Button1 == false)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Application.Quit();
            }
        }
    }


}
