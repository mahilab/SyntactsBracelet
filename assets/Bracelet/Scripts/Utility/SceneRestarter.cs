using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestarter : MonoBehaviour
{
    public KeyCode resartKey = KeyCode.Space;
    public KeyCode exitKey   = KeyCode.Escape;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(resartKey)) {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(exitKey))
            Application.Quit();
    }
}
