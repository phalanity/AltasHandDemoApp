using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Feature_02 : MonoBehaviour
{
	public GameObject gMixedRealityPlaysapce;
	public float gTimeSpawnIntervalInSecond;
	public int gPrefabsAmountLimit;
	public GameObject[] gPrefabs;

	private Dictionary<string, List<HandPrefabObject>> mMapLeftHandSpawnedPrefabs;
	private Dictionary<string, List<HandPrefabObject>> mMapRightHandSpawnedPrefabs;

	private bool mStartSpawing = false;
	private int mPrefabsAmount;

	// Start is called before the first frame update
	void Start()
    {
		mPrefabsAmount = 0;

		mMapLeftHandSpawnedPrefabs = new Dictionary<string, List<HandPrefabObject>>();
		mMapRightHandSpawnedPrefabs = new Dictionary<string, List<HandPrefabObject>>();
	}


	private bool mFIsLeftHandScanned = false;
	private float mTimePassed = 0;
	// Update is called once per frame
	void Update()
	{
		GameObject leftHand = null;
		GameObject rightHand = null;
		FindHandsObjectRoot(out leftHand, out rightHand);

		mTimePassed += Time.deltaTime;
		if (mTimePassed >= gTimeSpawnIntervalInSecond)
		{
			if (mStartSpawing)
			{
				if (System.Convert.ToBoolean(Random.Range(0, 100) % 2))
				{
					SpawnPrefabOnHand(leftHand, true);
				}
				else
				{
					SpawnPrefabOnHand(rightHand, false);
				}
			}

			mTimePassed = 0;
		}

		if (leftHand != null && !mFIsLeftHandScanned)
		{
			Debug.Log("left hand show up");
			mFIsLeftHandScanned = true;
		}
		if (leftHand == null && mFIsLeftHandScanned)
		{
			Debug.Log("left hand disappear");
			mFIsLeftHandScanned = false;
		}

		foreach (string key in mMapLeftHandSpawnedPrefabs.Keys)
		{
			if (leftHand != null)
			{
				Transform root = leftHand.transform.Find(key);
				foreach (HandPrefabObject obj in mMapLeftHandSpawnedPrefabs[key])
				{
					obj.Prefab.transform.position = root.transform.position;//0820
					obj.Prefab.transform.rotation = root.transform.rotation;
					if (obj.IsInside)
					{
						obj.Prefab.transform.localScale = new Vector3(0.01f, -0.01f, 0.01f);
					}
				}
			}
		}

		foreach (string key in mMapRightHandSpawnedPrefabs.Keys)
		{
			if (rightHand != null)
			{
				Transform root = rightHand.transform.Find(key);
				foreach (HandPrefabObject obj in mMapRightHandSpawnedPrefabs[key])
				{
					obj.Prefab.transform.position = root.transform.position + new Vector3(0, 0.01f, 0);
					obj.Prefab.transform.rotation = root.transform.rotation;
					if (obj.IsInside)
					{
						obj.Prefab.transform.localScale = new Vector3(0.01f, -0.01f, 0.01f);
					}
				}
			}
		}
	}


	// **** **** **** **** ****	
	void FindHandsObjectRoot(out GameObject LeftHand, out GameObject RightHand)
	{
		LeftHand = RightHand = null;

		int playspaceChildCount = gMixedRealityPlaysapce.transform.childCount;
		for (int i = 0; i < playspaceChildCount; i++)
		{
			GameObject child = gMixedRealityPlaysapce.transform.GetChild(i).gameObject;
			if (child.name.ToLower().Contains("Left_HandLeft".ToLower()))
			{
				LeftHand = child;
			}
			if (child.name.ToLower().Contains("Right_HandRight".ToLower()))
			{
				RightHand = child;
			}
			if (LeftHand != null && RightHand != null)
			{
				break;
			}
		}
	}

	void SpawnPrefabOnHand(GameObject Hand, bool IsLeftHand)
	{
		if (Hand == null)
		{
			return;
		}
		if (mPrefabsAmount >= gPrefabsAmountLimit)
		{
			return;
		}

		int targetPlaceIndex = Random.Range(0, Hand.transform.childCount);
		int targetPrefabIndex = Random.Range(0, gPrefabs.Length);
		GameObject targetPlace = Hand.transform.GetChild(targetPlaceIndex).gameObject;

		GameObject newObject = Instantiate(gPrefabs[targetPrefabIndex], targetPlace.transform.position, targetPlace.transform.rotation);
		for (int i = 0; i < newObject.transform.childCount; i++)
		{
			Transform newObjTransform = newObject.transform.GetChild(i);
			newObjTransform.position += new Vector3(0, 0.01f, 0);
		}

		HandPrefabObject newHandPrefabObject = new HandPrefabObject();
		newHandPrefabObject.Prefab = newObject;
		newHandPrefabObject.IsInside = System.Convert.ToBoolean(Random.Range(0, 100) % 2);
		if (IsLeftHand)
		{
			if (!mMapLeftHandSpawnedPrefabs.ContainsKey(targetPlace.name))
			{
				mMapLeftHandSpawnedPrefabs.Add(targetPlace.name, new List<HandPrefabObject>());
			}
			mMapLeftHandSpawnedPrefabs[targetPlace.name].Add(newHandPrefabObject);
		}
		else
		{
			if (!mMapRightHandSpawnedPrefabs.ContainsKey(targetPlace.name))
			{
				mMapRightHandSpawnedPrefabs.Add(targetPlace.name, new List<HandPrefabObject>());
			}
			mMapRightHandSpawnedPrefabs[targetPlace.name].Add(newHandPrefabObject);
		}

		mPrefabsAmount++;
	}

	public void StartSpawning()
	{
		mStartSpawing = true;
	}

	public void StopSpawning()
	{
		mStartSpawing = false;
	}

	public void ClearAll()
	{
		foreach (string key in mMapLeftHandSpawnedPrefabs.Keys)
		{
			foreach (HandPrefabObject obj in mMapLeftHandSpawnedPrefabs[key])
			{
				Destroy(obj.Prefab);
			}
		}
		foreach (string key in mMapRightHandSpawnedPrefabs.Keys)
		{
			foreach (HandPrefabObject obj in mMapRightHandSpawnedPrefabs[key])
			{
				Destroy(obj.Prefab);
			}
		}
		mMapLeftHandSpawnedPrefabs.Clear();
		mMapRightHandSpawnedPrefabs.Clear();

		mPrefabsAmount = 0;
	}


	// **** **** **** **** ****	
	public class HandPrefabObject
	{
		public GameObject Prefab { get; set; }
		public bool IsInside = false;
	}


}
