using Project3D.Controller;
using Project3D.GameElements.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// character con �� stat�̶� level���� �������� �����ֱ�
namespace Data 
{

    public class Player_stat : MonoBehaviour
    {
        [SerializeField] protected int _exp;


        public int Exp
        {
            get { return _exp; }
            set
            {
                _exp = value;
                // ������ üũ
                int level = _level;
                while (true)
                {
                    Data.Stat stat;
                    if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false) // �����̸� 
                        break;
                    if (_exp < stat.totalExp) // _exp�� ���� ���������� totalExp ���� ������ ���� ���� ���� �ʿ� �����Ƿ� stop
                        break;
                    level++;
                }

                if (level != _level)
                {
                    Debug.Log("Level Up!");
                    _level = level;
                    SetStat(level);
                }
            }
        }
        public void SetStat(int level)
        {
            Dictionary<int, Data.Stat> dict = Project3D.Controller.CharacterController.Data.StatDict;
            Data.Stat stat = dict[level]; // level�� Key�� ����ϱ�� ����߾���. �������� ������ �޶����ϱ�

            _hp = stat.maxHp;
            _maxHp = stat.maxHp;
            _attack = stat.attack;
        }


    }
}
