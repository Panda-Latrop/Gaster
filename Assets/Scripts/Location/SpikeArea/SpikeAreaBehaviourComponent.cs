using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAreaBehaviourComponent : MonoBehaviour, ISaveableComponent
{
	public enum SpikeBehaviourType
	{
		none, one, line, cut
	}
	public enum SpikeBehaviourOnEndType
	{
		end, loop, pong,
	}
	protected SpikeAreaPatternComponent pattern;
	protected int currentX, currentY;
	protected bool directionX, directionY;
	protected float nextSpike;

	public SpikeAreaPatternComponent Pattern
	{
		set
		{
			pattern = value;
			currentX = value.x.start;
			currentY = value.y.start;
			directionX = value.x.direction;
			directionY = value.y.direction;
		}
	}

	public void Edge(Vector2Int size)
	{
		if (currentX >= size.x || currentX < 0)
		{
			switch (pattern.x.end)
			{
				case SpikeBehaviourOnEndType.loop:
					if (directionX)
						currentX = 0;
					else
						currentX = size.x - 1;
					break;
				case SpikeBehaviourOnEndType.pong:
					if (directionX)
						currentX = size.x - 2;
					else
						currentX = 1;
					directionX = !directionX;
					break;
				default:
					break;
			}
		}
		if (currentY >= size.y || currentY < 0)
		{
			switch (pattern.y.end)
			{
				case SpikeBehaviourOnEndType.loop:
					if (directionY)
						currentY = 0;
					else
						currentY = size.y - 1;
					break;
				case SpikeBehaviourOnEndType.pong:
					if (directionY)
						currentY = size.y - 2;
					else
						currentY = 1;
					directionY = !directionY;
					break;
				default:
					break;
			}
		}
	}
	public void Shift(SpikeAreaGridComponent.SpikeAreaGridNodeClass[] nodes, Vector2Int size)
	{
		{
			switch (pattern.x.type)
			{
				case SpikeBehaviourType.one:
					nodes[currentX + currentY * size.x].state = 1;
					break;
				case SpikeBehaviourType.line:
					for (int y = 0; y < size.y; y++)
						nodes[currentX + y * size.x].state = 1;
					break;
				case SpikeBehaviourType.cut:
					for (int y = 1; y <= pattern.x.cutLenght && (currentY + y) < size.y; y++)
						nodes[currentX + (currentY + y) * size.x].state = 1;
					nodes[currentX + currentY * size.x].state = 1;
					for (int y = 1; y <= pattern.x.cutLenght && (currentY - y) >= 0; y++)
						nodes[currentX + (currentY - y) * size.x].state = 1;
					break;
			}
			switch (pattern.y.type)
			{
				case SpikeBehaviourType.one:
					nodes[currentX + currentY * size.x].state = 1;
					break;
				case SpikeBehaviourType.line:
					for (int x = 0; x < size.x; x++)
						nodes[x + currentY * size.x].state = 1;
					break;
				case SpikeBehaviourType.cut:
					for (int x = 1; x <= pattern.y.cutLenght && (currentX + x) < size.x; x++)
						nodes[(currentX + x) + currentY * size.x].state = 1;
					nodes[currentX + currentY * size.x].state = 1;
					for (int x = 1; x <= pattern.y.cutLenght && (currentX - x) >= 0; x++)
						nodes[(currentX - x) + currentY * size.x].state = 1;
					break;
			}
			if (!pattern.x.type.Equals(SpikeBehaviourType.none))
			{
				if (directionX)
					currentX++;
				else
					currentX--;
			}
			if (!pattern.y.type.Equals(SpikeBehaviourType.none))
			{
				if (directionY)
					currentY++;
				else
					currentY--;
			}
		}
	}
	public void Ignore(SpikeAreaGridComponent.SpikeAreaGridNodeClass[] nodes, Vector2Int size)
	{
		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				int id = x + y * size.x;
				if (x == pattern.x.ignore || y == pattern.y.ignore || nodes[id].state == 0)
				{
					nodes[id].state = 0;
				}
			}
		}
	}
	public virtual bool OnUpdate(SpikeAreaGridComponent.SpikeAreaGridNodeClass[] nodes, Vector2Int size)
	{
		if (Time.time >= nextSpike)
		{
			Edge(size);
			Shift(nodes, size);
			Ignore(nodes, size);
			nextSpike = Time.time + pattern.timeToSpike;
			return true;
		}
		return false;
	}
	/*
	protected int currentX, currentY;
	protected bool directionX, directionY;
	protected float nextSpike;
	 * */
	public JSONObject Save(JSONObject jsonObject)
	{
		jsonObject.Add("currentX",new JSONNumber(currentX));
		jsonObject.Add("currentY", new JSONNumber(currentY));
		jsonObject.Add("directionX", new JSONBool(directionX));
		jsonObject.Add("directionY", new JSONBool(directionY));
		SaveSystem.TimerSave(jsonObject, "spike", nextSpike);
		return jsonObject;
	}

	public JSONObject Load(JSONObject jsonObject)
	{
		currentX = jsonObject["currentX"].AsInt;
		currentY = jsonObject["currentY"].AsInt;
		directionX = jsonObject["directionX"].AsBool;
		directionY = jsonObject["directionY"].AsBool;
		SaveSystem.TimerLoad(jsonObject, "spike",ref nextSpike);
		return jsonObject;
	}
}