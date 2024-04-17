using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNum : MonoBehaviour
{
    public Color criticalRed = Color.red;
    public Color normalWhite = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.8f);
        transform.localPosition += new Vector3 (0, 1.0f, -1.0f);
 
   
    }
    public void Critical(bool isCritical)
    {
        if (isCritical)
        {
            this.transform.GetChild(0).GetComponent<TextMesh>().color = criticalRed;
            this.transform.GetChild(0).GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
        }
        else
        {
            this.transform.GetChild(0).GetComponent<TextMesh>().color = normalWhite;
            this.transform.GetChild(0).GetComponent<TextMesh>().fontStyle = FontStyle.Normal;
        }
    }
}
