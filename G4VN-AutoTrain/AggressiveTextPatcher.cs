using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;

namespace CrackVLBS
{
	/// <summary>
	/// 激进文本补丁类 - 通过持续扫描和替换窗口文本来实现翻译
	/// </summary>
	internal class AggressiveTextPatcher
	{
		// 越南文到中文的翻译映射表（包含乱码形式）
		private static readonly Dictionary<string, string> TranslationMap;
		
		// 未翻译文本收集器（用于一次性输出）
		private static readonly HashSet<string> UntranslatedTexts = new HashSet<string>();
		private static readonly object UntranslatedTextsLock = new object();
		
		// 静态构造函数，用于初始化并输出日志
		static AggressiveTextPatcher()
		{
			try
			{
				Console.WriteLine("AggressiveTextPatcher: Starting TranslationMap initialization...");
				TranslationMap = new Dictionary<string, string>();
				
				// 使用ContainsKey检查逐个添加，避免重复键导致整个字典初始化失败
				Action<string, string> addKey = (key, value) =>
				{
					if (TranslationMap.ContainsKey(key))
					{
						Console.WriteLine("AggressiveTextPatcher: WARNING - Duplicate key detected: '" + key + "' (existing value: '" + TranslationMap[key] + "', new value: '" + value + "')");
					}
					else
					{
						TranslationMap.Add(key, value);
					}
				};
				
				// 添加所有翻译映射
				{
			// 标签页
			addKey("Tổng hợp", "综合");
			addKey("Đồ tẩu", "挂机");
			addKey("Thống kê", "统计");
			addKey("Bang hội", "帮会");
			addKey("Kỹ năng", "技能");
			addKey("Lọc đồ", "筛选");
			addKey("Phụ trợ", "辅助");
			
			// 功能选项（正常文本）
			addKey("Luyện công", "练功");
			addKey("Nhiệm vụ Dã tẩu", "日常任务");
			addKey("PT Nhiệm vụ", "PT任务");
			addKey("PT Nhóm", "PT任务");
			addKey("Túi vô", "背包");
			addKey("Tự vệ", "背包");
			addKey("Túi xếp cá", "鱼袋");
			addKey("Tự xếp đồ", "鱼袋");
			addKey("Nhặt trong thành", "城内拾取");
			addKey("Không nhặt vật phẩm (đen)", "不拾取物品(黑名单)");
			addKey("PT Tết", "春节PT");
			addKey("PT Tất cả", "春节PT");
			addKey("Danh vọng", "声望");
			addKey("Tín sứ", "信使");
			addKey("Ẩn Game", "隐藏游戏");
			addKey("Reset (>3h)", "重置(>3小时)");
			
			// 功能选项（乱码文本 - 问号形式）
			addKey("Luy?n c?ng", "练功");
			addKey("PT Nh?m", "PT任务");
			addKey("PT Têt c?", "春节PT");
			addKey("Tù v?", "背包");
			addKey("èn Game", "隐藏游戏");
			
			// 按钮
			addKey("Hành trang", "行囊");
			addKey("Tìm", "查找");
			addKey("Đăng ký", "注册");
			
			// 输入框标签
			addKey("Thoát Game", "退出游戏");
			addKey("Tắt máy", "关闭电脑");
			addKey("Thoát Game (hh:mm)", "退出游戏 (时:分)");
			addKey("Tắt máy (hh:mm)", "关闭电脑 (时:分)");
			
			// 统计信息
			addKey("Về thành", "回城");
			addKey("Tử vong", "死亡");
			addKey("Thu nhặt", "拾取");
			addKey("Thu nhập", "拾取");
			addKey("Dã tẩu", "挂机");
			addKey("Nhiệm vụ", "任务");
			
			// 表格标题
			addKey("Tên nhân vật", "角色名");
			addKey("Số lượng", "数量");
			addKey("Niveau", "等级");
			addKey("Level", "等级");
			addKey("S/L", "数量");
			addKey("N/L", "等级");
			
			// 根据日志添加的实际找到的文本
			addKey("Tù t×m mua trang bT, v?t phèm", "自动购买装备物品");
			addKey("Tu th?n hoμn", "自动还魂");
			addKey("Thèng ka", "统计");
			addKey("Di?t qu?i k?t h?p DT&&LC", "杀怪结合DT&&LC");
			addKey("Tù mua v?t phèm t1i TBTN", "自动购买物品到TBTN");
			addKey("Thi hμnh", "执行");
			addKey("Thi?t l?p th?ng sè...", "设置参数...");
			addKey("Tù [Giao ??] sau khi lêy LB", "领取LB后自动[交任务]");
			addKey("Tù x?p ??", "自动整理");
			addKey("Tù ??nh", "自动设定");
			
			// 从日志中提取的乱码文本（需要翻译）
			addKey("Hμnh trang", "行囊");
			addKey("Nh?t trong thμnh", "城内拾取");
			addKey("Vò thμnh", "回城");
			addKey("Vò thμnh: 0", "回城: 0");
			addKey("T? vong", "死亡");
			addKey("T? vong: 0", "死亡: 0");
			addKey("Thu nh?p", "拾取");
			addKey("Thu nh?p: 0 v1n 0 l-?ng", "拾取: 0万 0两");
			addKey("Kh?ng nh?t v?t phèm (?en)", "不拾取物品(黑名单)");
			addKey("TYn s?", "信使");
			addKey("Tín s?", "信使");
			addKey("Nhi?m v? D· tèu", "日常任务");
			addKey("Danh v?ng", "声望");
			addKey("§¨ng ky", "注册");
			addKey("§¨ng ký", "注册");
			addKey("T×m ??", "查找");
			addKey("Tho?t Game (hh:mm)", "退出游戏 (时:分)");
			addKey("T?t m?y (hh:mm)", "关闭电脑 (时:分)");
			addKey("H?ng bao", "红包");
			addKey("Méc nh?n", "木人");
			addKey("L?nh bμi ngéu nhian", "随机令牌");
			addKey("D· tèu", "挂机");
			addKey("D· tèu: 0", "挂机: 0");
			addKey("Nhi?m v?", "任务");
			addKey("Nhi?m v?:", "任务:");
			addKey("Giao ??", "交任务");
			addKey("Giao nhiệm vụ", "交任务");
			addKey("Test", "测试");
			
			// 更多乱码变体（注意：已移除所有重复的键，它们已经在上面定义过了）
			
			// 更多常见乱码变体（从日志中提取）
			addKey("Về thμnh", "回城");
			addKey("Về thμnh:", "回城:");
			addKey("Về thành:", "回城:");
			addKey("Tử vong:", "死亡:");
			addKey("Thu nhập:", "拾取:");
			addKey("Dã tẩu:", "挂机:");
			addKey("Nhiệm vụ:", "任务:");
			addKey("Nhiệm vụ Dã tẩu:", "日常任务:");
			addKey("Tên nhân vật:", "角色名:");
			addKey("Số lượng:", "数量:");
			addKey("Level:", "等级:");
			addKey("S/L:", "数量:");
			addKey("N/L:", "等级:");
			
			// 数字和单位的乱码形式
			addKey("v1n", "万");
			addKey("vạn", "万");
			addKey("l-?ng", "两");
			addKey("lượng", "两");
			addKey("0 v1n 0 l-?ng", "0万 0两");
			addKey("0 vạn 0 lượng", "0万 0两");
			
			// 其他常见文本
			addKey("Quây quái", "围怪");
			addKey("Phạm vi", "范围");
			addKey("Đánh quái trên đường đi", "路上杀怪");
			addKey("Tiếp cận", "接近");
			addKey("Đánh chiêu bên phải", "右侧攻击");
			addKey("Xuống ngựa khi đánh quái", "杀怪时下马");
			addKey("Lên ngựa khi di chuyển", "移动时上马");
			addKey("Xử lý Boss Xanh", "处理绿Boss");
			addKey("Tránh Boss Vàng", "避开黄Boss");
			addKey("Theo sau", "跟随");
			addKey("Đội trưởng", "队长");
			addKey("Bất kỳ (PT)", "任意(队伍)");
			addKey("Nhặt rác", "拾取垃圾");
			addKey("Không bỏ sót", "不遗漏");
			addKey("Toạ độ", "坐标");
			addKey("Nơi LC", "LC地点");
			addKey("Điểm nhớ", "记忆点");
			addKey("Lọc", "筛选");
			addKey("Chỉnh", "调整");
			addKey("Chiau h? tr?", "辅助技能");
			addKey("Ch?nh?", "调整");
			addKey("Mở rộng", "展开");
			addKey("Chuyển đồ (CTRL + A)", "转移物品 (CTRL + A)");
			addKey("Quăng đồ", "丢弃物品");
			addKey("Mật khẩu điều khiển", "控制密码");
			addKey("Lệnh bài can ban", "需要出售的令牌");
			addKey("Lệnh bài cần bán", "需要出售的令牌");
			addKey("Rương lấy đồ (không trả lại)", "箱子拾取物品(不归还)");
			addKey("Rương lấy đồ (trả lại)", "箱子拾取物品(归还)");
			addKey("R1", "R1");
			addKey("R2", "R2");
			addKey("R3", "R3");
			addKey("BH", "BH");
			addKey("Khi không hoàn thành nhiệm vụ", "未完成任务时");
			addKey("Khi hoàn thành 40 nhiệm vụ", "完成40个任务时");
			addKey("Tìm trang bị / Vật phẩm", "查找装备/物品");
			addKey("Tìm Đồ chí / Mật chí", "查找秘籍/密籍");
			addKey("Nâng điểm kinh nghiệm", "提升经验点");
			addKey("Nâng cấp Phúc duyên", "升级福缘");
			addKey("Nâng cần Danh vong", "提升声望");
			addKey("Thoát game khi Disc (phút)", "断线时退出游戏(分钟)");
			addKey("Tạm dừng", "暂停");
			addKey("UT Online", "UT在线");
			addKey("Tự dùng X2", "自动使用X2");
			addKey("X2/TTL/SDT (phút)", "X2/TTL/SDT(分钟)");
			addKey("Lặp lại", "重复");
			addKey("N?ng cờp HT", "升级HT");
			addKey("Luy?n HT", "练HT");
			addKey("Xo? SMS", "删除SMS");
			addKey("QUIT (ALL)", "退出(全部)");
			addKey("Đọc [TĐ+NLC]", "读取[坐标+地点]");
			addKey("Ghi [TĐ+NLC]", "保存[坐标+地点]");
			addKey("Ghi Profile [b?n ?? Dã Tấu]", "保存配置[挂机设置]");
			addKey("Huỳnh trang", "装备");
			addKey("Hùnh trang", "装备");
			addKey("Hunh trang", "装备");
			addKey("TDP S.lực thấp", "TDP内力低");
			addKey("TDP N.lực thấp", "TDP外功低");
			addKey("TDP khi tiền >=", "TDP当金钱>=");
			addKey("Rung chuông khi TDP", "TDP时响铃");
			addKey("Thoát game S.Lực thấp", "退出游戏内力低");
			addKey("Thoát game N.Lực thấp", "退出游戏外功低");
			
			// 从截图和日志中提取的更多文本（注意：已移除重复的键"Tắt máy"、"Thoát Game"、"Điểm nhớ"、"Lọc"、"Chỉnh"等，它们已经在上面定义过了）
			addKey("Di chuyển", "移动");
			addKey("Nhặt đồ", "拾取物品");
			addKey("S?a", "修复");
			addKey("Đ?c [TĐ+NLC]", "读取[TĐ+NLC]");
			addKey("Tr? giúp", "帮助");
			addKey("Xem", "查看");
			addKey("Run", "运行");
			addKey("Stop", "停止");
			addKey("Ti謨轎h 1", "标签1");
			addKey("Ti謨轎h 2", "标签2");
			addKey("Ti謨輅h 1", "标签1");
			addKey("Ti謨輅h 2", "标签2");
			addKey("Danh sách Scripts", "脚本列表");
			addKey("自动设", "自动设置");
			
			// 子标签标题栏的乱码文本（从截图提取 - 添加多种变体）
			addKey("D.T落", "挂机");
			addKey("D.t落", "挂机"); // 小写变体
			addKey("T.翰h", "统计");
			addKey("T.轎h", "统计"); // 从截图看到的实际字符
			addKey("Bang h閉", "帮会");
			addKey("?khi解", "技能");
			addKey("?khi鮮", "技能"); // 从截图看到的实际字符
			addKey("L.c珑g", "筛选");
			addKey("P.h錳", "辅助");
			addKey("P.h锰", "辅助"); // 从截图看到的实际字符
			// 注意：不添加"T."映射，因为会导致误匹配
			
			// G4VN相关文本（"回忆"已翻译过，改为其他文本）
			addKey("G4VN", "剑侠1");
			addKey("G4VN回忆", "剑侠1辅助");
			addKey("回忆", "辅助"); // 单独处理"回忆"两个字
			addKey("回忆服务器", "剑侠1辅助服务器");
			
			// 登录界面相关文本
			addKey("G4VN VLBS 1.9", "剑侠1 VLBS 1.9");
			addKey("G4VN VLBS 1.3", "剑侠1 VLBS 1.3");
			
			// 未翻译的越南文
			addKey("Nhiệm vụ Dã tẩu", "日常任务");
			addKey("Danh vọng", "声望");
			addKey("Tên nhân vật", "角色名称");
			
			// 修复错误翻译
			addKey("重置(>3小日)", "重置(>3小时)");
			addKey("关闭电脑(时:久)", "关闭电脑(时:分)");
			addKey("隐藏游", "隐藏游戏");
			
			// 下拉列表中的乱码文本（从截图提取）
			addKey("D?t落", "挂机");
			addKey("Hi謚 thu", "回城");
			addKey("T奠 ho?", "统计");
			addKey("Xa phu", "筛选");
			addKey("R ng", "技能");
			addKey("V?L---T. Nh~~", "帮会");
			addKey("D辌h Quan", "辅助");
			addKey("L?Quan", "综合");
			addKey("Ti Trang", "装备");
			addKey("Th?R薦", "声望");
			addKey("B泠 Ng鶴", "背包");
			addKey("Th蓉 B?T.Nh~~", "任务");
			addKey("C珑g th袄h quan (Th珑)", "成功完成任务");
			addKey("Long Ng?(Th)", "龙城");
			addKey("Th薊 C鯛 (BaL+g Huy謨)", "挑战副本");
			addKey("Th薊 C! (Nam Nhac Tr尋)", "挑战副本");
			addKey("V筐 S?Th珑g (Nam Nh笛 Tr蕁)", "完成挑战");
			addKey("H綾 Saung (Nam Nh笛 Tr蕁)", "完成挑战");
			
			// 从日志中提取的未翻译文本
			addKey("Tho?t game S.Lùc thêp", "退出游戏 S.等级");
			addKey("Tho?t game N.Lùc thêp", "退出游戏 N.等级");
			addKey("Nh?t ??", "拾取");
			addKey("Ch?n h?", "选择");
			addKey("§éi tr-?ng", "队伍");
			addKey("Bêt kú (PT)", "任何(PT)");
			addKey("Ti?p c?n", "接近");
			addKey("Xuèng ngùa khi ??nh qu?i", "下马当攻击");
			addKey("§?nh chiau ban ph?i", "指定目标");
			addKey("Di chuyón", "移动");
			addKey("Lan ngùa khi di chuyón", "骑马当移动");
			addKey("L?c ??", "筛选");
			addKey("§ióm nhí", "点");
			addKey("Ph1m vi", "范围");
			addKey("Nh?t r?c", "拾取垃圾");
			addKey("Kh?ng bá s?t", "不报告");
			addKey("X? ly Boss Xanh", "处理蓝Boss");
			addKey("Tr?nh Boss Vμng", "避开黄Boss");
			addKey("TDP khi sai b?n ?? luy?n c?ng", "TDP当错误时练功");
			
			// 从未翻译文本文件中提取的新映射
			addKey("B?m S.lùc m?c 1", "充内力等级1");
			addKey("B?m N.lùc m?c 1", "充外功等级1");
			addKey("TDP S.lùc thêp", "TDP内力低");
			addKey("TDP khi h?t b×nh m?u/mana", "TDP当血瓶/蓝瓶用完");
			addKey("T?t m?y", "关闭电脑");
			addKey("§?ng im (idle) trong thμnh 15 phót", "在城内静止15分钟");
			addKey("TDP khi h?t ch? trèng", "TDP当背包满");
			addKey("M? réng", "展开");
			
			// 带编号的下拉框项（从未翻译文本文件中提取）
			addKey("1. D?t落", "1. 挂机");
			addKey("1. D?t萿", "1. 挂机");
			addKey("2. Hi謚 thu", "2. 回城");
			addKey("2. Hi謚 thu鑓", "2. 回城");
			addKey("3. T奠 ho?", "3. 统计");
			addKey("3. T筽 ho?", "3. 统计");
			addKey("4. Xa phu", "4. 筛选");
			addKey("5. R ng", "5. 技能");
			addKey("5. R ng ", "5. 技能");
			addKey("6. V?L---T. Nh~~", "6. 帮会");
			addKey("6. V?L﹎ T. Nh﹏", "6. 帮会");
			addKey("7. D辌h Quan", "7. 辅助");
			addKey("8. L?Quan", "8. 综合");
			addKey("9. Ti Trang", "9. 装备");
			addKey("10. Th?R薦", "10. 声望");
			addKey("10. Th?R蘮", "10. 声望");
			addKey("11. B泠 Ng鶴", "11. 背包");
			addKey("12. Th蓉 B?T.Nh~~", "12. 任务");
			addKey("12. Th莕 B?T.Nh﹏", "12. 任务");
			addKey("13. C珑g th袄h quan (Th珑)", "13. 成功完成任务");
			addKey("14. Long Ng?(Th)", "14. 龙城");
			addKey("14. Long Ng?(Th玭)", "14. 龙城");
			addKey("15. Th薊 C鯛 (BaL+g Huy謨)", "15. 挑战副本");
			addKey("16. Th薊 C! (Nam Nhac Tr尋)", "16. 挑战副本");
			addKey("17. V筐 S?Th珑g (Nam Nh笛 Tr蕁)", "17. 完成挑战");
			addKey("17. V筺 S?Th玭g (Nam Nh筩 Tr蕁)", "17. 完成挑战");
			addKey("18. H綾 Saung (Nam Nh笛 Tr蕁)", "18. 完成挑战");
			
			// 其他未翻译的文本
			addKey("Tên nhân vật", "角色名称");
			addKey("T?do", "自由");
			addKey("Auto", "自动");
			addKey("UT.Online", "UT在线");
			addKey("UT.Offline", "UT离线");
			addKey("2. U?th竎 Online (BCH)", "2. 使用在线(BCH)");
			addKey("3. U?th竎 Offline", "3. 使用离线");
			addKey("4. Hu?nhi謒 v?", "4. 训练任务");
			addKey("0. 站立 im", "0. 静止");
			addKey("0. 站立 im", "0. 静止");
			addKey("Lêy v?t phèm khi Reset", "重置时拾取物品");
			addKey("Lêy v?t phèm", "拾取物品");
			addKey("L?nh bμi nhi?m v?", "任务令牌");
			addKey("L?c L?nh bμi", "筛选令牌");
			addKey("Lêy khi ??y Hμnh trang", "背包满时拾取");
			addKey("Ch?ch?", "选择");
			addKey("Hu?b?", "训练");
			addKey("R-?ng lêy ?? (kh?ng tr? l1i)", "箱子拾取物品(不归还)");
			addKey("Khi kh?ng hoμn thμnh nhi?m v?", "未完成任务时");
			addKey("Khi hoμn thμnh 40 nhi?m v?", "完成40个任务时");
			addKey("Da tau (mo rong)", "挂机(展开)");
			addKey("Tù d?ng X2 khi", "自动使用X2当");
			addKey("Gi÷ l1i nv TK >=", "保留任务声望>=");
			addKey("Gi÷ l1i nv KN(tri?u) >=", "保留任务技能(百万)>=");
			addKey("Cho phDp hu? chu?i NV", "允许训练任务链");
			addKey("(khi cêp < 50 && chu?i nhi?m v? < 50)", "(当等级<50 && 任务链<50)");
			addKey("Nh?n ?? D· tèu", "接受挂机");
			addKey("Cho ?? D· tèu", "给挂机");
			
			// 带编号的布局选项
			addKey("1.Lo筰 1 x 1 ?(d鋍 - ngang)", "1.布局 1x1 (竖-横)");
			addKey("2.Lo筰 2 x 1 ?(d鋍 - ngang)", "2.布局 2x1 (竖-横)");
			addKey("3.Lo筰 3 x 1 ?(d鋍 - ngang)", "3.布局 3x1 (竖-横)");
			addKey("4.Lo筰 4 x 1 ?(d鋍 - ngang)", "4.布局 4x1 (竖-横)");
			addKey("5.Lo筰 1 x 2 ?(d鋍 - ngang)", "5.布局 1x2 (竖-横)");
			addKey("6.Lo筰 2 x 2 ?(d鋍 - ngang)", "6.布局 2x2 (竖-横)");
			addKey("7.Lo筰 3 x 2 ?(d鋍 - ngang)", "7.布局 3x2 (竖-横)");
			
			// 其他带编号的选项
			addKey("1. Ti襫 &  1 ?", "1. 标签 & 1");
			addKey("2. Th猰 2 ?& v?kh?", "2. 设置 2 & 背包");
			addKey("3. Th猰  4 ?", "3. 设置 4");
			addKey("5. Nh苩 t蕋 c?", "5. 拾取物品");
			}
			
				Console.WriteLine("AggressiveTextPatcher: TranslationMap initialized successfully with " + TranslationMap.Count + " entries");
			}
			catch (Exception ex)
			{
				Console.WriteLine("AggressiveTextPatcher: ERROR initializing TranslationMap: " + ex.Message);
				Console.WriteLine("AggressiveTextPatcher: Exception type: " + ex.GetType().FullName);
				if (ex is ArgumentException argEx)
				{
					Console.WriteLine("AggressiveTextPatcher: ArgumentException details: " + argEx.Message);
				}
				Console.WriteLine("AggressiveTextPatcher: Stack trace: " + ex.StackTrace);
				TranslationMap = new Dictionary<string, string>(); // 使用空字典作为后备
			}
		}

