using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Security;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SimpleJSON;


namespace GraphCore {
//public static class JsonHelper{
//	public static object Deserialize(string json){
//		return ToObject(JToken.Parse(json));
//	}
//	private static object ToObject(JToken token)
//	{
//		switch (token.Type)
//		{
//			case JTokenType.Object:
//			return token.Children<JProperty>().ToDictionary(prop => prop.Name, prop => ToObject(prop.Value));
//			case JTokenType.Array:
//			return token.Select(ToObject).ToList();
//			default:
//			return ((JValue)token).Value;
//		}
//	}
//}

public class WikiWebRequestCore{

		//method for faking CA authorization of HTTPS site
		private bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
			bool isOk = true;
			// If there are errors in the certificate chain, look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None) {
				for (int i=0; i<chain.ChainStatus.Length; i++) {
					if (chain.ChainStatus [i].Status != X509ChainStatusFlags.RevocationStatusUnknown) {
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build ((X509Certificate2)certificate);
						if (!chainIsValid) {
							isOk = false;
						}
					}
				}
			}
			return isOk;
		}

		private static string web_request_linked_pages(int pageid, int count){
			//TODO: use 'count' to determine how many pages much be pulled from Wikipedia by continuing request
			string api = "http://en.wikipedia.org/w/api.php?";
			string request_linked_pages = "action=query&generator=links&format=json&pageids=";
			string wiki_request = api + request_linked_pages + pageid.ToString ();
			ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
			//Make HTTP request
			WebRequest request = WebRequest.Create (wiki_request);
			//Receive response
			WebResponse response = request.GetResponse ();
			Stream dataStream = response.GetResponseStream ();
			StreamReader reader = new StreamReader (dataStream);
			string responseFromServer = reader.ReadToEnd ();
			reader.Close ();
			response.Close ();
			//return JSON response string:
			return responseFromServer;
		}

		private static string web_request_page_info(string pageid){
			string api = "http://en.wikipedia.org/w/api.php?";
			string request_page_info = "action=query&prop=info&format=json&pageids=";
			string wiki_request = api + request_page_info + pageid;
			//hack around - validates some form of CA certificate required for the HTTPS communication
			ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
			//Make HTTP request
			WebRequest request = WebRequest.Create (wiki_request);
			//Receive response
			WebResponse response = request.GetResponse ();
			Stream dataStream = response.GetResponseStream ();
			StreamReader reader = new StreamReader (dataStream);
			string responseFromServer = reader.ReadToEnd ();
			reader.Close ();
			response.Close ();
			//return JSON response string
			return responseFromServer;
		}

		private static List<String> get_dict_keys(JSONNode dict){
			List<String> key_list = new List<String> ();
			//Iterate over the dictionary, take out the keys:
			foreach (KeyValuePair<string, JSONNode> n in (JSONClass)dict){
				key_list.Add (n.Key);
			}
			return key_list;
		}

		private static JSONNode get_linked_page_dict (String jsonString){
		//Convert JSON response from string to List<int>
			JSONNode page_json = JSON.Parse(jsonString);
			List<String> tags = new List<String> ();
			tags.Add ("query");
			tags.Add ("pages");
			for (int i=0; i <=1; i++) {
				page_json = (page_json [tags [i]]);
			}
			return page_json;
		}
		
		public static List<String> get_linked_page_id_list(int pageid){
			int count = 1; //TODO: make it depend on count
			string responseJson = web_request_linked_pages (pageid, count);
			JSONNode page_json = get_linked_page_dict (responseJson);
			return get_dict_keys (page_json);
		}

		public static JSONNode get_page_info(int pageid){
			string jsonString = web_request_page_info(pageid.ToString());
			JSONNode page_json = JSON.Parse(jsonString);
			//List<String> tags = new List<String> ();
			//tags.Add ("query");
			//tags.Add ("pages");
			//tags.Add (pageid.ToString ());
			//for (int i = 0; i <= 2; i++) {
			//	page = (page [tags [i]]);
			//}
			return page_json["query"]["pages"];
		}

		public static JSONNode get_page_info(List<int> pageids){
			
			JSONClass response = new JSONClass();
			JSONNode page_json = new JSONNode();
			string pageid_string = "";
			string jsonString = "";

			for (int i = 0; i<pageids.Count; i++){
				if (i % 50 == 0 & i > 0){
					jsonString = web_request_page_info(pageid_string);
					page_json = (JSON.Parse(jsonString))["query"]["pages"];
					foreach (KeyValuePair<string, JSONNode> entry in (JSONClass)page_json){
						response.Add(entry.Key,entry.Value);
					}
					pageid_string = "";
				}

				pageid_string = pageid_string + "|" + pageids[i];
			}

			//Request last set of infos:
			jsonString = web_request_page_info(pageid_string);
			page_json = (JSON.Parse(jsonString))["query"]["pages"];
			
			foreach (KeyValuePair<string, JSONNode> entry in (JSONClass)page_json){
				response.Add(entry.Key,entry.Value);
			}

			return (JSONNode)response;

		}
		//public static void Main (){
		//	int pageid = 736;
		//	List<String> linked_pages = WikiWebRequestCore.get_linked_page_id_list (pageid);
		//	foreach (String page in linked_pages) {
		//		Console.WriteLine ("Linked page id: " + page);
		//	}
		//}
	}
}