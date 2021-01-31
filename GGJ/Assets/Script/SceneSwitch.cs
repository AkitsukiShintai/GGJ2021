using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public enum SceneName
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Main
    }

    public SceneName nextScene;

    private void OnTriggerEnter(Collider other)
    {
        switch (nextScene)
        {
            case SceneName.Level1:
                SceneManager.LoadScene("Level 1");
                break;

            case SceneName.Level2:
                SceneManager.LoadScene("Level 2");
                break;

            case SceneName.Level3:
                SceneManager.LoadScene("Level 3");
                break;

            case SceneName.Level4:
                SceneManager.LoadScene("Level 4");
                break;

            case SceneName.Main:
                break;

            default:
                break;
        }
    }

    public void OnClick()
    {
        Player.players = null;
        switch (nextScene)
        {
            case SceneName.Level1:
                SceneManager.LoadScene("Level 1");
                break;

            case SceneName.Level2:
                SceneManager.LoadScene("Level 2");
                break;

            case SceneName.Level3:
                SceneManager.LoadScene("Level 3");
                break;

            case SceneName.Level4:
                SceneManager.LoadScene("Level 4");
                break;

            case SceneName.Main:
                SceneManager.LoadScene("MainScene");

                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player.players = null;
            switch (nextScene)
            {
                case SceneName.Level1:
                    SceneManager.LoadScene("Level 1");
                    break;

                case SceneName.Level2:
                    SceneManager.LoadScene("Level 2");
                    break;

                case SceneName.Level3:
                    SceneManager.LoadScene("Level 3");
                    break;

                case SceneName.Level4:
                    SceneManager.LoadScene("Level 4");
                    break;

                case SceneName.Main:
                    SceneManager.LoadScene("MainScene");

                    break;

                default:
                    break;
            }
        }
    }
}