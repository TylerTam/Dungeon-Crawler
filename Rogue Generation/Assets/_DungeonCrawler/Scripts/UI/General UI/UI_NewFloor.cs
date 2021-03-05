using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_NewFloor : MonoBehaviour
{
    public static UI_NewFloor Instance;
    public float m_imageFadeTime;
    public float m_textFadeTime;
    public float m_textStayTime;

    public TMPro.TextMeshProUGUI m_txt_dungeonName;
    public TMPro.TextMeshProUGUI m_txt_floorNum;

    public CanvasGroup m_rootCg;

    public CanvasGroup m_textCg;
    private bool m_initialFadeIn = true;

    private void Awake()
    {
        Instance = this;
    }

    public void InitialFadeIn()
    {
        m_rootCg.alpha = 1;
        m_textCg.alpha = 0;
        m_initialFadeIn = true;
    }
    public IEnumerator FadeIn(string p_dungeonName, int p_floorNum)
    {
        m_txt_dungeonName.text = p_dungeonName;
        m_txt_floorNum.text = "Floor " + p_floorNum.ToString();

        m_textCg.alpha = 0;
        float timer = 0;
        if (!m_initialFadeIn)
        {
            while (timer < m_imageFadeTime)
            {
                timer += Time.deltaTime;
                m_rootCg.alpha = timer / m_imageFadeTime;
                yield return null;
            }
        }
        m_initialFadeIn = false;
        m_rootCg.alpha = 1;

        timer = 0;
        while(timer < m_textFadeTime)
        {
            timer += Time.deltaTime;
            m_textCg.alpha = timer / m_textFadeTime;
            yield return null;
        }
        m_textCg.alpha = 1;
    }

    public IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < m_textStayTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while(timer < m_textFadeTime)
        {
            timer += Time.deltaTime;
            m_textCg.alpha = 1 - (timer / m_textFadeTime);
            yield return null;
        }
        m_textCg.alpha = 0;
        timer = 0;

        while(timer < m_imageFadeTime)
        {
            timer += Time.deltaTime;
            m_rootCg.alpha = 1 - (timer / m_imageFadeTime);
            yield return null;
        }
        m_rootCg.alpha = 0;
    }
    
    public void DisableFloorUI()
    {
        gameObject.SetActive(false);
    }
}
