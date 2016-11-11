using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using GraphCore;
using System;
using SimpleJSON;

namespace Topology {

	public class Node : MonoBehaviour {

		public int id;
		public float mass = 1.0f;
		public bool anchored = false;
		public Vector3 velocity;

		public GameController control;

		public TextMesh nodeText;

		public Canvas canvas;
		public Button url_button;
		public Button expand_button;

		void Start(){
			(this.canvas).gameObject.SetActive(false); //deactivate all menus

			Button btn = this.url_button.GetComponent<Button>();
			btn.onClick.AddListener(url_button_action);			

			btn = this.expand_button.GetComponent<Button>();
			btn.onClick.AddListener( ()=>{ expand_button_action();});			


		}

		void Update () {

			//node text and options panel always facing camera
			nodeText.transform.LookAt (Camera.main.transform);
			canvas.transform.LookAt (Camera.main.transform);

			//if not anchored: move it
			if (!anchored){
				double a = 3.0;
				//for every node: calculate force
				foreach(DictionaryEntry e in control.nodes){
					Node n = (Node)e.Value;
					if (n.id != this.id){
						Vector3 diff = this.transform.position - n.transform.position;
						double r = (double)diff.magnitude;
						//Coloumb-like inverse square law
						this.velocity = this.velocity + (float)(a/(r*r))*diff.normalized;
					}
				}
				//Update particle velocity if not anchored:
				this.transform.position = this.transform.position + this.velocity;
			}
			
		}

		void OnMouseDown(){
			//this.nodeText.text = this.nodeText.text+"[clicked]";
			GameObject c = this.canvas.gameObject;
			c.SetActive(!c.activeSelf);

			//string url = "http://en.wikipedia.org/?curid=" + this.id.ToString();
			//Application.OpenURL(url);
			
		}

		public void url_button_action(){
			string url = "http://en.wikipedia.org/?curid=" + this.id.ToString();
			Application.OpenURL(url);
			(this.canvas).gameObject.SetActive(false);

		}

		public void expand_button_action(){		
			Tuple<object,object> graph = this.control.build_graph(this.id,2);
			HashSet<int> node_set = (HashSet<int>) graph.Item1;
			HashSet<Tuple<int,int>> edge_set = (HashSet<Tuple<int,int>>) graph.Item2;
			node_set.Remove(this.id);

			JSONNode page_info = WikiWebRequestCore.get_page_info(node_set.ToList<int>());
			foreach(int nodeid in node_set){
				this.control.add_node(nodeid, page_info);
				this.control.add_edge(this.id, nodeid);
			}
			(this.canvas).gameObject.SetActive(false);
		}

	}
}