		// API声明
		[DllImport("user32.dll")]
		private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		// TabControl相关常量和API
		private const uint TCM_GETITEMCOUNT = 0x1304;
		private const uint TCM_GETITEM = 0x1305;
		private const uint TCM_SETITEM = 0x1306;
		private const uint TCIF_TEXT = 0x0001;
		
		// ComboBox相关常量
		private const uint CB_GETCOUNT = 0x0146;
		private const uint CB_GETLBTEXT = 0x0148;
		private const uint CB_SETITEMDATA = 0x0151;
		private const uint CB_GETCURSEL = 0x0147;
		private const uint CB_SETCURSEL = 0x014E;
		private const uint CB_DELETESTRING = 0x0144;
		private const uint CB_INSERTSTRING = 0x0143;
		private const uint CB_ADDSTRING = 0x0143;
		
		// TCITEM结构体
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct TCITEM
		{
			public uint mask;
			public uint dwState;
			public uint dwStateMask;
			public IntPtr pszText;
			public int cchTextMax;
			public int iImage;
			public IntPtr lParam;
		}

		// SendMessage重载，用于ComboBox（使用StringBuilder作为lParam，用于获取文本）
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		private static extern int SendMessageCombo(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);
		
		// SendMessage重载，用于ComboBox（使用IntPtr作为lParam，用于设置文本）
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
		private static extern IntPtr SendMessageComboStr(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

		private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		/// <summary>
		/// 翻译文本
		/// </summary>
		private static string TranslateText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			// 检查TranslationMap是否已初始化
			if (TranslationMap == null)
			{
				Console.WriteLine("AggressiveTextPatcher: WARNING - TranslationMap is null!");
				return text;
			}
			
			if (TranslationMap.Count == 0)
			{
				Console.WriteLine("AggressiveTextPatcher: WARNING - TranslationMap is empty!");
				return text;
			}

			string trimmed = text.Trim();

			// 1. 精确匹配（最优先）
			if (TranslationMap.ContainsKey(trimmed))
			{
				return TranslationMap[trimmed];
			}

			// 2. 尝试匹配文本的开头部分（只匹配较长的键，至少3个字符）
			foreach (var kvp in TranslationMap)
			{
				if (kvp.Key.Length >= 3 && trimmed.StartsWith(kvp.Key))
				{
					if (trimmed.Contains(":") && !kvp.Value.Contains(":"))
					{
						return kvp.Value + ":";
					}
					return kvp.Value;
				}
			}

			// 3. 部分匹配，但要求匹配长度至少为3个字符，且匹配度超过50%
			int bestMatchLength = 0;
			string bestMatch = null;
			foreach (var kvp in TranslationMap)
			{
				// 跳过太短的键（少于3个字符）
				if (kvp.Key.Length < 3)
				{
					continue;
				}

				// 计算匹配长度
				int matchLength = 0;
				if (trimmed.Contains(kvp.Key))
				{
					matchLength = kvp.Key.Length;
				}
				else if (kvp.Key.Contains(trimmed) && trimmed.Length >= 3)
				{
					matchLength = trimmed.Length;
				}

				// 要求匹配长度至少为3，且匹配度超过50%
				if (matchLength >= 3 && matchLength >= Math.Max(trimmed.Length, kvp.Key.Length) * 0.5)
				{
					if (matchLength > bestMatchLength)
					{
						bestMatchLength = matchLength;
						bestMatch = kvp.Value;
					}
				}
			}

			if (bestMatch != null)
			{
				if (trimmed.Contains(":") && !bestMatch.Contains(":"))
				{
					return bestMatch + ":";
				}
				return bestMatch;
			}

			// 4. 尝试匹配包含冒号的文本（如 "Về thành: 0"）
			if (trimmed.Contains(":"))
			{
				string textBeforeColon = trimmed.Split(':')[0].Trim();
				if (textBeforeColon.Length >= 2)
				{
					foreach (var kvp in TranslationMap)
					{
						if (kvp.Key.Length >= 2 && (textBeforeColon.Contains(kvp.Key) || kvp.Key.Contains(textBeforeColon)))
						{
							// 提取冒号后的内容
							string suffix = "";
							int colonIndex = trimmed.IndexOf(':');
							if (colonIndex >= 0 && colonIndex < trimmed.Length - 1)
							{
								suffix = trimmed.Substring(colonIndex);
							}
							return kvp.Value + suffix;
						}
					}
				}
			}

			// 5. 如果没有找到翻译，记录未翻译文本
			if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 0)
			{
				// 跳过明显不需要翻译的文本
				if (!IsSkipText(trimmed))
				{
					lock (UntranslatedTextsLock)
					{
						UntranslatedTexts.Add(trimmed);
					}
				}
			}
			
