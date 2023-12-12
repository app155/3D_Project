using Project3D.Controller;
using Project3D.GameElements.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// character con 에 stat이랑 level개념 만들어오고 참조주기
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
                // 레벨업 체크
                int level = _level;
                while (true)
                {
                    Data.Stat stat;
                    if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false) // 만렙이면 
                        break;
                    if (_exp < stat.totalExp) // _exp가 현재 레벨에서의 totalExp 보다 적으면 이제 레벨 증가 필요 없으므로 stop
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
            Data.Stat stat = dict[level]; // level을 Key로 사용하기로 약속했었다. 레벨마다 스탯이 달라지니까

            _hp = stat.maxHp;
            _maxHp = stat.maxHp;
            _attack = stat.attack;
        }


    }
}
