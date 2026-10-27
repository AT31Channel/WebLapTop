# WebLapTop

Ứng dụng web bán laptop xây dựng bằng ASP.NET Core MVC.

## Yêu cầu

- .NET 8 SDK
- SQL Server

## Cài đặt

1. Clone repo
2. Tạo file cấu hình local:
   ```
   cp WebLapTop/appsettings.json WebLapTop/appsettings.Development.json
   ```
3. Chỉnh sửa `appsettings.Development.json`:
   - Thay `YOUR_SERVER_NAME` bằng tên SQL Server của bạn
   - Điền thông tin email nếu cần
4. Chạy migration:
   ```
   dotnet ef database update --project WebLapTop
   ```
5. Khởi động ứng dụng:
   ```
   runapp.bat
   ```
   hoặc:
   ```
   dotnet run --project WebLapTop
   ```
