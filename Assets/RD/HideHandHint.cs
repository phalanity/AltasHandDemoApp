using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideHandHint : MonoBehaviour
{
	public GameObject gMixedRealityPlaysapce;
	public GameObject gBlackPlaneFingerPrefab;
	public GameObject gBlackPlaneMiddleJointPrefab;
	public GameObject gBlackPlanePalmPrefab;

	public bool gIsCreateFingerBlackBlock;
	public bool gIsHideHandHintCubes;
	
	private List<FingerPrefabObject> mLeftFingerPrefabs;
	private List<FingerPrefabObject> mRightFingerPrefabs;

	// Start is called before the first frame update
	void Start()
    {
		mLeftFingerPrefabs = new List<FingerPrefabObject>();
		mRightFingerPrefabs = new List<FingerPrefabObject>();
	}

	private bool mFIsLeftHandScanned = false;
	private bool mFIsRightHandScanned = false;
	// Update is called once per frame
	void Update()
	{
		GameObject leftHand = null;
		GameObject rightHand = null;
		FindHandsObjectRoot(out leftHand, out rightHand);
		HideSpatialAwarenessSystem();
		HideCursor();
		HideDiagnostics();

		// left hand
		if (leftHand != null && !mFIsLeftHandScanned)
		{
			Debug.Log("left hand show up");
			mFIsLeftHandScanned = true;

			if (gIsCreateFingerBlackBlock)
			{
				AddFingerBlackBlock(leftHand, true);
			}
			if (gIsHideHandHintCubes)
			{
				HideHandHintCubes(leftHand);
			}
		}
		if (leftHand == null && mFIsLeftHandScanned)
		{
			Debug.Log("left hand disappear");
			mFIsLeftHandScanned = false;

			foreach(FingerPrefabObject obj in mLeftFingerPrefabs)
			{
				Destroy(obj.Prefab);
			}
			mLeftFingerPrefabs.Clear();
		}
		if(leftHand != null)
		{
			foreach(FingerPrefabObject obj in mLeftFingerPrefabs)
			{
				obj.Prefab.transform.position = obj.RotationBase.transform.position;
				obj.Prefab.transform.rotation = Quaternion.LookRotation(obj.RotationTarget.transform.position - obj.RotationBase.transform.position, obj.RotationBase.transform.rotation * Vector3.up);
			}
		}

		// right hand
		if (rightHand != null && !mFIsRightHandScanned)
		{
			Debug.Log("right hand show up");
			mFIsRightHandScanned = true;

			if (gIsCreateFingerBlackBlock)
			{
				AddFingerBlackBlock(rightHand, false);
			}
			if (gIsHideHandHintCubes)
			{
				HideHandHintCubes(rightHand);
			}

		}
		if (rightHand == null && mFIsRightHandScanned)
		{
			Debug.Log("right hand disappear");
			mFIsRightHandScanned = false;

			foreach (FingerPrefabObject obj in mRightFingerPrefabs)
			{
				Destroy(obj.Prefab);
			}
			mRightFingerPrefabs.Clear();
		}
		if (rightHand != null)
		{
			foreach (FingerPrefabObject obj in mRightFingerPrefabs)
			{
				obj.Prefab.transform.position = obj.RotationBase.transform.position;
				obj.Prefab.transform.rotation = Quaternion.LookRotation(obj.RotationTarget.transform.position - obj.RotationBase.transform.position, obj.RotationBase.transform.rotation * Vector3.up);
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

	void HideHandHintCubes(GameObject Hand)
	{
		for (int i = 0; i < Hand.transform.childCount; i++)
		{
			GameObject child = Hand.transform.GetChild(i).gameObject;
			Renderer renderer = child.GetComponent(typeof(Renderer)) as Renderer;
			if (renderer != null)
			{
				renderer.enabled = false;
			}
		}
	}

	private bool mFSpatitalAwarenassActive = true;
	void HideSpatialAwarenessSystem()
	{
		if (mFSpatitalAwarenassActive)
		{
			for (int i = 0; i < gMixedRealityPlaysapce.transform.childCount; i++)
			{
				Transform child = gMixedRealityPlaysapce.transform.GetChild(i);
				if (child.name.ToLower().Contains("Spatial".ToLower()))
				{
					child.gameObject.SetActive(false);
					mFSpatitalAwarenassActive = false;
					break;
				}
			}
		}
	}

	void HideCursor()
	{
		for (int i = 0; i < gMixedRealityPlaysapce.transform.childCount; i++)
		{
			Transform child = gMixedRealityPlaysapce.transform.GetChild(i);
			if (child.name.ToLower().Contains("DefaultControllerPointer".ToLower()))
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	void HideDiagnostics()
	{
		for (int i = 0; i < gMixedRealityPlaysapce.transform.childCount; i++)
		{
			Transform child = gMixedRealityPlaysapce.transform.GetChild(i);
			if (child.name.ToLower().Contains("Diagnostics".ToLower()))
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	void AddFingerBlackBlock(GameObject Hand, bool IsLeftHand)
	{
		for(int i=0; i< Hand.transform.childCount; i++)
		{
			GameObject child = Hand.transform.GetChild(i).gameObject;

			if (child.name.ToLower().Contains("Palm".ToLower()))
			{
				GameObject blackPlane = Instantiate(gBlackPlanePalmPrefab, child.transform.position, child.transform.rotation);
				blackPlane.transform.parent = child.transform;
			}
			else
			{
				FingerPrefabObject fingerObject = SpawnFingerPrefabObject(Hand, child);
				if (fingerObject != null)
				{
					if (IsLeftHand)
					{
						mLeftFingerPrefabs.Add(fingerObject);
					}
					else
					{
						mRightFingerPrefabs.Add(fingerObject);
					}
				}
			}
		}

		/*for (int i = 0; i < Hand.transform.childCount; i++)
		{
			Transform child = Hand.transform.GetChild(i);
			if (child.name.ToLower().Contains("Palm".ToLower()))
			{
				GameObject blackPlane = Instantiate(gBlackPlanePalmPrefab, child.position, child.rotation);
				blackPlane.transform.parent = child;
			}
			else
			{
				GameObject blackPlane = Instantiate(gBlackPlaneFingerPrefab, child.position, child.rotation);
				blackPlane.transform.parent = child;
			}
		}*/
	}


	FingerPrefabObject SpawnFingerPrefabObject(GameObject Hand, GameObject ThisObject)
	{
		if(Hand == null || ThisObject == null)
		{
			return null;
		}

		string thisTag = "";
		string rotationTargetTag = "";
		GameObject spawnedPrefabType = null;
		if (ThisObject.name.ToLower().Contains("Tip".ToLower()))
		{
			thisTag = "Tip";
			rotationTargetTag = "DistalJoint";
			spawnedPrefabType = gBlackPlaneFingerPrefab;
		}
		if (ThisObject.name.ToLower().Contains("DistalJoint".ToLower()))
		{
			thisTag = "DistalJoint";
			rotationTargetTag = "MiddleJoint";
			spawnedPrefabType = gBlackPlaneFingerPrefab;
		}
		if (ThisObject.name.ToLower().Contains("ThumbDistalJoint".ToLower()))
		{
			thisTag = "DistalJoint";
			rotationTargetTag = "ProximalJoint";
			spawnedPrefabType = gBlackPlaneFingerPrefab;
		}
		if (ThisObject.name.ToLower().Contains("ProximalJoint".ToLower()))
		{
			thisTag = "ProximalJoint";
			rotationTargetTag = "MetacarpalJoint";
			spawnedPrefabType = gBlackPlaneMiddleJointPrefab;
		}
		if (ThisObject.name.ToLower().Contains("MiddleJoint".ToLower()))
		{
			thisTag = "MiddleJoint";
			rotationTargetTag = "Knuckle";
			spawnedPrefabType = gBlackPlaneMiddleJointPrefab;
		}
		if(string.IsNullOrWhiteSpace(thisTag) || string.IsNullOrWhiteSpace(rotationTargetTag) || spawnedPrefabType == null)
		{
			return null;
		}

		FingerPrefabObject Rtn = new FingerPrefabObject();

		string rotationTargetJointName = ThisObject.name.Replace(thisTag, rotationTargetTag);
		Transform rotationTargetJointTransform = Hand.transform.Find(rotationTargetJointName);
		if(rotationTargetJointTransform == null)
		{
			return null;
		}
		GameObject rotationTargetJointObject = rotationTargetJointTransform.gameObject;

		Rtn.Prefab = Instantiate(spawnedPrefabType, ThisObject.transform.position, ThisObject.transform.rotation);
		Rtn.RotationBase = ThisObject;
		Rtn.RotationTarget = rotationTargetJointObject;

		return Rtn;
	}
	

	// **** **** **** **** ****	
	public class FingerPrefabObject
	{
		public GameObject Prefab { get; set; }
		public GameObject RotationBase { get; set; }
		public GameObject RotationTarget { get; set; }
	}
}
