using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonAI : Person
{	
	void Update ()
	{
		LookAtCamera();
		this.age = Age();
		RandomLevelUp();
		UpdateSize();

		if (pingTimer.Percent() >= 1f)
		{
			if (connections.Count > 0)
			{
				Relationship R = GetRandomRelationship();
				R.Ping(this);
			}

			GetNextTimerDuration();
		}
	}

	private Person GetRandomConnection()
	{
		List<Person> friendsOfFriends = new List<Person>();
		foreach (Person p in connections)
		{
			foreach (Person q in p.connections)
			{
				if (!friendsOfFriends.Contains(q))
				{
					friendsOfFriends.Add(q);
				}
			}
		}

		int rand = Random.Range(0, connections.Count + friendsOfFriends.Count);
		if (rand < connections.Count)
		{
			return connections[rand];
		}
		else
		{
			return friendsOfFriends[rand - connections.Count];
		}
	}

	private Relationship GetRandomRelationship()
	{
		Person P = GetRandomConnection();
		Relationship R = Network.instance.GetRelationship(this, P);
		if (R == null)
		{
			return Network.instance.CreateRelationship(this, P);
		}
		else 
		{
			return R;
		}
	}
}
