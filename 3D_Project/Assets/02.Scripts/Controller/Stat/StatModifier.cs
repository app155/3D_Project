using System;
using Unity.VisualScripting;
using Project3D.Stat.ID;

namespace Project3D.Stat
{
	public enum StatModType
	{
		None,
		FlatAdd,
		PercentAdd,
		PercentMul,
	}
    [Serializable]
    public class StatModifier
    {
        public StatID statID;
        public StatModType modType;
        public int value;
    }
}
