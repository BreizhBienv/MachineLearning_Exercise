using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BirdPopulation : MonoBehaviour
{
    [Header("Population")]
    [SerializeField] TMP_InputField InputMaxPop;

    [Header("Survivor")]
    [SerializeField] TMP_InputField InputS;
    [SerializeField] Scrollbar      ScrollBarS;

    [Header("Mating")]
    [SerializeField] TMP_InputField InputM;
    [SerializeField] Scrollbar      ScrollBarM;


    int MaxPopulation = 10;
    int TopSurvivorPercent = 10;
    int TopMatingPercent = 50;

    public void ChangeValue_MaxPop_InputField()
    {
        int.TryParse(InputMaxPop.text, out MaxPopulation);
    }

    public void ChangeValue_Survivor_InputField()
    {
        int tmp;
        int.TryParse(InputS.text, out tmp);

        tmp = Mathf.Clamp(tmp, 1, 99);
        InputS.text = tmp.ToString();

        TopSurvivorPercent = tmp;

        float toScrollB = tmp / 100f;
        ScrollBarS.value = toScrollB;
    }

    public void ChangeValue_Survivor_ScrollBar()
    {
        float value = Mathf.Clamp(ScrollBarS.value, 0.01f, 0.99f);
        ScrollBarS.value = value;

        value *= 100f;
        int valueI = (int)value;

        TopSurvivorPercent = valueI;
        InputS.text = valueI.ToString();
    }

    public void ChangeValue_Mating_InputField()
    {
        int tmp;
        int.TryParse(InputM.text, out tmp);

        tmp = Mathf.Clamp(tmp, 1, 99);
        InputM.text = tmp.ToString();

        TopMatingPercent = tmp;

        float toScrollB = tmp / 100f;
        ScrollBarM.value = toScrollB;
    }

    public void ChangeValue_Mating_ScrollBar()
    {
        float value = Mathf.Clamp(ScrollBarM.value, 0.01f, 0.99f);
        ScrollBarM.value = value;

        value *= 100f;
        int valueI = (int)value;

        TopMatingPercent = valueI;
        InputM.text = valueI.ToString();
    }
}
