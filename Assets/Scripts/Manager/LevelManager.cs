using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public bool isGameOn;
    public float score;

    public float amount;

    [SerializeField] private Image Fade;
    [SerializeField] private Animator anim;
    [SerializeField] private Text tapToStartText;
    [SerializeField] private Text currencyText;
    [SerializeField] private Text scoreText;

    private void Update()
    {
        if (isGameOn)
        {
            tapToStartText.gameObject.SetActive(false);
            score += (1.0f + amount) * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        currencyText.text = "Currency: " + GameManager.Instance.currency;
        scoreText.text = "Score: " + (int)score;
    }

    public void Respawn()
    {
        StartCoroutine("ReloadScene");
    }

    IEnumerator ReloadScene()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => Fade.color.a == 1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
