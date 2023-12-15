using System;
using Unity.VisualScripting;
using Teamproject.Stat.ID;

namespace Teamproject.Stat
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
