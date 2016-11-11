using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using GraphCore;
using System;
using SimpleJSON;

namespace Topology {

	public class Tuple<T,U>
	{
	    public T Item1 { get; private set; }
	    public U Item2 { get; private set; }
	    public Tuple(T item1, U item2)
	    {
	        Item1 = item1;
	        Item2 = item2;
	    }
	}

	public class GameController : MonoBehaviour {

		public Node nodePrefab;
		public Link linkPrefab;

		public Hashtable nodes;
		public Hashtable links;
		//private GUIText statusText;
		public int nodeCount = 0;
		public int linkCount = 0;
		//public GUIText nodeCountText;
		//public GUIText linkCountText;


		//Method for loading the GraphML layout file
		public IEnumerator LoadLayout(int root_id, int depth){

			//statusText.text = "Camera position: " + Camera.main.gameobject.transform.position.ToString();
			
			print("Step_1: Initiating graph builder...");
			Tuple<object, object> graph = build_graph(root_id,depth);
			print("Step_2: Finished building graph...");

			HashSet<int> node_set = (HashSet<int>) graph.Item1;
			HashSet<Tuple<int,int>> edge_set = (HashSet<Tuple<int,int>>) graph.Item2;

			List<int> node_list = node_set.ToList<int>();
			List<Tuple<int,int>> edge_list = edge_set.ToList<Tuple<int,int>>();
			
			JSONNode page_info = WikiWebRequestCore.get_page_info(node_list);
			
			for(int j=0; j<node_list.Count; j++){
				int id = node_list[j];
				add_node(id, page_info);
					
					
				if(j % 100 == 0)
					yield return true;
				}	

			for(int j=0; j<edge_list.Count; j++){
				int src = edge_list[j].Item1;
				int dest = edge_list[j].Item2;

				add_edge(src, dest);
				if(j % 100 == 0)
					yield return true;
			}	
			
		}

		public void add_node(int id, JSONNode page_info){
			
			if (nodes.ContainsKey(id)==false){
				float x = UnityEngine.Random.Range(-50.0f,50.0f);
				float y = UnityEngine.Random.Range(-50.0f,50.0f);
				float z = UnityEngine.Random.Range(-50.0f,50.0f);
				
				if (id == 736){x=0;y=0;z=0;} //Einstein is the main man and knows his position exactly.
				Node nodeobject = Instantiate(nodePrefab, new Vector3(x,y,z), Quaternion.identity) as Node;
				if (id == 736){nodeobject.anchored = true;} //Einstein is an immovable object.
				
				//Nodes title:
				nodeobject.nodeText.text = page_info[id.ToString()]["title"];
				//Nodes velocity:
				nodeobject.velocity = new Vector3(0,0,0);
				//Nodes id:
				nodeobject.id = id;
				nodeobject.control = this;
				nodeobject.gameObject.SetActive(true);
				nodes.Add(nodeobject.id, nodeobject);

				nodeCount++;
				//nodeCountText.text = "Nodes: " + nodeCount;
			}
			return;
		}

		public void add_edge(int src, int dest){

			string id_1 = src+"_"+dest;
			string id_2 = dest+"_"+src;

			if (!links.ContainsKey(id_1) &&  !links.ContainsKey(id_2)){
				Link linkobject = Instantiate(linkPrefab, new Vector3(0,0,0), Quaternion.identity) as Link;
				linkobject.id = src+"_"+dest;
				linkobject.sourceId = src;
				linkobject.targetId = dest;

				linkobject.status = ""; //Comment: "status" actually determines color
				links.Add(linkobject.id, linkobject);
				linkCount++;
				//linkCountText.text = "Edges: " + linkCount;

				foreach(string key in links.Keys){
					Link link = links[key] as Link;
					link.source = nodes[link.sourceId] as Node;
					link.target = nodes[link.targetId] as Node;
				}
			}
			return;
		}



		public Tuple<object, object> build_graph(int root_id, int depth){

			print("Searching node:" + root_id.ToString() + ", Depth: " + depth.ToString());
			//Initialize edges and nodes of current level:
			HashSet<int> node_set = new HashSet<int>();
			HashSet<Tuple<int,int>> edge_set = new HashSet<Tuple<int,int>>();

			//base case:
			if (depth <= 1){
				print("Base case: depth <=1");
				node_set.Add(root_id);
			}
			//general case:
			else{
				//add root: 
				node_set.Add(root_id);
				//request linked nodes [TODO: make it request variable amounts of nodes]
				print("Fetching child nodes...");
				List<string> unexplored = WikiWebRequestCore.get_linked_page_id_list(root_id);
				foreach(string node in unexplored){
					if (Convert.ToInt32(node)>0){ //Note: -ve value node indices aren't Wikipedia pages
						print ("Child: " + node);
						//conver to int, recurse:
						node_set.Add(Convert.ToInt32(node));
						Tuple<object,object> subgraph = build_graph(Convert.ToInt32(node),depth-1);

						//Get child nodes and edges:
						HashSet<int> child_nodes = (HashSet<int>)subgraph.Item1;
						HashSet<Tuple<int,int>> child_edges = (HashSet<Tuple<int,int>>) subgraph.Item2;

						//update:
						node_set.UnionWith(child_nodes);
						edge_set.UnionWith(child_edges);

						//add edges from root to children:
						edge_set.Add(new Tuple<int,int>(root_id,Convert.ToInt32(node)));
					}
				}
			}
			return new Tuple<object, object>(node_set,edge_set);
		}

		

		void Start () {
			nodes = new Hashtable();
			links = new Hashtable();
			//initial stats
			//nodeCountText = GameObject.Find("NodeCount").GetComponent<GUIText>();
			//nodeCountText.text = "Nodes: 0";
			//linkCountText = GameObject.Find("LinkCount").GetComponent<GUIText>();
			//linkCountText.text = "Edges: 0";
			//statusText = GameObject.Find("StatusText").GetComponent<GUIText>();
			//statusText.text = "";

			StartCoroutine( LoadLayout(736,2) );
		}
	}
}
