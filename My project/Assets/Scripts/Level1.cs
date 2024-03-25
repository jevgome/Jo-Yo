using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1 : MonoBehaviour
{
    [SerializeField] private GameObject dummy;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(dummy, new Vector2(0,-4), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.R)) {
            ResetScene();
        }
    }

    private void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
