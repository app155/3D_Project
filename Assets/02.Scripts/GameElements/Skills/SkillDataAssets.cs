using System.Collections.Generic;
using UnityEngine;

public class SkillDataAssets : MonoBehaviour
{
    public static SkillDataAssets instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(Resources.Load<SkillDataAssets>("SkillDataAssets"));
            }
            return _instance;
        }
    }
    private static SkillDataAssets _instance;

    public SkillData this[int skillID] => skillDatum[skillID];
    public Dictionary<int, SkillData> skillDatum;
    [SerializeField] private List<SkillData> _list;

    private void Awake()
    {
        skillDatum = new Dictionary<int, SkillData>();
        foreach (SkillData data in _list)
        {
            skillDatum.Add(data.id, data);
        }
    }
}