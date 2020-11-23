"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" "C:\AutoSleep\AutoSleep.exe"
net start AutoSleep
services.msc
echo "install finished"
runas /user:# "" >nul 2>&1