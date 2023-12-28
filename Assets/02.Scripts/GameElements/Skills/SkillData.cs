using Project3D.GameElements.Skill;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Teamproject", menuName = "ScriptableObjects/SkillData")]
public class SkillData : ScriptableObject
{
    public int id;
    public int coolDownTime;
    public string description;
    public Skill skill;
    public float skillRatio;
    public GameObject Range;
    public Image icon;
}