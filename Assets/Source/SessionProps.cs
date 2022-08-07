using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Fusion;

public enum PlayMode { CaptureTheFlag, DeathMatch, KingOfTheHill}

[Serializable]
public class SessionProps
{
	public string RoomName = "AutoSession";
	public int PlayerLimit = 2;
	public PlayMode PlayMode;
	public MapIndex StartMap;
	public bool SkipStaging;

	/// <summary>
	/// Support code that allow conversion of the above fields to and from the SessionProperty dictionary needed by Fusion
	/// </summary>

	public SessionProps()
	{
	}

	public SessionProps(ReadOnlyDictionary<string,SessionProperty> props)
	{
		foreach (FieldInfo field in GetType().GetFields())
		{
			field.SetValue(this, ConvertFromSessionProp(props[field.Name], field.FieldType));
		}
	}

	public Dictionary<string, SessionProperty> Properties
	{
		get
		{
			Dictionary<string, SessionProperty> props = new Dictionary<string, SessionProperty>();
			foreach (FieldInfo field in GetType().GetFields())
			{
				props[field.Name] = ConvertToSessionProp(field.GetValue(this));
			}
			return props;
		}
	}
	
	private object ConvertFromSessionProp(SessionProperty sp, Type toType)
	{
		if (toType == typeof(bool))
			return (int) sp == 1;
		if (sp.IsString)
			return (string) sp;
		return (int) sp;
	}

	private SessionProperty ConvertToSessionProp(object value)
	{
		if (value is string)
			return SessionProperty.Convert(value);
		if(value is bool b)
			return b ? 1 : 0;
		return (int) value;
	}
}