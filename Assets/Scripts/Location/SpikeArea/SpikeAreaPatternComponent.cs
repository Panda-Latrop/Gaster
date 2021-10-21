using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAreaPatternComponent : MonoBehaviour
{

	public SpikeBehaviourStruct x, y;
	public float timeToSpike;
	[System.Serializable]
	public struct SpikeBehaviourStruct
	{
		public int start;
		public bool direction;
		public SpikeAreaBehaviourComponent.SpikeBehaviourType type;
		public int cutLenght;
		public SpikeAreaBehaviourComponent.SpikeBehaviourOnEndType end;
		public int ignore;
	}


}
