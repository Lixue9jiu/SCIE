using Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Game
{
	public static partial class Utils
	{
		public static readonly string[] Names =
		{
			"自动跳跃强度",
			"白天避开火范围",
			"夜晚避开火范围",
			"白天避开玩家范围",
			"夜晚避开玩家范围",
			"宽度",
			"身高",
			"厚度",
			"质量",
			"密度",
			"水平空气阻力",
			"垂直空气阻力",
			"水平水流阻力",
			"垂直水流阻力",
			"水中摇摆角度",
			"水中转速",
			"最大流畅上升高度",
			"白天追逐范围",
			"夜晚追逐范围",
			"白天追逐时间",
			"夜晚追逐时间",
			"自动追逐标记",
			"追逐非玩家概率",
			"被攻击后追逐概率",
			"接触时追逐概率",
			"最大挖掘深度",
			"肉喜好系数",
			"鱼喜好系数",
			"水果喜好系数",
			"草喜好系数",
			"面包喜好系数",
			"白天寻找玩家范围",
			"夜晚寻找玩家范围",
			"灼眼颜色",
			"攻击抗性",
			"摔落抗性",
			"燃烧抗性",
			"可搁浅",
			"空气容量",
			" 牧群范围",
			"加速度系数",
			"行走速度",
			"爬梯速度",
			"起跳速度",
			"创造模式飞行速度",
			"飞行速度",
			"游泳速度",
			"转弯速度",
			"观察速度",
			"空气中行走系数",
			"转弯时行走速度",
			"自动观察水平",
			"攻击力",
			"漫反射颜色",
			"不透明度",
			"下蛋频率",
			"掉落最小数量",
			"掉落最大数量",
			"掉落概率",
			"烧死掉落最小数量",
			"烧死掉落最大数量",
			"烧死掉落概率",
			"变身概率",
			"倔强概率",
			"产奶时间",
			"攀爬速度",
			"挖掘速度",
			"放火概率"
		};
		public static readonly string[] Ids =
		{
			"Fe",
			"Au",
			"Ag",
			"Pb",
			"pt",
			"Zn",
			"Sn",
			"Cr",
			"Ti",
			"Ni",
			"Al",
			"U238",
			"P",
			"Fe",
			"Cu",
			"Hg",
			"Ge"
		};
		public static readonly string[] Strings =
		{
			"钢",
			"金",
			"银",
			"铅",
			"铂",
			"锌",
			"锡",
			"铬",
			"钛",
			"镍",
			"铝",
			"铀",
			"磷",
			"铁",
			"铜",
			"汞",
			"锗",
			"Fe-Al-Cr合金",
            "黄铜",
			"塑料",
		};
		public static Dictionary<string, string> TR;
		/// <summary>
		/// 读取键值对文件
		/// </summary>
		/// <param name="dict"></param>
		/// <param name="stream">要读取的流</param>
		/// <param name="separator">分隔符</param>
		/// <param name="commentchar">注释符</param>
		public static void ReadKeyValueFile(Dictionary<string, string> dict, StreamReader reader, char separator = '=', char commentchar = '#')
		{
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
		[MethodImpl((MethodImplOptions)0x100)]
		public static string Get(string s) => s != null && TR.TryGetValue(s, out string result) ? result : s;
		public static Stream GetTargetFile(string name, bool throwIfNotFound = true)
		{
			for (var enumerator = ModsManager.GetEntries(Storage.GetExtension(name)).GetEnumerator(); enumerator.MoveNext();)
				if (string.Equals(Path.GetFileName(enumerator.Current.Filename), name, StringComparison.OrdinalIgnoreCase))
					return enumerator.Current.Stream;
			if (throwIfNotFound)
				throw new InvalidOperationException(name + " not found.");
			return null;
		}
		[MethodImpl((MethodImplOptions)0x100)]
		public static string ToStr(this Materials m)
		{
			return Strings[(int)m];
		}
		[MethodImpl((MethodImplOptions)0x100)]
		public static string ToId(this Materials m)
		{
			return Ids[(int)m];
		}
	}
}