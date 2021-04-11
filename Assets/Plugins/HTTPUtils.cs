using UnityEngine;
using UnityEngine.UI;

using S = System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Linq;

public static class HTTP
{
	public static string HMACSHA1_Hash(string input, string key)
	{
		HMACSHA1 enc = new HMACSHA1(Encoding.UTF8.GetBytes(key));
		MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
		return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(enc.ComputeHash(stream).ToHexString().ToLower()));
	}
	//	GET
	public static string HTTPGET(string url, NameValueCollection headersEnrichment = null)
	{
		WebClient client = new WebClient();
		if(headersEnrichment != null && headersEnrichment.Count > 0)
			for(int i = 0; i < headersEnrichment.Count; i++) client.Headers.Add(headersEnrichment.GetKey(i), headersEnrichment.Get(headersEnrichment.GetKey(i)));
		return client.DownloadString(url);
	}
	public static void HTTPGETAsync(string url, System.Action<string> callback)
	{
		new Thread(() => { callback(HTTPGET(url)); }).Start();
	}
	//	POST
	public static string HTTPPOST(string url, NameValueCollection nameValuePairs)
	{
		return Encoding.UTF8.GetString(new WebClient().UploadValues(url, "POST", nameValuePairs));
	}
	public static void HTTPPOSTAsync(string url, NameValueCollection nameValuePairs, System.Action<string> callback)
	{
		new Thread(() => { callback(HTTPPOST(url, nameValuePairs)); }).Start();
	}
	public static string HTTPPOSTData(string url, string data, out WebHeaderCollection headers, string contentType = "text/plain", NameValueCollection headersEnrichment = null)
	{
		return HTTPPOSTData(url, Encoding.UTF8.GetBytes(data), out headers, contentType, headersEnrichment);
	}
	public static string HTTPPOSTData(string url, byte[] data, out WebHeaderCollection headers, string contentType = "text/plain", NameValueCollection headersEnrichment = null)
	{
		WebClient client = new WebClient();
		var servicePoint = ServicePointManager.FindServicePoint(new System.Uri(url));
		servicePoint.Expect100Continue = false;

		#if DEBUG
		string hds = "";
		try
		{
			for(int i = 0; i < headersEnrichment.Count; i++)
				hds += headersEnrichment.Keys[i] + ": " + headersEnrichment.Get(headersEnrichment.Keys[i]) + "\n";
		}catch{}
		Debug.Log("URL:\n" + url + "\n\nData:\n" + Encoding.UTF8.GetString(data) + "\n\nRequest headers:\n" + hds);
		#endif
		client.Headers.Add(HttpRequestHeader.KeepAlive, "true");
		client.Headers.Add("Content-Type", contentType);

		if(headersEnrichment != null && headersEnrichment.Count > 0)
			for(int i = 0; i < headersEnrichment.Count; i++)
				client.Headers.Add(headersEnrichment.GetKey(i), headersEnrichment.Get(headersEnrichment.GetKey(i)));
		string retval = "";
		try
		{
			#if DEBUG
			Debug.Log("Request header:\n" + client.Headers);
			Debug.Log("Request content:\n" + Encoding.UTF8.GetString(data));
			#endif
			retval = Encoding.UTF8.GetString(client.UploadData(url, "POST", data));
		}
		catch(WebException wex)
		{
			#if DEBUG
			Debug.Log("retval? " + retval + ": " + wex);
			#endif
			throw new WebException(wex.Message, wex);
		}
		catch
		#if DEBUG
		(S.Exception ex)
		#endif
		{
			#if DEBUG
			Debug.Log(ex.Message);
			#endif
			headers = new WebHeaderCollection();
			return null;
		}
		headers = client.ResponseHeaders;
		return retval;
	}
}
