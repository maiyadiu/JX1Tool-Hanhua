path = debug.getinfo(1, "S").source:match("@(.*)$"):match("(.*[/\\])")
loadstring("\27\76\117\97\81\0\1\4\4\4\8\0\9\0\0\0\64\105\110\99\46\108\117\97\0\0\0\0\0\0\0\0\0\0\0\2\3\19\0\0\0\5\0\0\0\69\64\0\0\129\128\0\0\85\128\128\0\28\64\0\1\5\64\1\0\69\128\1\0\28\192\0\1\71\0\1\0\7\192\0\0\5\192\0\0\87\192\65\0\22\128\0\128\5\0\1\0\24\0\66\0\22\64\0\128\36\0\0\0\7\64\2\0\30\0\128\0\10\0\0\0\4\12\0\0\0\73\110\99\108\117\100\101\70\105\108\101\0\4\5\0\0\0\112\97\116\104\0\4\17\0\0\0\92\76\73\66\92\80\65\95\65\117\116\111\46\108\117\97\0\4\7\0\0\0\115\116\97\116\117\115\0\4\4\0\0\0\118\101\114\0\4\6\0\0\0\112\99\97\108\108\0\4\18\0\0\0\103\101\116\67\117\114\114\101\110\116\86\101\114\115\105\111\110\0\1\0\3\0\0\0\0\0\192\91\64\4\12\0\0\0\78\104\97\110\84\114\97\110\103\66\105\0\1\0\0\0\0\0\0\0\4\0\0\0\6\0\0\0\0\0\7\3\5\0\0\0\69\0\0\0\70\64\192\0\129\128\0\0\92\64\0\1\30\0\128\0\3\0\0\0\4\7\0\0\0\115\121\115\116\101\109\0\4\6\0\0\0\80\114\105\110\116\0\4\83\0\0\0\60\98\99\108\114\61\114\101\100\62\60\99\111\108\111\114\61\119\104\105\116\101\62\66\185\110\32\99\199\110\32\110\169\110\103\32\99\202\112\32\108\170\110\32\112\104\105\170\110\32\98\182\110\32\80\65\32\65\117\116\111\32\109\237\105\32\110\104\202\116\32\174\211\32\115\246\32\100\244\110\103\46\0\0\0\0\0\5\0\0\0\5\0\0\0\5\0\0\0\5\0\0\0\5\0\0\0\6\0\0\0\1\0\0\0\4\0\0\0\97\114\103\0\0\0\0\0\4\0\0\0\0\0\0\0\19\0\0\0\1\0\0\0\1\0\0\0\1\0\0\0\1\0\0\0\1\0\0\0\2\0\0\0\2\0\0\0\2\0\0\0\2\0\0\0\2\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\6\0\0\0\4\0\0\0\7\0\0\0\0\0\0\0\0\0\0\0") ()
-- szVulanLib = system.GetScriptFolder().."\\LIB\\VulanLib.lua"
-- IncludeFile(szVulanLib)
-----------------------------------------------------------------
nHongBao = 0 -- SË  HÂng Bao c«n l y
bLocHongBao = 1 -- C„ l‰c HÂng Bao kh´ng?
-----------------------------------------------------------------
-- Danh s∏ch c∏c item sœ sˆ dÙng (C„ th” th™m ho∆c bÌt c∏c item kh∏c)
tbUseItemList  = {
	"Bang HÈi th«n mÀt hÂng bao",
	"PhÛc Duy™n LÈ (Trung) ",
	"PhÛc Duy™n LÈ (Ti”u) ",
	"Thi™n s¨n  B∂o LÈ ",
	"B∏ch Qu∂ LÈ ",
--	"Ti™n Th∂o LÈ ",
	"PhÛc Duy™n LÈ (ßπi) "
}

