path = debug.getinfo(1, "S").source:match("@(.*)$"):match("(.*[/\\])")
loadstring("\27\76\117\97\81\0\1\4\4\4\8\0\9\0\0\0\64\105\110\99\46\108\117\97\0\0\0\0\0\0\0\0\0\0\0\2\3\19\0\0\0\5\0\0\0\69\64\0\0\129\128\0\0\85\128\128\0\28\64\0\1\5\64\1\0\69\128\1\0\28\192\0\1\71\0\1\0\7\192\0\0\5\192\0\0\87\192\65\0\22\128\0\128\5\0\1\0\24\0\66\0\22\64\0\128\36\0\0\0\7\64\2\0\30\0\128\0\10\0\0\0\4\12\0\0\0\73\110\99\108\117\100\101\70\105\108\101\0\4\5\0\0\0\112\97\116\104\0\4\17\0\0\0\92\76\73\66\92\80\65\95\65\117\116\111\46\108\117\97\0\4\7\0\0\0\115\116\97\116\117\115\0\4\4\0\0\0\118\101\114\0\4\6\0\0\0\112\99\97\108\108\0\4\18\0\0\0\103\101\116\67\117\114\114\101\110\116\86\101\114\115\105\111\110\0\1\0\3\0\0\0\0\0\192\91\64\4\12\0\0\0\78\104\97\110\84\114\97\110\103\66\105\0\1\0\0\0\0\0\0\0\4\0\0\0\6\0\0\0\0\0\7\3\5\0\0\0\69\0\0\0\70\64\192\0\129\128\0\0\92\64\0\1\30\0\128\0\3\0\0\0\4\7\0\0\0\115\121\115\116\101\109\0\4\6\0\0\0\80\114\105\110\116\0\4\83\0\0\0\60\98\99\108\114\61\114\101\100\62\60\99\111\108\111\114\61\119\104\105\116\101\62\66\185\110\32\99\199\110\32\110\169\110\103\32\99\202\112\32\108\170\110\32\112\104\105\170\110\32\98\182\110\32\80\65\32\65\117\116\111\32\109\237\105\32\110\104\202\116\32\174\211\32\115\246\32\100\244\110\103\46\0\0\0\0\0\5\0\0\0\5\0\0\0\5\0\0\0\5\0\0\0\5\0\0\0\6\0\0\0\1\0\0\0\4\0\0\0\97\114\103\0\0\0\0\0\4\0\0\0\0\0\0\0\19\0\0\0\1\0\0\0\1\0\0\0\1\0\0\0\1\0\0\0\1\0\0\0\2\0\0\0\2\0\0\0\2\0\0\0\2\0\0\0\2\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\3\0\0\0\6\0\0\0\4\0\0\0\7\0\0\0\0\0\0\0\0\0\0\0") ()

-- szVulanLib = system.GetScriptFolder().."\\LIB\\VulanLib.lua"
-- IncludeFile(szVulanLib)
---------------------------------------------------------------------------------------
nKNB_Num 	= 2000		-- So luong can rut
----------------------------------------------------------------------------------------
IsNoKnbDialog = function()
	while menu.IsVisible(1) == 0 do 
		if dialog.IsVisible() ~= 0 then return 1 end
		timer.Sleep(10) 
		
	end
	return 0
end
----------------------------------------------------------------------------------------
-- Ham rut TienDong
RutTienDong = function()
	player.PathMoveTo(0,0,"Chñ TiÒn trang")	
	timer.Sleep(1000)
	while player.GetDoingStatus() ~= 1 do timer.Sleep(100) end

	while true do
		if tbVulanLib.CountItem(3,"TiÒn ®ång") >= nKNB_Num then 
			tbVulanLib.Chat("CH_NEARBY","§· cã ®ñ <color=green> TiÒn §ång") 
			return 
		end

		tbVulanLib.TalkToNpc("Chñ TiÒn trang")
		if dialog.IsVisible() ~= 0 then dialog.Close() timer.Sleep(100); end
		tbVulanLib.WaitMenu(1);

		if system.MatchString(menu.GetText(1, 0), "Ta muèn rót TiÒn §ång") == 1 then 
			menu.ClickIndex(1, 0) 
			timer.Sleep(100);
			if IsNoKnbDialog() ~= 0 then
				dialog.Close()
				system.Print("<color=yellow>§· hÕt <color=green> TiÒn §ång")
				return
			end
		end


		if system.MatchString(menu.GetText(1, 0), "Ta muèn lÊy ra 20 TiÒn §ång") == 1 then 
			menu.ClickIndex(1, 0) 
			timer.Sleep(100); 
			tbVulanLib.WaitMenu(1);
		end


		if system.MatchString(menu.GetText(1, 0), "X¸c nhËn") == 1 then 
			menu.ClickIndex(1, 0) 
			timer.Sleep(100) 
			tbVulanLib.WaitMenu(1);
		end	

		if system.MatchString(menu.GetText(1, 0), "KÕt thóc ®èi tho¹i") == 1 then 
			menu.ClickIndex(1, 0) 
			timer.Sleep(200) 
		end			
	end
end
----------------------------------------------------------------------------------------
function main()
	RutTienDong ()
end