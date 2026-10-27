@echo off
chcp 65001 > nul
:: Clean Run WebLapTop - Xử lý các lỗi file lock

echo [1/4] Đang dừng tất cả các tiến trình WebLapTop...
taskkill /F /FI "IMAGENAME eq WebLapTop*" /T >nul 2>&1
taskkill /F /FI "IMAGENAME eq dotnet.exe" /T >nul 2>&1
timeout /t 2 /nobreak >nul

echo [2/4] Đang xóa thư mục build để tránh lỗi file locked...
cd WebLapTop
rmdir /s /q bin >nul 2>&1
rmdir /s /q obj >nul 2>&1
cd ..

echo [3/4] Đang restore và build ứng dụng...
dotnet restore WebLapTop
dotnet build WebLapTop -v q

echo [4/4] Đang khởi động ứng dụng (HTTPS mở tự động)...
set ASPNETCORE_ENVIRONMENT=Production
set DOTNET_hostBuilder:reloadConfigOnChange=false

:: Chạy ứng dụng với tham số mở HTTPS
echo Ứng dụng đang chạy, đường dẫn HTTPS sẽ tự động mở trên trình duyệt...
dotnet run --project WebLapTop --no-build
 
