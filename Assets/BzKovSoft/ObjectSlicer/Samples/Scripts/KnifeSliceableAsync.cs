using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace BzKovSoft.ObjectSlicer.Samples
{
	/// <summary>
	/// This script will invoke slice method of IBzSliceableNoRepeat interface if knife slices this GameObject.
	/// The script must be attached to a GameObject that have rigidbody on it and
	/// IBzSliceable implementation in one of its parent.
	/// </summary>
	public class KnifeSliceableAsync : MonoBehaviour
	{
		IBzSliceableNoRepeat _sliceableAsync;
		
		public GameObject _slice;
		public Material[] _materials;
		void Start()
		{
			_sliceableAsync = GetComponentInParent<IBzSliceableNoRepeat>();
		}

		void OnTriggerEnter(Collider other)
		{
			var knife = other.gameObject.GetComponent<BzKnife>();
			if (knife == null)
				return;

			StartCoroutine(Slice(knife));
		}

		public IEnumerator Slice(BzKnife knife)
		{
			// The call from OnTriggerEnter, so some object positions are wrong.
			// We have to wait for next frame to work with correct values
			yield return null;

			Vector3 point = GetCollisionPoint(knife);
			Vector3 normal = Vector3.Cross(knife.MoveDirection, knife.BladeDirection);
			Plane plane = new Plane(normal, point);

			if (_sliceableAsync != null)
			{
				_sliceableAsync.Slice(plane, knife.SliceID, r=> 
				{
					if (!r.sliced) 
					{
						return;
					}
					_slice = r.outObjectPos;

					var meshFilter = _slice.GetComponent<MeshFilter>();
					float CenterX = meshFilter.sharedMesh.bounds.center.x;

					_materials = _slice.GetComponent<MeshRenderer>().materials;
                    foreach (var material in _materials)
                    {
						material.SetFloat("_PointX",CenterX);
                    }
				});
			}
		}

		private Vector3 GetCollisionPoint(BzKnife knife)
		{
			Vector3 distToObject = transform.position - knife.Origin;
			Vector3 proj = Vector3.Project(distToObject, knife.BladeDirection);

			Vector3 collisionPoint = knife.Origin + proj;
			return collisionPoint;
		}
	}
}