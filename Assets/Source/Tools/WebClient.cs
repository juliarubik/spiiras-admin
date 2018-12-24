using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class WebClient
{
	//query {
	//	monitor(id:9) { /* здесь идентификатор монитора, он передаётся по ссылке с QR-кода */
	//		currentMedia, /* если хотим запросить ID активного слайда */
	//		medias { /* если хотим запросить инфо о слайдах, которые воспроизведены или планируются на мониторе */
	//			id, /* идентификатор слайда */
	//			name /*название слайда */
	//		}
	//	}
	//}

	public enum RequestType { GET, POST, PUT, DELETE};

	private UnityWebRequest webRequest;
	private string url;

	public WebClient(string u, RequestType requestType)
	{
		url = u;

		switch(requestType)
		{
			case RequestType.GET:
			{
				webRequest = UnityWebRequest.Get(url);
			}
			break;

			case RequestType.POST:
			{
				webRequest = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
			}
			break;

			case RequestType.PUT:
			{
				webRequest = UnityWebRequest.Put(url, UnityWebRequest.kHttpVerbPUT);
			}
			break;

			case RequestType.DELETE:
			{
				webRequest = UnityWebRequest.Delete(url);
			}
			break;
		}
	}

	// HTTP запрос к серверу Graph QL
	public IEnumerator requestQL(string query, object vars, System.Action<bool, string> callback)
	{
		using(UnityWebRequest webRequest = buildQLQuery(query))
		{
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			yield return webRequest.SendWebRequest();

			if(webRequest.isDone)
			{
				string responseString = webRequest.downloadHandler.text;
				Debug.Log("WebClient response " + responseString);
				callback(true, responseString);
			}
			else if(webRequest.isNetworkError)
			{
				string error = "NETWORK error " + webRequest.error;
				Debug.Log("WebClient " + error);
				callback(false, error);
			}
			else if(webRequest.isHttpError)
			{
				string error = "NETWORK error " + webRequest.error;
				Debug.Log("WebClient " + error);
				callback(false, error);
			}
		}
	}

	// Http запрос к серверу JSON
	public IEnumerator requestJSON(JSONObject json, System.Action<bool, string> callback)
	{
		using(UnityWebRequest webRequest = buildJSONQuery(json))
		{
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			yield return webRequest.SendWebRequest();

			if(webRequest.isDone)
			{
				string responseString = Utils.NA;

				if(webRequest.downloadHandler != null)
				{
					responseString = webRequest.downloadHandler.text;
				}

				Debug.Log("WebClient response " + responseString);
				callback(true, responseString);
			}
			else if(webRequest.isNetworkError)
			{
				string error = "NETWORK error " + webRequest.error;
				Debug.Log("WebClient " + error);
				callback(false, error);
			}
			else if(webRequest.isHttpError)
			{
				string error = "NETWORK error " + webRequest.error;
				Debug.Log("WebClient " + error);
				callback(false, error);
			}
		}
	}

	private UnityWebRequest buildJSONQuery(JSONObject json)
	{
		if(json != null)
		{
			//Debug.Log("WebClient data " + json.ToString());
			System.Text.UTF8Encoding encodingUnicode = new System.Text.UTF8Encoding();
			byte[] payload = encodingUnicode.GetBytes(json.ToString());
			UploadHandler data = new UploadHandlerRaw(payload);
			webRequest.uploadHandler = data;
		}

		webRequest.chunkedTransfer = false;
		webRequest.SetRequestHeader("Content-Type", "application/json");
		return webRequest;
	}

	private UnityWebRequest buildQLQuery(string query)
	{
		if(query != null)
		{
			var graphQuery = new GraphQLQuery();
			graphQuery.query = query;
			graphQuery.variables = "";

			string json = JsonUtility.ToJson(graphQuery);

			System.Text.UTF8Encoding encodingUnicode = new System.Text.UTF8Encoding();
			byte[] payload = encodingUnicode.GetBytes(json);
			UploadHandler data = new UploadHandlerRaw(payload);
			webRequest.uploadHandler = data;
		}
		
		webRequest.chunkedTransfer = false;
		webRequest.SetRequestHeader("Content-Type", "application/json");
		return webRequest;
	}

	// Шифрование\Дешифрование строк в запросе
	private string encrypt(string src)
	{
		return src;
	}

	private string decrypt(string src)
	{
		return src;
	}

	[System.Serializable]
	private class GraphQLQuery
	{
		public string query = "";
		public object variables { get; set; }
	}
}