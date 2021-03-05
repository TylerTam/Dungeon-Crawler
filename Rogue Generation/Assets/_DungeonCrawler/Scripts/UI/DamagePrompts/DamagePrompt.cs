using UnityEngine;

public class DamagePrompt : MonoBehaviour
{

    public TMPro.TextMeshProUGUI m_textPrompt;
    public Color m_damageColor, m_healColor;

    public void SetUi(string p_message, bool p_damage)
    {
        m_textPrompt.color = p_damage ? m_damageColor : m_healColor;
        m_textPrompt.text = p_message;
    }
    public void MessageAnimComplete()
    {
        ObjectPooler.instance.ReturnToPool(gameObject);
    }
}
