using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CrackVLBS
{
	/// <summary>
	/// 界面中文补丁类 - 用于将目标程序的越南文界面翻译为中文
	/// </summary>
	internal class UIChinesePatcher
	{
		// 越南文到中文的翻译映射表（包含乱码文本）
		// 使用静态字段直接初始化（与AggressiveTextPatcher保持一致）
		private static readonly Dictionary<string, string> TranslationMap;
		
		// 静态构造函数，用于初始化并输出日志
		static UIChinesePatcher()
		{
			try
			{
				Console.WriteLine("UIChinesePatcher: Starting TranslationMap initialization...");
				TranslationMap = new Dictionary<string, string>();
				
				// 使用ContainsKey检查逐个添加，避免重复键导致整个字典初始化失败
				Action<string, string> addKey = (key, value) =>
				{
					if (TranslationMap.ContainsKey(key))
					{
						Console.WriteLine("UIChinesePatcher: WARNING - Duplicate key detected: '" + key + "' (existing value: '" + TranslationMap[key] + "', new value: '" + value + "')");
					}
					else
					{
						TranslationMap.Add(key, value);
					}
				};
				
				// 添加所有翻译映射
				{
			// 标签页（正常文本）
			addKey("Tổng hợp", "综合");
			addKey("Đồ tẩu", "挂机");
			addKey("Thống kê", "统计");
			addKey("Bang hội", "帮会");
			addKey("Kỹ năng", "技能");
			addKey("Lọc đồ", "筛选");
			addKey("Phụ trợ", "辅助");
			
			// 标签页（乱码文本）
			// 注意：不要将"T."映射为"综合"，因为很多文本都包含"T"，会导致误匹配
			// addKey("T.", "综合"); // 已禁用，避免误匹配
			addKey("D.t菇", "挂机");
			addKey("D.T落", "挂机"); // 从截图提取的新乱码
			addKey("T.h", "统计");
			addKey("T.翰h", "统计"); // 从截图提取的新乱码
			addKey("Bang h閉", "帮会");
			addKey("?khi鮮", "技能");
			addKey("?khi解", "技能"); // 从截图提取的新乱码
			addKey("L.c妣g", "筛选");
			addKey("L.c珑g", "筛选"); // 从截图提取的新乱码
			addKey("P.h鈺", "辅助");
			addKey("P.h錳", "辅助"); // 从截图提取的新乱码
			
			// 功能选项（正常文本）
			addKey("Luyện công", "练功");
			addKey("Nhiệm vụ Dã tẩu", "日常任务");
			addKey("PT Nhiệm vụ", "PT任务");
			addKey("Túi vô", "背包");
			addKey("Túi xếp cá", "鱼袋");
			addKey("Nhặt trong thành", "城内拾取");
			addKey("Không nhặt vật phẩm (đen)", "不拾取物品(黑名单)");
			addKey("PT Tết", "春节PT");
			addKey("Danh vọng", "声望");
			addKey("Tín sứ", "信使");
			addKey("Ẩn Game", "隐藏游戏");
			addKey("Reset (>3h)", "重置(>3小时)");
			
			// 功能选项（乱码文本 - 问号形式）
			addKey("Luy?n c?ng", "练功");
			addKey("LuyÔn cæng", "练功");
			addKey("Luyên căng", "练功");
			addKey("Nhiễm vô D. tều", "日常任务");
			addKey("Nhiễm vô D • tu", "日常任务");
			addKey("PT Nhâm", "PT任务");
			addKey("PT Nh?m", "PT任务");
			addKey("PT Nhóm", "PT任务");
			addKey("PT Têt c?", "春节PT");
			addKey("PT TẾT c", "春节PT");
			addKey("PT Tất cả", "春节PT");
			addKey("Tù vô", "背包");
			addKey("Tù v?", "背包");
			addKey("Tự vệ", "背包");
			addKey("Tù xếp cả", "鱼袋");
			addKey("Tự xếp đồ", "鱼袋");
			addKey("Nh£t trong thµnh", "城内拾取");
			addKey("Nh₤t trong thµnh", "城内拾取");
			addKey("Kh«ng nhất vĒt phầm (đen)", "不拾取物品(黑名单)");
			addKey("Kh«ng nhất vịt phÈm (en)", "不拾取物品(黑名单)");
			addKey("Danh văng", "声望");
			addKey("Tín so", "信使");
			addKey("Tin s", "信使");
			addKey("Èn Game", "隐藏游戏");
			addKey("èn Game", "隐藏游戏");
			addKey("ẩn Game", "隐藏游戏");
			
			// 按钮（正常文本）
			addKey("Hành trang", "行囊");
			addKey("Tìm", "查找");
			addKey("QSRpecAuto", "自动挂机");
			addKey("Đăng ký", "注册");
			
			// 按钮（乱码文本）
			addKey("H µnh trang", "行囊");
			addKey("TXm??", "查找");
			addKey("§ng ky", "注册");
			addKey("§¨ng ký", "注册");
			
			// 输入框标签（正常文本）
			addKey("Thoát Game", "退出游戏");
			addKey("Tắt máy", "关闭电脑");
			addKey("Thoát Game (hh:mm)", "退出游戏 (时:分)");
			addKey("Tắt máy (hh:mm)", "关闭电脑 (时:分)");
			
			// 输入框标签（乱码文本）
			addKey("Tho, t Game (hh:mm)", "退出游戏 (时:分)");
			addKey("Tßt m, y (hh:mm)", "关闭电脑 (时:分)");
			addKey("Tt m, y (hh:mm)", "关闭电脑 (时:分)");
			
			// 统计信息（正常文本）
			addKey("Về thành", "回城");
			addKey("Tử vong", "死亡");
			addKey("Thu nhặt", "拾取");
			addKey("Dã tẩu", "挂机");
			addKey("Nhiệm vụ", "任务");
			
			// 统计信息（乱码文本）
			addKey("Về thµnh", "回城");
			addKey("Về thành:", "回城:");
			addKey("Tô vong", "死亡");
			addKey("Tử vong:", "死亡:");
			addKey("Thu nhẸp", "拾取");
			addKey("Thu nhẹp", "拾取");
			addKey("Thu nhập", "拾取");
			addKey("D. tều", "挂机");
			addKey("D • tèu", "挂机");
			addKey("Dã tẩu:", "挂机:");
			addKey("Nhiễm vô", "任务");
			addKey("Nhiệm vụ:", "任务:");
			
			// 表格标题（正常文本）
			addKey("Tên nhân vật", "角色名");
			addKey("Số lượng", "数量");
			addKey("Niveau", "等级");
			addKey("Level", "等级");
			
			// 表格标题（乱码文本）
			addKey("T獨 nh_v", "角色名");
			addKey("T獨nh_v", "角色名");
			addKey("S/L", "数量");
			addKey("N/L", "等级");
			
			// 下拉菜单（乱码文本）
			addKey("1. D?t菇", "1. 挂机");
			
			// 其他常见文本
			addKey("Bo qua", "跳过");
			addKey("Login", "登录");
			
			// 根据日志添加的实际找到的文本（避免误翻译为"综合"）
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
			
			// 更多实际找到的文本（从图片和日志中提取）
			addKey("Lấy vật phẩm khi Reset", "重置时拾取物品");
			addKey("Lấy khi đầy Hành trang", "背包满时拾取");
			addKey("Lờy v?t phốm", "拾取物品");
			addKey("L?c L?nh bui", "筛选令牌");
			addKey("Lệnh bài nhiệm vụ", "任务令牌");
			addKey("Sau khi hoàn thành nhiệm vụ", "完成任务后");
			addKey("Giao ??", "交任务");
			addKey("Nhiệm vụ đặc biệt", "特殊任务");
			addKey("D?ng", "停止");
			addKey("Bơm S.lực mức 1", "充内力等级1");
			addKey("Bơm S.lực mức 2", "充内力等级2");
			addKey("Bơm N.lực mức 1", "充外功等级1");
			addKey("Bơm N.lực mức 2", "充外功等级2");
			addKey("Dùng X2 khi luyện công", "练功时使用X2");
			addKey("Đứng im (idle) trong thành 15 phút", "在城内静止15分钟");
			
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
			
				Console.WriteLine("UIChinesePatcher: TranslationMap initialized successfully with " + TranslationMap.Count + " entries");
			}
			catch (Exception ex)
			{
				Console.WriteLine("UIChinesePatcher: ERROR initializing TranslationMap: " + ex.Message);
				Console.WriteLine("UIChinesePatcher: Stack trace: " + ex.StackTrace);
				TranslationMap = new Dictionary<string, string>(); // 使用空字典作为后备
			}
		}

		/// <summary>
		/// 对目标程序的所有窗口进行中文补丁
		/// </summary>
		/// <param name="process">目标进程</param>
		/// <param name="maxRetries">最大重试次数</param>
		public static void PatchAllWindows(System.Diagnostics.Process process, int maxRetries = 50)
		{
			if (process == null)
			{
				Console.WriteLine("UIChinesePatcher: ERROR - Process is null!");
				return;
			}
			
			if (process.HasExited)
			{
				Console.WriteLine("UIChinesePatcher: ERROR - Process has already exited!");
				return;
			}

			Console.WriteLine("UIChinesePatcher: Creating patch thread...");
			Console.WriteLine("UIChinesePatcher: TranslationMap.Count=" + (TranslationMap != null ? TranslationMap.Count.ToString() : "null"));
			Thread patchThread = new Thread(() =>
			{
				try
				{
					Console.WriteLine("UIChinesePatcher: Thread started, PID=" + process.Id);
					Console.WriteLine("UIChinesePatcher: Process name=" + process.ProcessName);
					Console.WriteLine("UIChinesePatcher: Process HasExited=" + process.HasExited);
					Console.WriteLine("UIChinesePatcher: TranslationMap.Count=" + (TranslationMap != null ? TranslationMap.Count.ToString() : "null"));
					int retryCount = 0;
				
				// 等待窗口完全加载
				Console.WriteLine("UIChinesePatcher: Waiting 5 seconds for window to load...");
				Thread.Sleep(5000);
				Console.WriteLine("UIChinesePatcher: Wait completed, starting to search for windows...");
				
				while (retryCount < maxRetries && !process.HasExited)
				{
					try
					{
						// 等待窗口出现
						if (process.MainWindowHandle == IntPtr.Zero)
						{
							Thread.Sleep(500);
							retryCount++;
							if (retryCount % 10 == 0)
							{
								Console.WriteLine("UIChinesePatcher: Waiting for window, retry " + retryCount + "/" + maxRetries);
							}
							continue;
						}

						// 获取主窗口
						IntPtr mainWindowHandle = process.MainWindowHandle;
						Console.WriteLine("UIChinesePatcher: MainWindowHandle = " + mainWindowHandle);
						
						if (mainWindowHandle != IntPtr.Zero)
						{
							bool isVisible = IsWindowVisible(mainWindowHandle);
							Console.WriteLine("UIChinesePatcher: Window visible = " + isVisible);
							
							if (isVisible)
							{
								// 获取窗口标题
								StringBuilder title = new StringBuilder(256);
								ApiHelper.GetWindowText(mainWindowHandle, title, title.Capacity);
								Console.WriteLine("UIChinesePatcher: Window title = '" + title.ToString() + "'");
								
								Console.WriteLine("UIChinesePatcher: Found main window, HWND=" + mainWindowHandle);
								
								// 补丁主窗口及其所有子窗口
								Console.WriteLine("UIChinesePatcher: Starting recursive patch...");
								int patchedCount = PatchWindowRecursive(mainWindowHandle, 0);
								Console.WriteLine("UIChinesePatcher: Patched " + patchedCount + " windows");
								
								// 如果成功补丁，退出循环
								if (retryCount > 5 && patchedCount > 0)
								{
									Console.WriteLine("UIChinesePatcher: Initial patching completed");
									break;
								}
								else if (patchedCount == 0)
								{
									Console.WriteLine("UIChinesePatcher: Warning - No windows were patched. This may indicate:");
									Console.WriteLine("UIChinesePatcher:   1. No Vietnamese text found in windows");
									Console.WriteLine("UIChinesePatcher:   2. Text is not accessible via GetWindowText");
									Console.WriteLine("UIChinesePatcher:   3. Text is drawn directly (not using standard controls)");
								}
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("UIChinesePatcher Error: " + ex.Message + "\n" + ex.StackTrace);
					}

					Thread.Sleep(1000);
					retryCount++;
				}
				
					Console.WriteLine("UIChinesePatcher: Initial patching thread ended");
				}
				catch (Exception ex)
				{
					Console.WriteLine("UIChinesePatcher: FATAL ERROR in patch thread: " + ex.Message);
					Console.WriteLine("UIChinesePatcher: Stack trace: " + ex.StackTrace);
				}
			})
			{
				IsBackground = true
			};
			
			Console.WriteLine("UIChinesePatcher: Starting patch thread...");
			patchThread.Start();
			Console.WriteLine("UIChinesePatcher: Patch thread started, returning from PatchAllWindows");
		}

		// 用于跟踪补丁周期
		private static int _patchCycleCount = 0;

		/// <summary>
		/// 递归补丁窗口及其所有子窗口
		/// </summary>
		private static int PatchWindowRecursive(IntPtr windowHandle, int depth)
		{
			int patchedCount = 0;
			int cycleCount = _patchCycleCount;
			
			if (windowHandle == IntPtr.Zero)
			{
				return 0;
			}
			
			// 限制递归深度，避免无限递归
			if (depth > 20)
			{
				return 0;
			}

			try
			{
				// 获取窗口类名
				StringBuilder className = new StringBuilder(256);
				GetClassName(windowHandle, className, className.Capacity);
				string classNameStr = className.ToString();
				
				// 补丁当前窗口的文本
				StringBuilder sb = new StringBuilder(1024);
				ApiHelper.GetWindowText(windowHandle, sb, sb.Capacity);
				string currentText = sb.ToString();
				
				// 只处理有文本的窗口
				if (!string.IsNullOrEmpty(currentText) && currentText.Trim().Length > 0)
				{
					// 输出找到的文本（用于调试）
					if (depth < 3)
					{
						Console.WriteLine("UIChinesePatcher: Found text in " + classNameStr + ": '" + currentText + "' (HWND=" + windowHandle + ")");
					}
					
					// 尝试翻译所有文本（不仅仅是越南文或乱码）
					string chineseText = TranslateText(currentText);
					
					// 调试输出：显示原始文本和翻译结果
					if (!string.IsNullOrEmpty(currentText) && currentText.Length > 0 && depth < 3)
					{
						Console.WriteLine("UIChinesePatcher: Original text: '" + currentText + "', Translated: '" + chineseText + "', TranslationMap.Count=" + (TranslationMap != null ? TranslationMap.Count.ToString() : "null"));
					}
					
					// 如果翻译结果不同，就进行补丁
					// 但是，如果翻译结果是"综合"，且原文本不是"Tổng hợp"或其变体，则跳过（避免误翻译）
					if (chineseText != currentText && !string.IsNullOrEmpty(chineseText))
					{
						// 检查是否是误翻译为"综合"
						if (chineseText == "综合" && !currentText.Contains("Tổng hợp") && !currentText.Contains("Tổng") && currentText.Length > 3)
						{
							// 跳过这个翻译，避免误翻译
							if (depth < 2)
							{
								Console.WriteLine("UIChinesePatcher: Skipping translation '" + currentText + "' -> '综合' (likely incorrect match)");
							}
							chineseText = currentText; // 不翻译，保持原文
						}
						
						if (chineseText != currentText)
						{
							// 输出翻译信息（用于调试）
							if (depth < 3 || cycleCount % 10 == 0) // 只输出前3层或每10个周期输出一次
							{
								Console.WriteLine("UIChinesePatcher: Translating '" + currentText + "' -> '" + chineseText + "' (HWND=" + windowHandle + ", Class=" + classNameStr + ")");
							}
							
							// 进行补丁
							// 使用SendMessage设置文本（更可靠）
							if (classNameStr == "Button")
							{
								// 对于按钮，使用PatchButton方法
								try
								{
									CrackVLBS19v3.PatchButton(windowHandle, chineseText);
									patchedCount++;
									if (depth < 5) // 输出更多层的调试信息
									{
										Console.WriteLine("UIChinesePatcher: Patched Button '" + currentText + "' -> '" + chineseText + "' (HWND=" + windowHandle + ")");
									}
								}
								catch (Exception ex)
								{
									// 如果PatchButton失败，使用ForceWindowText
									Console.WriteLine("UIChinesePatcher: PatchButton failed, using ForceWindowText: " + ex.Message);
									ForceWindowText(windowHandle, chineseText);
									patchedCount++;
								}
							}
							else if (classNameStr == "Static" || classNameStr == "Edit" || classNameStr.Contains("Label"))
							{
								// 对于标签和文本框，使用ForceWindowText
								ForceWindowText(windowHandle, chineseText);
								patchedCount++;
								if (depth < 5)
								{
									Console.WriteLine("UIChinesePatcher: Patched " + classNameStr + " '" + currentText + "' -> '" + chineseText + "' (HWND=" + windowHandle + ")");
								}
							}
							else
							{
								// 对于其他控件，也尝试补丁
								ForceWindowText(windowHandle, chineseText);
								patchedCount++;
								if (depth < 3)
								{
									Console.WriteLine("UIChinesePatcher: Patched " + classNameStr + " '" + currentText + "' -> '" + chineseText + "' (HWND=" + windowHandle + ")");
								}
							}
						}
					}
					else if (depth < 2 && currentText.Length > 0)
					{
						// 输出未翻译的文本（用于调试）
						Console.WriteLine("UIChinesePatcher: Skipped text (no translation): '" + currentText + "' (Class=" + classNameStr + ")");
					}
				}

				// 特殊处理：ComboBox的下拉项和Dialog窗口
				if (classNameStr == "ComboBox" || classNameStr == "ComboBoxEx32")
				{
					try
					{
						// 获取下拉项数量
						int itemCount = (int)ApiHelper.SendMessage(windowHandle, 0x0146U, IntPtr.Zero, IntPtr.Zero); // CB_GETCOUNT
						if (itemCount > 0 && itemCount < 100)
						{
							// 获取当前选中的项
							int currentSel = (int)ApiHelper.SendMessage(windowHandle, 0x0147U, IntPtr.Zero, IntPtr.Zero); // CB_GETCURSEL
							
							// 从后往前处理，避免索引变化
							for (int i = itemCount - 1; i >= 0; i--)
							{
								try
								{
									// 获取下拉项文本 - 需要使用CB_GETLBTEXT
									StringBuilder comboTextSb = new StringBuilder(512);
									// 注意：CB_GETLBTEXT需要特殊的SendMessage重载，这里先记录
									// 实际处理会在AggressiveTextPatcher中完成
								}
								catch
								{
									// 忽略错误
								}
							}
						}
					}
					catch
					{
						// 忽略ComboBox处理错误
					}
				}
				
				// 特殊处理：Dialog窗口（#32770）
				if (classNameStr == "#32770")
				{
					// Dialog窗口的子控件会在递归处理中自动处理
					// 这里可以添加特殊的Dialog处理逻辑
				}
				
				// 递归处理所有子窗口
				IntPtr childWindow = IntPtr.Zero;
				do
				{
					childWindow = ApiHelper.FindWindowEx(windowHandle, childWindow, null, null);
					if (childWindow != IntPtr.Zero)
					{
						patchedCount += PatchWindowRecursive(childWindow, depth + 1);
					}
				} while (childWindow != IntPtr.Zero);
			}
			catch (Exception ex)
			{
				if (depth < 3)
				{
					Console.WriteLine("PatchWindowRecursive Error: " + ex.Message);
				}
			}
			
			return patchedCount;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		/// <summary>
		/// 强制设置窗口文本（用于按钮和标签）
		/// </summary>
		private static void ForceWindowText(IntPtr hWnd, string newText)
		{
			if (hWnd == IntPtr.Zero || !ApiHelper.IsWindow(hWnd))
			{
				return;
			}

			try
			{
				// 保存控件的原始启用状态
				bool wasEnabled = ApiHelper.IsWindowEnabled(hWnd);
				
				// 尝试多次设置，确保成功
				for (int i = 0; i < 3; i++)
				{
					ApiHelper.SetWindowText(hWnd, newText);
					Thread.Sleep(50);
					
					// 验证是否设置成功
					StringBuilder sb = new StringBuilder(256);
					ApiHelper.GetWindowText(hWnd, sb, sb.Capacity);
					if (sb.ToString() == newText)
					{
						break;
					}
				}
				
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
			}
			catch
			{
				// 忽略错误，继续尝试
			}
		}

		/// <summary>
		/// 翻译文本（越南文转中文，包括乱码文本）
		/// </summary>
		private static string TranslateText(string vietnameseText)
		{
			if (string.IsNullOrEmpty(vietnameseText))
			{
				return vietnameseText;
			}

			// 去除首尾空格
			string trimmedText = vietnameseText.Trim();
			string originalText = trimmedText;

			// 如果TranslationMap为空（初始化失败），返回原文
			if (TranslationMap == null)
			{
				Console.WriteLine("UIChinesePatcher.TranslateText: WARNING - TranslationMap is null for text: " + vietnameseText);
				return vietnameseText;
			}
			
			if (TranslationMap.Count == 0)
			{
				Console.WriteLine("UIChinesePatcher.TranslateText: WARNING - TranslationMap is empty for text: " + vietnameseText);
				return vietnameseText;
			}

			// 1. 直接查找翻译映射（精确匹配）- 最优先
			if (TranslationMap.ContainsKey(trimmedText))
			{
				return TranslationMap[trimmedText];
			}

			// 2. 尝试匹配文本的开头部分（处理 "Về thành: 0" 这样的情况）
			// 只匹配较长的键（至少3个字符），避免短键误匹配
			foreach (var kvp in TranslationMap)
			{
				if (kvp.Key.Length >= 3 && trimmedText.StartsWith(kvp.Key))
				{
					// 提取冒号后的内容（如果有）
					string suffix = "";
					if (trimmedText.Contains(":"))
					{
						int colonIndex = trimmedText.IndexOf(':');
						if (colonIndex >= 0 && colonIndex < trimmedText.Length - 1)
						{
							suffix = trimmedText.Substring(colonIndex);
						}
					}
					return kvp.Value + suffix;
				}
			}

			// 3. 尝试部分匹配，但要求匹配长度至少为3个字符，且匹配度超过50%
			// 避免短字符串（如"T."）误匹配
			int bestMatchLength = 0;
			string bestMatch = null;
			foreach (var kvp in TranslationMap)
			{
				// 跳过太短的键（少于3个字符），除非是精确匹配
				if (kvp.Key.Length < 3)
				{
					continue;
				}

				// 计算匹配长度
				int matchLength = 0;
				if (trimmedText.Contains(kvp.Key))
				{
					matchLength = kvp.Key.Length;
				}
				else if (kvp.Key.Contains(trimmedText) && trimmedText.Length >= 3)
				{
					matchLength = trimmedText.Length;
				}

				// 要求匹配长度至少为3，且匹配度超过50%
				if (matchLength >= 3 && matchLength >= Math.Max(trimmedText.Length, kvp.Key.Length) * 0.5)
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
				// 如果原文本有冒号，保留冒号
				if (trimmedText.Contains(":") && !bestMatch.Contains(":"))
				{
					return bestMatch + ":";
				}
				return bestMatch;
			}

			// 4. 尝试模糊匹配（处理问号等乱码字符）
			// 将问号替换后进行匹配
			string normalizedText = NormalizeGarbledText(trimmedText);
			if (normalizedText.Length >= 3)
			{
				bestMatchLength = 0;
				bestMatch = null;
				foreach (var kvp in TranslationMap)
				{
					if (kvp.Key.Length < 3)
					{
						continue;
					}

					string normalizedKey = NormalizeGarbledText(kvp.Key);
					int matchLength = 0;
					if (normalizedText.Contains(normalizedKey) && normalizedKey.Length >= 3)
					{
						matchLength = normalizedKey.Length;
					}
					else if (normalizedKey.Contains(normalizedText) && normalizedText.Length >= 3)
					{
						matchLength = normalizedText.Length;
					}

					if (matchLength >= 3 && matchLength >= Math.Max(normalizedText.Length, normalizedKey.Length) * 0.5)
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
					if (trimmedText.Contains(":") && !bestMatch.Contains(":"))
					{
						return bestMatch + ":";
					}
					return bestMatch;
				}
			}

			// 5. 尝试匹配包含冒号的文本（如 "Về thành: 0"）
			if (trimmedText.Contains(":"))
			{
				string textBeforeColon = trimmedText.Split(':')[0].Trim();
				if (textBeforeColon.Length >= 2)
				{
					foreach (var kvp in TranslationMap)
					{
						if (kvp.Key.Length >= 2 && (textBeforeColon.Contains(kvp.Key) || kvp.Key.Contains(textBeforeColon)))
						{
							// 提取冒号后的内容
							string suffix = "";
							int colonIndex = trimmedText.IndexOf(':');
							if (colonIndex >= 0 && colonIndex < trimmedText.Length - 1)
							{
								suffix = trimmedText.Substring(colonIndex);
							}
							return kvp.Value + suffix;
						}
					}
				}
			}

			// 6. 如果没有找到翻译，返回原文（不要使用默认值）
			return vietnameseText;
		}

		/// <summary>
		/// 规范化乱码文本（将问号等替换为可能的字符）
		/// </summary>
		private static string NormalizeGarbledText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			// 移除问号和其他乱码字符
			// 注意：不要移除所有特殊字符，只移除明显的乱码
			string normalized = text;
			// 移除常见的乱码字符（问号、特殊符号等）
			normalized = normalized.Replace("?", "");
			normalized = normalized.Replace("?", "");
			normalized = normalized.Replace("?", "");
			normalized = normalized.Replace("×", "");
			normalized = normalized.Replace("μ", "");
			normalized = normalized.Replace("è", "");
			normalized = normalized.Replace("1", "");
			// 注意：不要移除"T"和"t"，因为它们是正常字符
			// normalized = normalized.Replace("T", "");
			// normalized = normalized.Replace("t", "");
			return normalized;
		}

		/// <summary>
		/// 检查文本是否包含越南文字符或乱码
		/// </summary>
		private static bool ContainsVietnameseOrGarbled(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}

			// 检查是否包含常见的乱码字符
			foreach (char c in text)
			{
				// 检查是否是越南文字符范围或乱码字符
				if ((c >= 0x00C0 && c <= 0x1EF9) || // 越南文字符范围
				    (c >= 0x0080 && c <= 0x00FF && !char.IsLetterOrDigit(c) && !char.IsPunctuation(c) && !char.IsWhiteSpace(c))) // 可能的乱码
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 移除标点符号（用于模糊匹配）
		/// </summary>
		private static string RemovePunctuation(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}

			char[] chars = text.ToCharArray();
			StringBuilder sb = new StringBuilder();
			foreach (char c in chars)
			{
				if (!char.IsPunctuation(c) && !char.IsWhiteSpace(c))
				{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// 持续监控并补丁窗口文本（用于动态更新的文本）
		/// </summary>
		public static void StartContinuousPatching(System.Diagnostics.Process process, int intervalMs = 2000)
		{
			if (process == null)
			{
				Console.WriteLine("UIChinesePatcher: ERROR - Process is null in StartContinuousPatching!");
				return;
			}
			
			Console.WriteLine("UIChinesePatcher: Creating continuous patching thread...");
			Thread monitorThread = new Thread(() =>
			{
				try
				{
					Console.WriteLine("UIChinesePatcher: Continuous patching thread started, PID=" + process.Id);
					// 等待窗口完全加载
					Console.WriteLine("UIChinesePatcher: Waiting 5 seconds for window to load...");
					Thread.Sleep(5000);
					Console.WriteLine("UIChinesePatcher: Continuous patching started");
				
				int cycleCount = 0;
				while (!process.HasExited)
				{
					try
					{
						if (process.MainWindowHandle != IntPtr.Zero)
						{
							IntPtr mainWindowHandle = process.MainWindowHandle;
							if (mainWindowHandle != IntPtr.Zero && IsWindowVisible(mainWindowHandle))
							{
							// 递归补丁所有窗口
							_patchCycleCount++;
							int patchedCount = PatchWindowRecursive(mainWindowHandle, 0);
							cycleCount++;
							
							if (patchedCount > 0 && cycleCount % 5 == 0)
							{
								Console.WriteLine("UIChinesePatcher: Continuous patching cycle " + cycleCount + ", patched " + patchedCount + " windows");
							}
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("ContinuousPatching Error: " + ex.Message);
					}

					Thread.Sleep(intervalMs);
				}
				
				Console.WriteLine("UIChinesePatcher: Continuous patching ended");
				}
				catch (Exception ex)
				{
					Console.WriteLine("UIChinesePatcher: FATAL ERROR in continuous patching thread: " + ex.Message);
					Console.WriteLine("UIChinesePatcher: Stack trace: " + ex.StackTrace);
				}
			})
			{
				IsBackground = true
			};
			
			Console.WriteLine("UIChinesePatcher: Starting continuous patching thread...");
			monitorThread.Start();
			Console.WriteLine("UIChinesePatcher: Continuous patching thread started, returning from StartContinuousPatching");
		}
	}
}
