﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LabelsList 
{
	private static LabelsList instance;

	private const string PREF_NAME = "labelsName";
	private static Dictionary<string, JSONObject> labelsMap; 

	public static LabelsList self
	{
		get
		{
			if(instance == null)
			{
				instance = new LabelsList();
			}
			return instance;
		}
	}

	private LabelsList()
	{
		labelsMap = new Dictionary<string, JSONObject>();
	}

	/* [Сохранение и чтение списка локально]
	public static Dictionary<string, JSONObject> readLabels()
	{
		string prefName = PlayerPrefs.GetString(PREF_NAME);
		return Utils.stringToDic(prefName);
	}

	public static void sync()
	{
		PlayerPrefs.SetString(PREF_NAME, Utils.dicToString(labelsMap));
	}
	*/

	public void clear()
	{
		labelsMap.Clear();
	}

	public int size()
	{
		return labelsMap.Count;
	}

	public string getLabelTitle(int index)
	{
		if(index < labelsMap.Count)
		{
			return labelsMap.Keys.ElementAt(index);
		}

		return null;
	}

	public JSONObject getLabel(string key)
	{
		if(labelsMap.ContainsKey(key))
		{
			return labelsMap[key];
		}

		return null;
	}

	public JSONObject findLabelById(int id)
	{
		for(int i = 0 ; i < size(); i++)
		{
			JSONObject item = labelsMap[getLabelTitle(i)];

			if(item.GetField(Utils.JSON_ID).i == id)
			{
				return item;				
			}
		}

		return null;
	}

	public JSONObject initLabel(string name, string location, string message)
	{
		JSONObject json = new JSONObject();
		json.SetField(Utils.JSON_NAME, Utils.fixInputString(name));
		json.SetField(Utils.JSON_LOCATION, location);
		// Чтобы различать метки при чтении из QR кода
		json.SetField(Utils.JSON_TYPE, "label");
		json.SetField(Utils.JSON_MESSAGE, Utils.fixInputString(message));

		update(name, json);
		return json;
	}

	public void update(string key, JSONObject obj)
	{
		if(labelsMap.ContainsKey(key))
		{
			labelsMap.Remove(key);
		}

		labelsMap.Add(key, obj);
	}
}
