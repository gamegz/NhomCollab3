using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariousEffectsScene : MonoBehaviour
{

    public Transform[] m_effects;
    public GameObject scaleform;
    public GameObject[] m_destroyObjects = new GameObject[30];
    public GameObject FriendlyEnemyObject;

    private GameObject gm;

    public int inputLocation;
    public Text m_scalefactor;
    public static float m_graph_scenesizefactor = 1;
    public Text m_effectName;
    int index;


    private void Awake()
    {
        inputLocation = 0;
        m_effectName.text = m_effects[index].name.ToString();
        MakeObject();
    }

    // Update is called once per frame
    void Update()
    {
        InputKey();
        if (index < 70)
            FriendlyEnemyObject.SetActive(false);
        else
            FriendlyEnemyObject.SetActive(true);
    }

    #region Funtion
    private void InputKey()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if (index <= 0)
                index = m_effects.Length - 1;
            else
                index--;

            MakeObject();
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            if (index >= m_effects.Length - 1)
                index = 0;
            else
                index++;

            MakeObject();
        }

        if (Input.GetKeyDown(KeyCode.C))
            MakeObject();
    }

    private void MakeObject()
    {
        DestroyGameObject();
        gm = Instantiate(m_effects[index], m_effects[index].transform.position, m_effects[index].transform.rotation).gameObject;
        m_effectName.text = (index + 1) + " : " + m_effects[index].name.ToString();
        scaleform.transform.position = gm.transform.position;
        gm.transform.parent = scaleform.transform;
        gm.transform.localScale = new Vector3(1, 1, 1);
        float submit_scalefactor = m_graph_scenesizefactor;
        if (index < 70)
            submit_scalefactor *= 0.5f;
        gm.transform.localScale = new Vector3(submit_scalefactor, submit_scalefactor, submit_scalefactor);
        m_destroyObjects[inputLocation] = gm;
        inputLocation++;
    }

    private void DestroyGameObject()
    {
        for(int i = 0; i < inputLocation; i++)
        {
            Destroy(m_destroyObjects[i]);
        }
        inputLocation = 0;
    }

    public void GetSizeFactory()
    {
        m_graph_scenesizefactor = float.Parse(m_scalefactor.text.ToString());
        float submit_scalefactor = m_graph_scenesizefactor;
        if (index < 70)
            submit_scalefactor *= 0.5f;
        gm.transform.localScale = new Vector3(submit_scalefactor, submit_scalefactor, submit_scalefactor);
    }
    #endregion
}