			// 6. 如果没有找到翻译，返回原文（不要使用默认值）
			return text;
		}
		
		/// <summary>
		/// 判断是否应该跳过该文本（不需要翻译）
		/// </summary>
		private static bool IsSkipText(string text)
		{
			if (string.IsNullOrEmpty(text))
				return true;
				
			// 跳过纯数字、百分比、时间格式等
			if (System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d+\.?\d*%?$") || 
			    System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d{1,2}:\d{2}:\d{2}$") ||
			    text == "..." || text == "List1" || text == "Tab1" ||
			    text.Contains("99.99") || text.Contains("12:59"))
				return true;
				
			// 跳过已经是中文的文本（简单判断：包含中文字符）
			if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[\u4e00-\u9fa5]"))
			{
				// 但如果包含乱码字符（如"?"、"?"等），仍然需要翻译
				if (!text.Contains("?") && !text.Contains("?") && !text.Contains("?") && 
				    !text.Contains("?") && !text.Contains("?") && !text.Contains("?"))
					return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// 输出所有未翻译的文本到文件
		/// </summary>
		public static void ExportUntranslatedTexts(string filePath)
		{
			lock (UntranslatedTextsLock)
			{
				try
				{
					System.IO.File.WriteAllLines(filePath, UntranslatedTexts, Encoding.UTF8);
					Console.WriteLine("AggressiveTextPatcher: Exported " + UntranslatedTexts.Count + " untranslated texts to: " + filePath);
				}
				catch (Exception ex)
				{
					Console.WriteLine("AggressiveTextPatcher: Error exporting untranslated texts: " + ex.Message);
				}
			}
		}

		/// <summary>
		/// 启动激进文本补丁
		/// </summary>
		public static void StartAggressivePatching(Process process, int intervalMs = 1000)
		{
			if (process == null || process.HasExited)
			{
				Console.WriteLine("AggressiveTextPatcher: ERROR - Invalid process");
				return;
			}

			Console.WriteLine("AggressiveTextPatcher: Starting aggressive text patching for PID=" + process.Id);
			Console.WriteLine("AggressiveTextPatcher: TranslationMap.Count=" + (TranslationMap != null ? TranslationMap.Count.ToString() : "null"));

			Thread patchThread = new Thread(() =>
			{
				// 等待窗口加载
				Thread.Sleep(5000);
				Console.WriteLine("AggressiveTextPatcher: Starting aggressive patching loop");

				int cycleCount = 0;
				while (!process.HasExited)
				{
					try
					{
						int patchedCount = 0;
						
						// 每50个周期导出一次未翻译文本
						if (cycleCount > 0 && cycleCount % 50 == 0)
						{
							string exportPath = System.IO.Path.Combine(
								System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
								"untranslated_texts_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
							ExportUntranslatedTexts(exportPath);
						}

						// 枚举目标进程的所有窗口（包括Dialog窗口）
						EnumWindows((hWnd, lParam) =>
						{
							try
							{
								uint processId;
								GetWindowThreadProcessId(hWnd, out processId);

								// 只处理目标进程的窗口（包括不可见的Dialog窗口）
								if (processId == process.Id)
								{
									// 获取窗口类名，检查是否是Dialog
									StringBuilder classNameSb = new StringBuilder(256);
									GetClassName(hWnd, classNameSb, classNameSb.Capacity);
									string className = classNameSb.ToString();
									
									// Dialog窗口类名通常是"#32770"
									bool isDialog = (className == "#32770" || className.Contains("Dialog"));
									
									// 特殊处理：ComboBox和TabControl需要在主窗口级别也处理
									if (className == "ComboBox" || className == "ComboBoxEx32")
									{
										ProcessComboBox(hWnd, ref patchedCount);
									}
									else if (className == "SysTabControl32")
									{
										ProcessTabControl(hWnd, ref patchedCount);
									}
									
									// 处理可见窗口或Dialog窗口
									if (IsWindowVisible(hWnd) || isDialog)
									{
										// 获取窗口文本
										StringBuilder sb = new StringBuilder(1024);
										ApiHelper.GetWindowText(hWnd, sb, sb.Capacity);
										string windowText = sb.ToString();

										if (!string.IsNullOrEmpty(windowText) && windowText.Trim().Length > 0)
										{
											// 尝试翻译
											string translated = TranslateText(windowText);
											
											// 对于Dialog窗口，增加调试输出
											if (isDialog || (cycleCount < 5 && !string.IsNullOrEmpty(windowText)))
											{
												Console.WriteLine("AggressiveTextPatcher: Text: '" + windowText + "' -> '" + translated + "' (Class=" + className + (isDialog ? ", Dialog" : "") + ")");
											}
											
											// 检查是否是误翻译为"综合"
											if (translated == "综合" && !windowText.Contains("Tổng hợp") && !windowText.Contains("Tổng") && windowText.Length > 3)
											{
												// 跳过这个翻译，避免误翻译
												translated = windowText; // 不翻译，保持原文
											}
											
											if (translated != windowText)
											{
												// 保存控件的原始启用状态
												bool wasEnabled = ApiHelper.IsWindowEnabled(hWnd);
												
												// 替换文本
												ApiHelper.SetWindowText(hWnd, translated);
												
												// 确保控件保持启用状态（如果原来是启用的）
												if (wasEnabled)
												{
													// 立即检查并恢复启用状态
													if (!ApiHelper.IsWindowEnabled(hWnd))
													{
														ApiHelper.EnableWindow(hWnd, true);
													}
													// 使用SendMessage发送WM_ENABLE消息，确保启用状态被应用
													// WM_ENABLE = 0x000A, wParam=1表示启用
													ApiHelper.SendMessage(hWnd, 0x000AU, new IntPtr(1), IntPtr.Zero);
												}
												
												patchedCount++;
												
												// 输出所有补丁信息（用于调试）
												Console.WriteLine("AggressiveTextPatcher: Patched '" + windowText + "' -> '" + translated + "' (HWND=" + hWnd + ")");
											}
										}

										// 递归处理子窗口（包括Dialog的子控件）
										PatchChildWindows(hWnd, ref patchedCount, cycleCount);
									}
								}
							}
							catch
							{
								// 忽略错误，继续枚举
							}

							return true; // 继续枚举
						}, IntPtr.Zero);

						cycleCount++;
						if (patchedCount > 0 && cycleCount % 10 == 0)
						{
							Console.WriteLine("AggressiveTextPatcher: Cycle " + cycleCount + ", patched " + patchedCount + " windows");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("AggressiveTextPatcher Error: " + ex.Message);
					}

					Thread.Sleep(intervalMs);
				}

				Console.WriteLine("AggressiveTextPatcher: Patching ended");
			})
			{
				IsBackground = true
			};
			patchThread.Start();
		}

		/// <summary>
		/// 处理ComboBox控件
		/// </summary>
		private static void ProcessComboBox(IntPtr hWnd, ref int patchedCount)
		{
			try
			{
				// 获取当前选中的项
				int currentSel = (int)ApiHelper.SendMessage(hWnd, CB_GETCURSEL, IntPtr.Zero, IntPtr.Zero);
				
				// 获取下拉项数量
				int itemCount = (int)ApiHelper.SendMessage(hWnd, CB_GETCOUNT, IntPtr.Zero, IntPtr.Zero);
				if (itemCount > 0 && itemCount < 100) // 限制数量
				{
					// 从后往前处理，避免索引变化
					for (int i = itemCount - 1; i >= 0; i--)
					{
						try
						{
							// 获取下拉项文本
							StringBuilder comboTextSb = new StringBuilder(512);
							int textLen = SendMessageCombo(hWnd, CB_GETLBTEXT, new IntPtr(i), comboTextSb);
							if (textLen > 0 && textLen < comboTextSb.Capacity)
							{
								string comboText = comboTextSb.ToString();
								if (!string.IsNullOrEmpty(comboText) && comboText.Trim().Length > 0)
								{
									// 检查是否是文件路径（如果包含路径分隔符，可能是文件名）
									bool isFilePath = comboText.Contains("\\") || comboText.Contains("/");
									
									// 尝试翻译下拉项文本
									string translatedCombo = TranslateText(comboText);
									if (translatedCombo != comboText && !isFilePath)
									{
										Console.WriteLine("AggressiveTextPatcher: Found ComboBox item text: '" + comboText + "' -> '" + translatedCombo + "' (index=" + i + ", HWND=" + hWnd + ")");
										
										// 删除原项并插入翻译后的项
										ApiHelper.SendMessage(hWnd, CB_DELETESTRING, new IntPtr(i), IntPtr.Zero);
										
										// 使用 Marshal.StringToHGlobalAuto 分配内存
										IntPtr translatedPtr = Marshal.StringToHGlobalAuto(translatedCombo);
										try
										{
											IntPtr newIndexPtr = SendMessageComboStr(hWnd, CB_INSERTSTRING, i, translatedPtr);
											int newIndex = newIndexPtr.ToInt32();
											
											// 如果删除的是当前选中项，恢复选中状态
											if (i == currentSel && newIndex >= 0)
											{
												ApiHelper.SendMessage(hWnd, CB_SETCURSEL, new IntPtr(newIndex), IntPtr.Zero);
											}
											
											if (newIndex >= 0)
											{
												Console.WriteLine("AggressiveTextPatcher: Successfully patched ComboBox item " + i + ": '" + comboText + "' -> '" + translatedCombo + "'");
												patchedCount++;
											}
										}
										finally
										{
											Marshal.FreeHGlobal(translatedPtr);
										}
									}
								}
							}
						}
						catch (Exception ex)
						{
							// 忽略单个项的错误
							Console.WriteLine("AggressiveTextPatcher: Error processing ComboBox item " + i + ": " + ex.Message);
						}
					}
					
					// 处理ComboBox当前显示的文本（选中的项）
					if (currentSel >= 0)
					{
						try
						{
							StringBuilder currentTextSb = new StringBuilder(512);
							int currentTextLen = SendMessageCombo(hWnd, CB_GETLBTEXT, new IntPtr(currentSel), currentTextSb);
							if (currentTextLen > 0)
							{
								string currentText = currentTextSb.ToString();
								// 检查是否是文件路径
								bool isFilePath = currentText.Contains("\\") || currentText.Contains("/");
								if (!isFilePath)
								{
									string translatedCurrent = TranslateText(currentText);
									if (translatedCurrent != currentText)
									{
										// 更新ComboBox的显示文本
										ApiHelper.SetWindowText(hWnd, translatedCurrent);
										patchedCount++;
										Console.WriteLine("AggressiveTextPatcher: Updated ComboBox display text: '" + currentText + "' -> '" + translatedCurrent + "'");
									}
								}
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("AggressiveTextPatcher: Error updating ComboBox display text: " + ex.Message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("AggressiveTextPatcher: Error processing ComboBox: " + ex.Message);
			}
		}
		
		/// <summary>
		/// 处理TabControl控件
		/// </summary>
		private static void ProcessTabControl(IntPtr hWnd, ref int patchedCount)
		{
			try
			{
				// 获取标签页数量
				int itemCount = (int)ApiHelper.SendMessage(hWnd, TCM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
				if (itemCount > 0 && itemCount < 20) // 限制数量，避免过多
				{
					for (int i = 0; i < itemCount; i++)
					{
						try
						{
							// 分配文本缓冲区
							int bufferSize = 256;
							IntPtr textBuffer = Marshal.AllocHGlobal(bufferSize * 2); // Unicode需要2字节/字符
							try
							{
								// 初始化TCITEM结构 - 使用显式布局
								TCITEM item = new TCITEM();
								item.mask = TCIF_TEXT;
								item.pszText = textBuffer;
								item.cchTextMax = bufferSize;
								item.dwState = 0;
								item.dwStateMask = 0;
								item.iImage = -1;
								item.lParam = IntPtr.Zero;

								// 计算结构体大小（手动计算，避免Marshal.SizeOf的问题）
								// TCITEM布局：mask(4) + dwState(4) + dwStateMask(4) + pszText(4/8) + cchTextMax(4) + iImage(4) + lParam(4/8)
								int structSize = IntPtr.Size == 8 ? 40 : 28; // 64位: 40字节, 32位: 28字节
								IntPtr itemPtr = Marshal.AllocHGlobal(structSize);
								try
								{
									// 手动填充结构体（32位：mask(0-3) + dwState(4-7) + dwStateMask(8-11) + pszText(12-15) + cchTextMax(16-19) + iImage(20-23) + lParam(24-27)）
									// 64位：mask(0-3) + dwState(4-7) + dwStateMask(8-11) + pszText(12-19) + cchTextMax(20-23) + iImage(24-27) + lParam(28-35)
									int offset = 0;
									Marshal.WriteInt32(itemPtr, offset, (int)item.mask);
									offset += 4;
									Marshal.WriteInt32(itemPtr, offset, (int)item.dwState);
									offset += 4;
									Marshal.WriteInt32(itemPtr, offset, (int)item.dwStateMask);
									offset += 4;
									Marshal.WriteIntPtr(itemPtr, offset, item.pszText);
									offset += IntPtr.Size;
									Marshal.WriteInt32(itemPtr, offset, item.cchTextMax);
									offset += 4;
									Marshal.WriteInt32(itemPtr, offset, item.iImage);
									offset += 4;
									Marshal.WriteIntPtr(itemPtr, offset, item.lParam);
									
									// 获取标签页文本
									IntPtr result = ApiHelper.SendMessage(hWnd, TCM_GETITEM, new IntPtr(i), itemPtr);
									if (result != IntPtr.Zero)
									{
										// 从缓冲区读取文本
										string tabText = Marshal.PtrToStringAuto(textBuffer);
										if (!string.IsNullOrEmpty(tabText) && tabText != "Tab1" && tabText.Trim().Length > 0)
										{
											// 尝试翻译标签页文本
											string translatedTab = TranslateText(tabText);
											if (translatedTab != tabText)
											{
												Console.WriteLine("AggressiveTextPatcher: Found TabControl tab text: '" + tabText + "' -> '" + translatedTab + "' (index=" + i + ", HWND=" + hWnd + ")");
												
												// 设置翻译后的文本
												IntPtr translatedPtr = Marshal.StringToHGlobalAuto(translatedTab);
												try
												{
													// 创建新的TCITEM结构
													TCITEM setItem = new TCITEM();
													setItem.mask = TCIF_TEXT;
													setItem.pszText = translatedPtr;
													setItem.cchTextMax = translatedTab.Length + 1;
													setItem.dwState = 0;
													setItem.dwStateMask = 0;
													setItem.iImage = -1;
													setItem.lParam = IntPtr.Zero;
													
													IntPtr setItemPtr = Marshal.AllocHGlobal(structSize);
													try
													{
														// 手动填充结构体
														int setOffset = 0;
														Marshal.WriteInt32(setItemPtr, setOffset, (int)setItem.mask);
														setOffset += 4;
														Marshal.WriteInt32(setItemPtr, setOffset, (int)setItem.dwState);
														setOffset += 4;
														Marshal.WriteInt32(setItemPtr, setOffset, (int)setItem.dwStateMask);
														setOffset += 4;
														Marshal.WriteIntPtr(setItemPtr, setOffset, setItem.pszText);
														setOffset += IntPtr.Size;
														Marshal.WriteInt32(setItemPtr, setOffset, setItem.cchTextMax);
														setOffset += 4;
														Marshal.WriteInt32(setItemPtr, setOffset, setItem.iImage);
														setOffset += 4;
														Marshal.WriteIntPtr(setItemPtr, setOffset, setItem.lParam);
														
														IntPtr setResult = ApiHelper.SendMessage(hWnd, TCM_SETITEM, new IntPtr(i), setItemPtr);
														if (setResult != IntPtr.Zero)
														{
															Console.WriteLine("AggressiveTextPatcher: Successfully patched TabControl tab " + i + ": '" + tabText + "' -> '" + translatedTab + "'");
															patchedCount++;
														}
													}
													finally
													{
														Marshal.FreeHGlobal(setItemPtr);
													}
												}
												finally
												{
													Marshal.FreeHGlobal(translatedPtr);
												}
											}
										}
									}
								}
								finally
								{
									Marshal.FreeHGlobal(itemPtr);
								}
							}
							finally
							{
								Marshal.FreeHGlobal(textBuffer);
							}
						}
						catch (Exception ex)
						{
							// 忽略单个标签页的错误，继续处理下一个
							Console.WriteLine("AggressiveTextPatcher: Error processing TabControl tab " + i + ": " + ex.Message);
						}
					}
				}
			}
			catch (Exception ex)
			{
				// 忽略TabControl处理的错误，继续处理其他窗口
				Console.WriteLine("AggressiveTextPatcher: Error processing TabControl: " + ex.Message);
			}
		}

		/// <summary>
		/// 递归补丁子窗口
		/// </summary>
		private static void PatchChildWindows(IntPtr parentWindow, ref int patchedCount, int cycleCount)
		{
			IntPtr childWindow = IntPtr.Zero;
			int depth = 0;
			int maxDepth = 15; // 增加深度限制，以便捕获更深层的子标签栏

			do
			{
				childWindow = FindWindowEx(parentWindow, childWindow, null, null);
				if (childWindow != IntPtr.Zero && depth < maxDepth)
				{
					try
					{
						if (IsWindowVisible(childWindow))
						{
							// 获取窗口类名
							StringBuilder classNameSb = new StringBuilder(256);
							GetClassName(childWindow, classNameSb, classNameSb.Capacity);
							string className = classNameSb.ToString();
							
							// 获取窗口文本
							StringBuilder sb = new StringBuilder(1024);
							ApiHelper.GetWindowText(childWindow, sb, sb.Capacity);
							string windowText = sb.ToString();
							
							// 特殊处理：ComboBox的下拉项文本（使用统一的方法）
							if (className == "ComboBox" || className == "ComboBoxEx32")
							{
								ProcessComboBox(childWindow, ref patchedCount);
							}
							
							// 特殊处理：TabControl的标签页文本（使用统一的方法）
							if (className == "SysTabControl32")
							{
								ProcessTabControl(childWindow, ref patchedCount);
							}

							if (!string.IsNullOrEmpty(windowText) && windowText.Trim().Length > 0)
							{
								// 尝试翻译
								string translated = TranslateText(windowText);
								
								// 对于子标签栏的短文本（如"D.t落"、"T.轎h"等），增加调试输出
								if ((windowText.Length <= 10 && (windowText.Contains(".") || windowText.Contains("?") || windowText.Contains("落") || windowText.Contains("轎") || windowText.Contains("閉") || windowText.Contains("鮮") || windowText.Contains("珑") || windowText.Contains("锰"))) || cycleCount < 10)
								{
									Console.WriteLine("AggressiveTextPatcher: Text: '" + windowText + "' -> '" + translated + "' (Class=" + className + ", depth=" + depth + ")");
								}
										
								// 检查是否是误翻译为"综合"
								if (translated == "综合" && !windowText.Contains("Tổng hợp") && !windowText.Contains("Tổng") && windowText.Length > 3)
								{
									// 跳过这个翻译，避免误翻译
									translated = windowText; // 不翻译，保持原文
								}
								
								if (translated != windowText)
								{
									// 保存控件的原始启用状态
									bool wasEnabled = ApiHelper.IsWindowEnabled(childWindow);
									
									// 替换文本
									ApiHelper.SetWindowText(childWindow, translated);
									
									// 确保控件保持启用状态（如果原来是启用的）
									if (wasEnabled)
									{
										// 立即检查并恢复启用状态
										if (!ApiHelper.IsWindowEnabled(childWindow))
										{
											ApiHelper.EnableWindow(childWindow, true);
										}
										// 使用SendMessage发送WM_ENABLE消息，确保启用状态被应用
										// WM_ENABLE = 0x000A, wParam=1表示启用
										ApiHelper.SendMessage(childWindow, 0x000AU, new IntPtr(1), IntPtr.Zero);
									}
									
									patchedCount++;
									
									// 输出所有补丁信息（用于调试）
									Console.WriteLine("AggressiveTextPatcher: Patched child '" + windowText + "' -> '" + translated + "' (HWND=" + childWindow + ", depth=" + depth + ", Class=" + className + ")");
								}
							}

							// 递归处理子窗口
							PatchChildWindows(childWindow, ref patchedCount, cycleCount);
						}
					}
					catch
					{
						// 忽略错误，继续处理
					}

					depth++;
				}
			} while (childWindow != IntPtr.Zero && depth < maxDepth);
		}
	}
}
