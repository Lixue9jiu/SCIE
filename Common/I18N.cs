using Engine;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game
{
#if DEBUG
	public enum Strings
	{
		Coalpowder_Desc,
		CokeCoal_Desc,
		Cylinder_Desc,
		LPG_Desc,
		LNG_Desc,
		He_Desc,
		Ar_Desc,
		CrudeSalt_Desc,
		RefinedSalt_Desc,
		Yeast_Desc,
		Alum_Desc,
		QuartzPowder_Desc,
		Lichenin_Desc,
		MonoSi_Desc,
		PolySi_Desc,
		Resistor_Desc,
		BasicMachineCase_Desc,
		SecondryMachineCase_Desc,
		FireBrickWall_Desc,
		_Desc,
		Drill_Desc,
		Tubularis_Desc,
		Fridge_Desc,
		Magnetizer_Desc,
		Separator_Desc,
		Battery_Desc,
		Wire_Desc,
		Fan_Desc,
		SteamBoat_Desc,
		Train_Desc,
		Carriage_Desc,
		Airship_Desc,
		RifleBarrel_Desc,
	}
#endif
	public static partial class Utils
	{
#if DEBUG
		public static string[] TR;
		/// <summary>
		/// 读取键值对文件
		/// </summary>
		/// <param name="dict"></param>
		/// <param name="stream">要读取的流</param>
		/// <param name="separator">分隔符</param>
		/// <param name="commentchar">注释符</param>
		public static void ReadKeyValueFile(Dictionary<string, string> dict, Stream stream, char separator = '=', char commentchar = '#')
		{
			var reader = new StreamReader(stream);
			while (true)
			{
				var line = reader.ReadLine();
				if (line == null) return;
				if (line[0] != commentchar)
				{
					int i = line.IndexOf(separator);
					if (i >= 0)
						dict[line.Substring(0, i)] = line.Substring(i + 1);
				}
			}
		}
		public static void ReadLanguageFile()
		{
			var d = new Dictionary<string, string>();
			for (var enumerator = ModsManager.GetEntries(".lng").GetEnumerator(); enumerator.MoveNext();)
				ReadKeyValueFile(d, enumerator.Current.Stream);
			TR = new string[d.Count];
			var type = typeof(Strings);
			for (var enumerator = d.GetEnumerator(); enumerator.MoveNext();)
			{
				var current = enumerator.Current;
				TR[(int)Enum.Parse(type, current.Key, false)] = current.Value;
			}
		}
		public static string Get(Strings s)
		{
			return TR[(int)s];
		}
#endif
		public static Stream GetTargetFile(string name, bool throwIfNotFound = true)
		{
			for (var enumerator = ModsManager.GetEntries(Storage.GetExtension(name)).GetEnumerator(); enumerator.MoveNext();)
				if (string.Equals(Path.GetFileName(enumerator.Current.Filename), name, StringComparison.OrdinalIgnoreCase))
					return enumerator.Current.Stream;
			if (throwIfNotFound)
				throw new InvalidOperationException(name + " not found.");
			return null;
		}
	}
}