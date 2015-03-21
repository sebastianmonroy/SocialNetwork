using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Person : MonoBehaviour
{
	// HUMAN PROPERTIES
	public int id;
	private float birthtime;
	public float age;

	public AnimationCurve levelUpFrequency;

	// BIG 5 PERSONALITY ATTRIBUTES
	public int extraversion = 0;		// friendliness, activity level
	public int agreeableness = 0;		// trust, cooperation, sympathy
	public int conscientiousness = 0;	// cautiousness, orderliness, self-discipline
	public int neuroticism = 0;			// anxiety, anger, depression, self-consciousness, vulnerability
	public int openness = 0;			// imagination, intellect, adventurousness

	// Extraversion affects the frequency of pinging other people
	// Agreeableness affects the decay rate of relationships
	// Conscientiousness affects the amount the person prefers to keep their distance
	// Neuroticism affects the chance that a ping can result negatively
	// Openness affects the distance a ping can move the person

	public Timer pingTimer;

	public SocialSphere socialSphere;
	public List<Person> connections = new List<Person>();	// current relationships

	public static int nextID = 0;
	public bool randomizeAttributes;

	private Timer levelUpTimer;

	void Awake()
	{
		if (randomizeAttributes)
		{
			SetRandomAttributes();
		}
		else
		{
			this.birthtime = Time.time;
		}

		this.id = nextID++;
		this.gameObject.name = "Person " + this.id;

		pingTimer = new Timer(1.0f);
	}

	void Update()
	{
		LookAtCamera();
		this.age = Age();
		UpdateSize();
	}

	public void UpdateSize()
	{
		this.transform.localScale = Vector3.one * (0.1f + levelUpFrequency.Evaluate(Age() / 300f));
	}

	public void RandomLevelUp()
	{
		if (levelUpTimer.Percent() >= 1f)
		{
			if (Age() < 150f)
			{
				GainRandomAttribute();
			}
			else
			{
				RemoveRandomAttribute();
			}
			//Debug.Log("aged");
			levelUpTimer = new Timer(5f + (levelUpFrequency.Evaluate(Age() / 300f)) * 20f);
		}
	}

	private void GainRandomAttribute()
	{
		int randAttribute = Random.Range(0, 5);

		switch(randAttribute)
		{
			case 0:
				extraversion++;
				break;
			case 1:
				agreeableness++;
				break;
			case 2:
				conscientiousness++;
				break;
			case 3:
				neuroticism++;
				break;
			case 4:
				openness++;
				UpdateSocialSphere();
				break;
		}
	}

	private void RemoveRandomAttribute()
	{
		int rand = Random.Range(0, extraversion+agreeableness+conscientiousness+neuroticism+openness);
		int randAttribute = 0;

		if (rand < extraversion)
		{
			randAttribute = 0;
		}
		else if (rand - extraversion < agreeableness)
		{
			randAttribute = 1;
		}
		else if (rand - extraversion - agreeableness < conscientiousness)
		{
			randAttribute = 2;
		}
		else if (rand - extraversion - agreeableness - conscientiousness < neuroticism)
		{
			randAttribute = 3;
		}
		else
		{
			randAttribute = 4;
		}


		switch(randAttribute)
		{
			case 0:
				extraversion--;
				break;
			case 1:
				agreeableness--;
				break;
			case 2:
				conscientiousness--;
				break;
			case 3:
				neuroticism--;
				break;
			case 4:
				openness--;
				UpdateSocialSphere();
				break;
		}
	}

	public void LookAtCamera()
	{
		Vector3 cameraPos = Camera.main.transform.position;
		Vector3 thisToCamera = cameraPos - this.transform.position;
		this.transform.LookAt(this.transform.position - thisToCamera);
	}

	public void UpdateSocialSphere()
	{
		socialSphere.SetRadius(1f+openness*2f);
	}

	public float GetNextTimerDuration()
	{
		float duration = 4f - Random.Range(0f, (Mathf.Clamp(this.extraversion, 1, 10))/3f);
		pingTimer = new Timer(duration);
		return duration;
	}

	public int CountAttributes()
	{
		return extraversion + agreeableness + conscientiousness + neuroticism + openness;
	}

	public void SetRandomBirthtime() 
	{
		this.birthtime = Time.time - Random.Range(0f, 150f);
	}

	public void SetRandomAttributes()
	{
		SetRandomBirthtime();
		int availablePoints = (int) (this.Age() / 15f);

		for (int i = 0; i < availablePoints; i++)
		{
			GainRandomAttribute();
		}

		levelUpTimer = new Timer(5f + (levelUpFrequency.Evaluate(Age() / 300f)) * 20f);
	}

	public int Id()
	{
		return id;
	}

	public float Age()
	{
		return Mathf.Abs(Time.time - birthtime);
	}

	public void PingPerson(Person P)
	{
		if (P == this)
		{
			Debug.Log("Can't ping self");
		}
		else
		{
			if (connections.Contains(P))
			{
				Network.instance.relationships[new Person[2]{this, P}].Ping(this);
			}
			else
			{
				this.CreateRelationship(P);
			}
		}
	}

	public void CreateRelationship(Person P)
	{
		Person[] key = new Person[2] {this, P};

		if (!connections.Contains(P))
		{
			connections.Add(P);
		}

		if (!P.connections.Contains(this))
		{
			P.connections.Add(this);
		}
		
		Network.instance.CreateRelationship(this, P);
	}

	public float GetMovementSpeed()
	{
		return Mathf.Sqrt(1f+Mathf.Clamp(openness, 0, 10));
	}

	public void DestroyRelationship(Person P)
	{
		connections.Remove(P);
		P.connections.Remove(this);
	}
}
