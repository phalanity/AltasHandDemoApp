using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feature_01 : MonoBehaviour
{
	public GameObject gMixedRealityPlaysapce;
	public GameObject gPrefabPalm;
	public GameObject gPrefabThumbTip;
	public GameObject gPrefabThumbDistalJoint;
	public GameObject gPrefabIndexTip;
	public GameObject gPrefabIndexDistalJoint;
	public GameObject gPrefabIndexMiddleJoint;
	public GameObject gPrefabMiddleTip;
	public GameObject gPrefabMiddleDistalJoint;
	public GameObject gPrefabMiddleMiddleJoint;
	public GameObject gPrefabRingTip;
	public GameObject gPrefabRingDistalJoint;
	public GameObject gPrefabRingMiddleJoint;
	public GameObject gPrefabPinkyTip;
	public GameObject gPrefabPinkyDistalJoint;
	public GameObject gPrefabPinkyMiddleJoint;

	private Dictionary<string, GameObject> mPrefabsFingerTexture;
	private List<FingerPrefabObject> mLeftFingerPrefabs;
	private List<FingerPrefabObject> mRightFingerPrefabs;

	// Start is called before the first frame update
	void Start()
    {
		mLeftFingerPrefabs = new List<FingerPrefabObject>();
		mRightFingerPrefabs = new List<FingerPrefabObject>();

		mPrefabsFingerTexture = new Dictionary<string, GameObject>();
		mPrefabsFingerTexture.Add("Palm", gPrefabPalm);
		mPrefabsFingerTexture.Add("ThumbTip", gPrefabThumbTip);
		mPrefabsFingerTexture.Add("ThumbDistalJoint", gPrefabThumbDistalJoint);
		mPrefabsFingerTexture.Add("IndexTip", gPrefabIndexTip);
		mPrefabsFingerTexture.Add("IndexDistalJoint", gPrefabIndexDistalJoint);
		mPrefabsFingerTexture.Add("IndexMiddleJoint", gPrefabIndexMiddleJoint);
		mPrefabsFingerTexture.Add("MiddleTip", gPrefabMiddleTip);
		mPrefabsFingerTexture.Add("MiddleDistalJoint", gPrefabMiddleDistalJoint);
		mPrefabsFingerTexture.Add("MiddleMiddleJoint", gPrefabMiddleMiddleJoint);
		mPrefabsFingerTexture.Add("RingTip", gPrefabRingTip);
		mPrefabsFingerTexture.Add("RingDistalJoint", gPrefabRingDistalJoint);
		mPrefabsFingerTexture.Add("RingMiddleJoint", gPrefabRingMiddleJoint);
		mPrefabsFingerTexture.Add("PinkyTip", gPrefabPinkyTip);
		mPrefabsFingerTexture.Add("PinkyDistalJoint", gPrefabPinkyDistalJoint);
		mPrefabsFingerTexture.Add("PinkyMiddleJoint", gPrefabPinkyMiddleJoint);
	}

	private bool mFIsLeftHandScanned = false;
	private bool mFIsRightHandScanned = false;
	// Update is called once per frame
	void Update()
	{
		GameObject leftHand = null;
		GameObject rightHand = null;
		FindHandsObjectRoot(out leftHand, out rightHand);

		// left hand
		if (leftHand != null && !mFIsLeftHandScanned)
		{
			Debug.Log("left hand show up");
			mFIsLeftHandScanned = true;

			CreateFingerTextureObjects(leftHand, true);
		}
		if (leftHand == null && mFIsLeftHandScanned)
		{
			Debug.Log("left hand disappear");
			mFIsLeftHandScanned = false;

			foreach (FingerPrefabObject obj in mLeftFingerPrefabs)
			{
				Destroy(obj.Prefab);
			}
			mLeftFingerPrefabs.Clear();
		}
		if (leftHand != null)
		{
			foreach (FingerPrefabObject obj in mLeftFingerPrefabs)
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

			CreateFingerTextureObjects(rightHand, false);
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

	void FindJoint(GameObject Hand, string JointName, out GameObject Joint)
	{
		Joint = null;

		for (int i = 0; i < Hand.transform.childCount; i++)
		{
			GameObject child = Hand.transform.GetChild(i).gameObject;
			if (child.name.ToLower().Contains(JointName.ToLower()))
			{
				Joint = child;
			}
			if (Joint != null)
			{
				break;
			}
		}
	}

	void FindNextJoint(GameObject Hand, GameObject ThisObject, out GameObject NextJoint)
	{
		NextJoint = null;

		if (Hand == null || ThisObject == null)
		{
			return;
		}

		string thisTag = "";
		string rotationTargetTag = "";
		if (ThisObject.name.ToLower().Contains("Tip".ToLower()))
		{
			thisTag = "Tip";
			rotationTargetTag = "DistalJoint";
		}
		if (ThisObject.name.ToLower().Contains("DistalJoint".ToLower()))
		{
			thisTag = "DistalJoint";
			rotationTargetTag = "MiddleJoint";
		}
		if (ThisObject.name.ToLower().Contains("ThumbDistalJoint".ToLower()))
		{
			thisTag = "DistalJoint";
			rotationTargetTag = "ProximalJoint";
		}
		if (ThisObject.name.ToLower().Contains("ProximalJoint".ToLower()))
		{
			thisTag = "ProximalJoint";
			rotationTargetTag = "MetacarpalJoint";
		}
		if (ThisObject.name.ToLower().Contains("MiddleJoint".ToLower()))
		{
			thisTag = "MiddleJoint";
			rotationTargetTag = "Knuckle";
		}
		if (string.IsNullOrWhiteSpace(thisTag) || string.IsNullOrWhiteSpace(rotationTargetTag))
		{
			return;
		}

		string rotationTargetJointName = ThisObject.name.Replace(thisTag, rotationTargetTag);
		Transform rotationTargetJointTransform = Hand.transform.Find(rotationTargetJointName);
		if (rotationTargetJointTransform == null)
		{
			return;
		}
		NextJoint = rotationTargetJointTransform.gameObject;
	}

	void CreateFingerTextureObjects(GameObject Hand, bool IsLeftHand)
	{
		foreach(string key in mPrefabsFingerTexture.Keys)
		{
			GameObject jointObj = null;
			FindJoint(Hand, key, out jointObj);

			if(jointObj != null)
			{
				GameObject texObj = Instantiate(mPrefabsFingerTexture[key], jointObj.transform.position, jointObj.transform.rotation);
				if (!IsLeftHand)
				{
					texObj.transform.localScale = new Vector3(
						texObj.transform.localScale.x * -1,
						texObj.transform.localScale.y,
						texObj.transform.localScale.z);
				}

				GameObject nextJointObj = null;
				FindNextJoint(Hand, jointObj, out nextJointObj);

				if(nextJointObj != null)
				{
					FingerPrefabObject fingerPrefabObject = new FingerPrefabObject();
					fingerPrefabObject.Prefab = texObj;
					fingerPrefabObject.RotationBase = jointObj;
					fingerPrefabObject.RotationTarget = nextJointObj;

					mLeftFingerPrefabs.Add(fingerPrefabObject);
				}
				if (jointObj.name.ToLower().Contains("Palm".ToLower()))
				{
					texObj.transform.parent = jointObj.transform;
				}
			}
		}

	}


	// **** **** **** **** ****	
	public class FingerPrefabObject
	{
		public GameObject Prefab { get; set; }
		public GameObject RotationBase { get; set; }
		public GameObject RotationTarget { get; set; }
	}

}
