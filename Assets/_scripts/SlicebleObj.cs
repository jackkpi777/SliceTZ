using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BzKovSoft.ObjectSlicer.Samples
{
    public class SlicebleObj : MonoBehaviour
    {
		 
		IBzSliceableNoRepeat _sliceableAsync;

		public GameObject _slice;
		public Material[] _materials;
		public bool children = false;

		bool _inProgress;
		float _pointY;
		public float width;
		void Start()
		{
			_sliceableAsync = GetComponentInParent<IBzSliceableNoRepeat>();
		}

        private void Update()
        {
			
				//float pointY = _slice.transform.InverseTransformPoint(GameManager.instance.knife.transform.position).y;
				float pointY = GameManager.instance.knife.transform.position.y;
				if (_pointY > pointY)
				{
					_pointY = pointY;
				}
				foreach (var material in _materials)
				{
					material.SetFloat("_PointY", _pointY);
				
				
			}
			if (GameManager.instance.knife.transform.localPosition.y == GameManager.instance.knifeFinalPos.transform.position.y && _slice)
			{
				SlicebleObj sliceObj = _slice.GetComponent<SlicebleObj>();
				sliceObj.enabled = false;
				Rigidbody sliceRb = _slice.GetComponent<Rigidbody>();
				BoxCollider sliceCollider = _slice.GetComponent<BoxCollider>();
				sliceCollider.isTrigger = true;
				sliceRb.isKinematic = false;

				sliceRb.AddForce(new Vector3(0, -30, -30));
			}



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
				_sliceableAsync.Slice(plane, knife.SliceID, r =>
				{
					if (!r.sliced)
					{
						return;
					}
					_inProgress = true;
					_pointY = float.MaxValue;
					_slice = r.outObjectPos;

					GameManager.instance.slicePart = _slice;
					
						var meshFilter = _slice.GetComponent<MeshFilter>();
						float CenterX = meshFilter.sharedMesh.bounds.center.z;

						_materials = _slice.GetComponent<MeshRenderer>().materials;
						if (_slice.GetComponentInChildren<MeshRenderer>().materials != null)
						{
							_materials = _slice.GetComponentInChildren<MeshRenderer>().materials;
						}
						foreach (var material in _materials)
						{
							material.SetFloat("_PointX", CenterX * -1);

						}
						width = meshFilter.sharedMesh.bounds.size.x;/////wrong calculation!!!!!!!!!!
					    //width = CalculateWidth(_slice).z;
						Debug.Log("width" + width.ToString());

					if (width<.8f) 
					{
						foreach (var material in _materials)
						{
							float deviation = Mathf.Clamp(width, 1f, 1.5f);
							Debug.Log("wdth low, increase radius, width" + width.ToString());
							material.SetFloat("_Radius", /*Mathf.Exp(deviation) / 2*/ 0.5f);
						}
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
		Vector3 CalculateWidth(GameObject slice)
		{
			var meshFilter = _slice.GetComponent<MeshFilter>();
			float CenterZ = meshFilter.sharedMesh.bounds.center.z;

			float LastZ = meshFilter.sharedMesh.bounds.size.z;


			Vector3 topVertex = slice.GetComponent<MeshFilter>().sharedMesh.vertices[0];
			for (int i = 0; i < slice.GetComponent<MeshFilter>().sharedMesh.vertices.Length - 1; i++)
			{
				Vector3 vertex = slice.GetComponent<MeshFilter>().sharedMesh.vertices[i];
				Vector3 nextVertex = slice.GetComponent<MeshFilter>().sharedMesh.vertices[i + 1];
				if (vertex.z > nextVertex.z)
				{
					topVertex = vertex;
				}
				else if (vertex.z < nextVertex.z)
				{
					topVertex = nextVertex;
				}
			}
			return topVertex;

		}
	}
	
}

