using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAreaAnimationComponent : MonoBehaviour
{

	[SerializeField]
	protected Sprite[] sprites;
	[SerializeField]
	protected float timeToFrame = 0.1f;

	public void Clear(SpikeAreaGridComponent.SpikeAreaGridNodeClass[] nodes)
	{
		for (int i = 0; i < nodes.Length; i++)
			nodes[i].renderer.sprite = sprites[0];
	}

	public bool OnLateUpdate(SpikeAreaGridComponent.SpikeAreaGridNodeClass[] nodes)
	{
		bool animate = false;
		for (int i = 0; i < nodes.Length; i++)
		{
			float time = timeToFrame;
			if (nodes[i].state != -1)
			{
				animate = true;
				if (Time.time >= nodes[i].time)
				{
					SpikeAreaGridComponent.SpikeAreaGridNodeClass node = nodes[i];
					switch (node.state)
					{
						case 0:
							node.renderer.sprite = sprites[0];
							time *= 0.0f;
							node.state = -1;

							break;
						case 1:
							node.renderer.sprite = sprites[1];
							time *= 0.25f;
							node.state = 2;
							break;
						case 2:
							node.renderer.sprite = sprites[2];
							time *= 2.0f;
							node.state = 3;
							break;
						case 3:
							node.renderer.sprite = sprites[1];
							time *= 0.5f;
							node.state = 0;
							break;
						default:
							break;
					}
					node.time = Time.time + time;
				}
			}
		} 

		return animate;
	}
}