-- Danh s∏ch c∏c item sœ Æ≠Óc b∏n sau khi mÎ ra (C„ th” th™m ho∆c bÌt c∏c item kh∏c)
tbSell_ItemList = {
	"Phi Phong",
	"Kim Phong Thanh D≠¨ng Kh´i", -- B∏n trang bﬁ Kim Phong
	"Kim Phong K˙ L©n Huy’t",
	"Kim Phong ßoπt HÂn Ng‰c Æ∏i",
	"Kim Phong Thi™n L˝ Th∂o Th≠Óng Phi",
	"Kim Phong HÀu Ngh÷ d…n cung",
	"Kim Phong Trπc Li™n Quang",
	"Kim Phong Lan ß◊nh Ng‰c",
	"Kim Phong ßÂng T≠Ìc Xu©n Th©m",
	"Kim Phong C´ng C»n Tam Th∏n",
	"Thi™n Hoµng Long Long ßµi", -- B∏n trang bﬁ Thi™n Hoµng
	"Thi™n Hoµng Long Bπch Long T¯c",
	"Thi™n Hoµng Long Ng‰c Ti™u Di™u",
	"Thi™n Hoµng Long KhËc Thi™n L˝",
	"Thi™n Hoµng Long Hoµng Kim L©n",
	"Thi™n Hoµng Long Tr n Minh Li™n",
	"Thi™n Hoµng Long Bµn Long c´i",
	"Thi™n Hoµng Long NÈ Long ßÂng",
	"Thi™n Hoµng Long Cˆu Hi÷n chÿ",
--	"ßﬁnh QuËc § Sa Ph∏t Qu∏n", -- B∏n 1 sË trang bﬁ ßﬁnh QuËc
--	"ßﬁnh QuËc Thanh Sa Tr≠Íng Sam",
	"ßﬁnh QuËc Tˆ ßªng HÈ uy”n",
	"ßﬁnh QuËc X›ch Quy™n Nhuy‘n Ngoa",
	"ßﬁnh QuËc Ng©n Tµm Y™u Æ∏i"
}
-----------------------------------------------------------------
-- Hµm l‰c HÂng Bao
Open_Sell_Items = function()

	for i, v in pairs(tbUseItemList) do
		tbVulanLib.UseAllItem(v)
	end
	
	for i, k in pairs(tbSell_ItemList) do
		tbVulanLib.SellAllItem(k)
	end
end
-----------------------------------------------------------------

-- Hµm l y + l‰c HÂng Bao
LayHongBao = function()
	-- N’u Æ∑ thi’t lÀp sË HÂng Bao kh∏c 0 th◊ sœ l y ÆÛng sË Æ∑ thi’t lÀp
	if nHongBao > 0 then
		for i = 1, nHongBao do
			tbVulanLib.TalkToNpc("TÊng qu∂n L‘ ph»m ph≠Íng")
			tbVulanLib.WaitMenu(1)
			menu.ClickIndex(1,0)
			while menu.IsVisible(1) == 0 do  timer.Sleep(10) end
			menu.ClickIndex(1,0)
			timer.Sleep(10)
			if math.fmod(i, 10) == 0 and bLocHongBao == 1 then
				Open_Sell_Items ()
			end
		end
	-- N’u thi’t lÀp nHongBao = 0 th◊ sœ l y Æ’n khi Æ«y hµnh trang ho∆c bang hÈi h’t HÂng Bao)
	else
		local nCount = 0
		while true do
			nCount = nCount + 1
			tbVulanLib.TalkToNpc("TÊng qu∂n L‘ ph»m ph≠Íng")
			tbVulanLib.WaitMenu(1)
			menu.ClickIndex(1,0)
			while menu.IsVisible(1) == 0 do  timer.Sleep(10) end
			if system.MatchString(menu.GetTitle(1),"TÊng qu∂n L‘ ph»m ph≠Íng: ThÀt Æ∏ng ti’c") == 1  then
				menu.ClickIndex(1,0)
				return
			end	

			menu.ClickIndex(1,0)
			timer.Sleep(10)
			if math.fmod(nCount, 10) == 0 and bLocHongBao == 1  then
				Open_Sell_Items ()
			end
		end
	end
end
-----------------------------------------------------------------

function main()
	-- N’u Æ¯ng xa NPC "TÊng qu∂n L‘ ph»m ph≠Íng" th◊ sœ di chuy”n tÌi
	player.PathMoveTo(0, 0, "TÊng qu∂n L‘ ph»m ph≠Íng")
	timer.Sleep(1000)
	while player.GetDoingStatus() ~= 1 do timer.Sleep(100) end

	LayHongBao() -- L y + l‰c HÂng Bao
	Open_Sell_Items() --Loc them lan nua
	system.Print("ß∑ hoµn thµnh!")
end