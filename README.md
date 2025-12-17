------------------------------------------------
I. KIỂM TRA SQL SERVER ĐANG CHẠY
------------------------------------------------
1. Mở SQL Server Configuration Manager
2. Kiểm tra các dịch vụ sau:
   - SQL Server (MSSQLSERVER hoặc SQLEXPRESS) → Running
   - SQL Server Browser → Running
   Nếu chưa chạy → Click chuột phải → Start

------------------------------------------------
II. BẬT TCP/IP VÀ CẤU HÌNH PORT 1433
------------------------------------------------
1. Mở SQL Server Configuration Manager
2. Vào:
   SQL Server Network Configuration
   → Protocols for MSSQLSERVER (hoặc SQLEXPRESS)
3. Click phải vào TCP/IP → Enable
4. Click phải TCP/IP → Properties
5. Chọn tab "IP Addresses"

6. Kéo xuống IPAll:
   - TCP Dynamic Ports: (xóa hết)
   - TCP Port: 1433

7. Nhấn OK

------------------------------------------------
III. KHỞI ĐỘNG LẠI SQL SERVER
------------------------------------------------
