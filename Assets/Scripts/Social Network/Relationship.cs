using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Relationship : MonoBehaviour
{
	private float startTime;
	public float lastActive;

	public int numAPings;
	public int numBPings;

	public float strength = 0f;
	public float decayRate = 0f;
	public static float maxDecayRate = 0.5f;
	public static float maxStrength = 5f;
	public static float maxSize = 0.5f;
	public AnimationCurve decayCurve;
	public float recordStrength = 0f;

	public static float maxInactivity = 20f;

	public Person A, B;

	void Start()
	{
		Initialize();
	}
	
	private void Initialize()
	{
		this.startTime = Time.time;
		this.numAPings = 0;
		this.numBPings = 0;
		this.strength = 0f;
		this.active = true;
	}

	void Update()
	{
		if (A != null && B != null)
		{
			UpdateTransform();
			Decay();
		}
	}

	public void SetPersons(Person A, Person B)
	{
		this.A = A;
		this.B = B;
	}

	public void UpdateTransform()
	{
		this.transform.position = (A.transform.position + B.transform.position) / 2f;
		this.transform.localScale = new Vector3(this.GetDistance(), strength / maxStrength * maxSize, 1f);
		this.transform.rotation = Quaternion.FromToRotation(Vector3.right, (B.transform.position - A.transform.position).normalized);
	}

	public void Decay()
	{
		ChangeStrength(-1f * Time.deltaTime * GetDecayRate());

		DeathByInactivity();
	}

	public float GetDecayRate()
	{
		return (21f - Mathf.Clamp(A.agreeableness, 0, 10) - Mathf.Clamp(B.agreeableness, 0, 10)) / 20f * maxDecayRate;
	}

	public void Ping(Person P)
	{
		lastActive = Time.time;

		if (P == A) 
		{
			this.numAPings++;
		}
		else if (P == B)
		{
			this.numBPings++;
		}

		ChangeStrength(GetCompatibility() * 2f);
		StartCoroutine(MoveToward(A, B, GetCompatibility(), 1.0f));
		lastActive = Time.time;
	}

	public void ChangeStrength(float amount)
	{
		this.strength = Mathf.Clamp(this.strength + amount, 0f, maxStrength);
		if (strength > recordStrength)
		{
			recordStrength = strength;
		}
	}

		IEnumerator MoveToward(Person P, Person Q, float amount, float duration)
		{
			Timer moveTimer = new Timer(duration);
			Vector3 origin = P.transform.position;
			Vector3 destination = Q.transform.position;
			//destination = origin + (destination - origin).normalized * amount;

			while (moveTimer.Percent() < 1f && GetDistance() >= 3f)
			{
				P.transform.position += (destination - origin).normalized * P.GetMovementSpeed() * Time.deltaTime;
				yield return 0;
			}
		}

	public float GetDistance()
	{
		return Vector3.Distance(A.transform.position, B.transform.position);
	}

	public float GetDuration()
	{
		return startTime - Time.time;
	}

	public void DeathByInactivity()
	{
		if ((strength <= 0f) && ((Time.time - lastActive) >= maxInactivity))
		{
			Kill();
		}
	}

	// compatibility is on a scale from -1f to 1f, where negative values mean the people will not get along well
	// depends on similarity of personalities, number of shared connections, ages, and distance from each other
	public float GetCompatibility()
	{
		float worst = 1 + Mathf.Pow(A.CountAttributes() - B.CountAttributes(), 2f);
		float actual = Mathf.Pow(A.extraversion - B.extraversion, 2f) + Mathf.Pow(A.agreeableness - B.agreeableness, 2f) + Mathf.Pow(A.conscientiousness - B.conscientiousness, 2f) + Mathf.Pow(A.neuroticism - B.neuroticism, 2f) + Mathf.Pow(A.openness - B.openness, 2f);
		float result = ((worst - actual) / worst);
		if (Random.Range(0f, A.neuroticism) > 4f)
		{
			result -= 1f;
		}

		return result;
	}

	void Kill()
	{
		Network.instance.DestroyRelationship(A, B);
		A.DestroyRelationship(B);
		Destroy(this.gameObject);
	}
}
