using UnityEngine;

public class SetRandomColor : MonoBehaviour
{
    private Color m_color;

    public Color[] colors;

    public Color Color
    {
        get
        {
            return m_color;
        }

        set
        {
            m_color = value;
        }
    }

    // Initializes things
    void Awake () 
	{
	    if (colors.Length <= 0 || colors == null)
	    {
	       m_color = new Color(Random.value, Random.value, Random.value);
	    }
	    else
	    {
	       m_color = colors[Random.Range(0, colors.Length - 1)];
	    }

	    if (GetComponent<Renderer>() != null)
	    {
	        GetComponent<Renderer>().material.color = m_color;
	    }
	}
}
