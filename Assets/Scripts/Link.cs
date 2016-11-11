using UnityEngine;
using System.Collections;

namespace Topology {

	public class Link : MonoBehaviour {

		public string id;
		public Node source;
		public Node target;
		public int sourceId;
		public int targetId;
		
		public string status;
		public bool loaded = false;

		private LineRenderer lineRenderer;

		void Start () {
			lineRenderer = gameObject.AddComponent<LineRenderer>();

			//color link according to status
			Color c = Color.gray;
			c.a = 0.5f;

			//draw line
			lineRenderer.material = new Material (Shader.Find("Standard"));
			lineRenderer.material.SetColor ("_Color", c);
			lineRenderer.SetWidth(0.3f, 0.3f);
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetPosition(0, new Vector3(0,0,0));
			lineRenderer.SetPosition(1, new Vector3(1,0,0));
		}

		void Update () {
			if(source && target && !loaded){
				//draw links as full duplex, half in each direction
				//Vector3 m = (target.transform.position - source.transform.position)/2 + source.transform.position;
				source.mass = source.mass +1;
				target.mass = target.mass +1;
				lineRenderer.SetPosition(0, source.transform.position);
				lineRenderer.SetPosition(1, target.transform.position);

				loaded = true;
			}

			float k = 2.5f;
			if (source && target){


				Vector3 m = target.transform.position - source.transform.position;
				Vector3 norm_m = m.normalized;
				float x = m.magnitude - 30.0f;
				float c = 0.7f; //damping constant
				if (target.anchored == false){
					target.velocity = target.velocity - ((k*x*norm_m)/target.mass);
					target.velocity =  (1.0f-c)*target.velocity;
				}
				if (source.anchored == false){
					source.velocity = source.velocity + ((k*x*norm_m)/source.mass);
					source.velocity =  (1.0f-c)*source.velocity;
				}
				
				lineRenderer.SetPosition(0, source.transform.position);
				lineRenderer.SetPosition(1, target.transform.position);
		
				//loaded = false;	
			}
		}
	}

}