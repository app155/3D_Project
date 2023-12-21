using Project3D.GameElements.Skill;
using UnityEngine;

[CreateAssetMenu(fileName = "Teamproject", menuName = "ScriptableObjects/SkillData")]
public class SkillData : ScriptableObject
{
    public int id;
    public string description;
    public float coolDownTime;
    public Skill skill;
}