using Project3D.GameElements.Skill;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Teamproject", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public int id;
    public Image profile;
    public int skill1;
    public int skill2;
}
