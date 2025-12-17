/* =====================================================
   HOTFIX V3: Fix Password Column Length
   Tăng độ dài cột MatKhau để chứa SHA256 hash (64 chars)
===================================================== */

USE ParkingDB;
GO

PRINT N'=== Bắt đầu Hotfix - Fix Password Column ===';
GO

-- Tăng độ dài cột MatKhau từ VARCHAR(50) lên VARCHAR(100)
ALTER TABLE NguoiDung
ALTER COLUMN MatKhau VARCHAR(100) NOT NULL;
GO

PRINT N'✓ Đã tăng độ dài cột MatKhau lên VARCHAR(100)';
GO

-- Kiểm tra
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'NguoiDung' AND COLUMN_NAME = 'MatKhau';
GO

PRINT N'';
PRINT N'=== ✅ Hotfix hoàn thành! ===';
PRINT N'Bây giờ có thể lưu password hash 64 ký tự';
GO
