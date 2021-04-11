using UnityEngine;
using UnityEngine.UI;

using S = System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Extensions
{
	#region Generic
	public delegate T Conversion<T, S>(S source);
	public static string ToHexString(this byte[] me)
	{
		return System.BitConverter.ToString(me).Replace("-", "");
	}
	#endregion
	#region Dictionary
	public delegate bool DictionaryComparer<T, S>(T key, S value);
	public static Dictionary<T, S> Clone<T, S>(this Dictionary<T, S> me)
	{
		Dictionary<T, S> clone = new Dictionary<T, S>();
		foreach(KeyValuePair<T, S> entry in me)
			clone.Add(entry.Key, entry.Value);
		return clone;
	}
	public static void RemoveAll<T, S>(this Dictionary<T, S> me, DictionaryComparer<T, S> comparer)
	{
		List<T> keysToRemove = new List<T>();
		foreach(KeyValuePair<T, S> entry in me)
			if(comparer(entry.Key, entry.Value))
				keysToRemove.Add(entry.Key);
		keysToRemove.ForEach(o => me.Remove(o));
	}
	public static void FromFile<T, S>(this Dictionary<T, S> me, string path, Conversion<T, string> keyConversion, Conversion<S, string> valueConversion)
	{
		if(!File.Exists(path))	return;
		me.RemoveAll((k, v) => true);
		string[] lines = File.ReadAllText(path).Split('\n'), parts;
		foreach(string line in lines)
		{
			parts = line.Split('\t');
			if(parts.Length < 2)	continue;
			try{	me.Add(keyConversion(parts[0]), valueConversion(parts[1]));	}catch{}
		}
	}
	public static void ToFile<T, S>(this Dictionary<T, S> me, string path)
	{
		try
		{
			string file = "";
			foreach(KeyValuePair<T, S> entry in me)
				file += entry.Key.ToString() + '\t' + entry.Value.ToString() + '\n';
			File.WriteAllText(path, file.Trim());
		}
		catch
		{
			#if UNITY_EDITOR || DEBUG
			Debug.LogError("Unable to write to file\n" + path);
			#endif
		}
	}
	#endregion
	#region Integer
	public static string ToCostString(this int me)
	{
		string retval = S.String.Format("{0:N}", me);
		retval = retval.Substring(0, retval.LastIndexOf('.')).Replace(",", ".");
		return retval;
	}
	public static int CostStringToInt(this string me)
	{
		return int.Parse(Regex.Replace(me, @"[.,]+", ""));
	}
	#endregion
	#region IEnumerable<T>
	public static bool ContainsAll<T>(this IEnumerable<T> me, IEnumerable<T> elements)
	{
		bool currentFound = false;
		foreach(T t in elements)
		{
			foreach(T t0 in me)
			{
				if((currentFound |= t.Equals(t0)) == true)
					break;
			}
			if(currentFound == false)
				return false;
		}
		return true;
	}
	#endregion
}
