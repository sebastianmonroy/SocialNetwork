using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FingerHandler : MonoBehaviour 
{
	public static FingerHandler instance;

	// Prefab should be a gameobject you want to represent a mouse-driven finger with the FingerMouse.cs script attached.
	public GameObject fingerMousePrefab;
	// Prefab should be a gameobject you want to represent a mouse-driven finger with the FingerTouch.cs script attached.
	public GameObject fingerTouchPrefab;
	// Usually you'll want the above prefabs to be the same except in that they differ in which Finger script they have attached.

	// The current number of fingers being detected
	public int fingerCount = 0;
	// The current list of fingers being detected
	public List<Finger> fingers = new List<Finger>();
	// The current mouse-driven finger being detected if it exists
	public FingerMouse fingerMouse;

	// Booleans used to allow only mouse-driven OR touch-driven fingers to be detected at any time, not both
	// (Allowing both at the same time leads to problems that have to do with the way Unity already attempts to emulate )
	private bool allowTouches = true, allowMouse = false;
	
	// Enable this boolean to see an output of all fingers currently being detected (using their ToString() methods)
	public bool debugFingers;

	void Start()
	{
		instance = this;
	}

	void Update() 
	{
		List<Touch> newTouches = new List<Touch>();

		foreach (Touch touch in Input.touches)
		{
			newTouches.Add(touch);
		}

		if (allowTouches)
		{
			// Populate list of all new touches
			foreach (Finger finger in fingers) 
			{
				//Debug.Log((finger as FingerTouch).GetTouchId());
				if (finger.isTouch())
				{
					FingerTouch fingertouch = finger as FingerTouch;
					bool found = false;
					foreach (Touch touch in Input.touches) 
					{
						// Remove touches that correspond to fingers that have already been detected from the newTouches list
						if (touch.fingerId == fingertouch.GetTouchId()) 
						{
							found = true;
							newTouches.Remove(touch);
						}
					}

					// if you expected to find an Input.touch corresponding to this finger but didn't, set it as invalid.
					if (!found) 
					{
						finger.SetValidity(false);
					}
				}
			}

			// Make new FingerTouchs for remaining unaccounted-for touches
			foreach (Touch touch in newTouches) 
			{
				GameObject newFingerObject = Instantiate(fingerTouchPrefab) as GameObject;
				FingerTouch newFinger = newFingerObject.GetComponent<FingerTouch>();
				newFinger.SetTouch(touch);
				fingers.Add(newFinger);
			}

			// Prevent mouse input if there are touches
			if (Input.touchCount > 0)
			{
				allowMouse = false;
			}
			else
			{
				allowMouse = true;
			}
		}
		
		if (allowMouse)
		{
			// Create a new FingerMouse object if the left mouse button is pressed and add it to the fingers list
			if (Input.GetMouseButtonDown(0))
			{
				fingers.Clear();
				allowTouches = false;
				GameObject fingerMouseObject = Instantiate(fingerMousePrefab) as GameObject;
				fingerMouse = fingerMouseObject.GetComponent<FingerMouse>();
				fingers.Add(fingerMouse);
			} 
			// Set the FingerMouse object as invalid for cleanup when the left mouse button is released
			else if (Input.GetMouseButtonUp(0))
			{
				allowTouches = true;
				if (fingerMouse != null)
				{
					fingerMouse.SetValidity(false);
				}
			}
		}

		// Remove invalid fingers from the fingers list (you might not want this for some reason)
		CleanFingers();
		// Print all the detected fingers if debugFingers is true
		PrintAllFingers();

		// Update the public fingerCount int, useful for debugging
		this.fingerCount = fingers.Count;
	}

	void PrintAllFingers() 
	{
		if (debugFingers)
		{
			if (fingerCount > 0) 
			{
				string output = "";
				output += "+-----------" + fingerCount + " Fingers-----------+\n";
				foreach (Finger finger in fingers) 
				{
					output += "|\t" + finger.ToString() + "\n";
				}
				output += "+--------------------------------+";
				print(output);
			} 
			else 
			{
				//Debug.Log("########### 0 Fingers ###########");
			}
		}
	}

	void CleanFingers() 
	{
		// Clean up invalid Fingers (remove them from fingers list)
		List<Finger> valids = new List<Finger>();
		List<Finger> invalids = new List<Finger>();
		foreach (Finger finger in fingers)
		{
			if (finger.GetValidity())
			{
				valids.Add(finger);
			}
			else
			{
				invalids.Add(finger);
			}
		}

		fingers = valids;

		// YOU MIGHT NOT WANT TO DELETE THEM FOR SOME REASON???
		for (int i = 0; i < invalids.Count; i++)
		{
			Destroy(invalids[i].gameObject);
		}
	}
}
