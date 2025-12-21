/* =====================================================
   HỆ THỐNG QUẢN LÝ HẦM GỬI XE - SCRIPT DATABASE ĐẦY ĐỦ
===================================================== */

-- =====================================================
-- 1. TẠO DATABASE MỚI
-- =====================================================
USE master;  -- Chuyển sang database master để thao tác
GO

-- Kiểm tra và xóa database cũ nếu tồn tại
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'ParkingDB')
BEGIN
    -- Ngắt tất cả kết nối đến database
    ALTER DATABASE ParkingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    -- Xóa database
    DROP DATABASE ParkingDB;
END
GO

-- Tạo database mới
CREATE DATABASE ParkingDB;
GO

-- Chuyển sang sử dụng database vừa tạo
USE ParkingDB;
GO

-- =====================================================
-- 2. TẠO CÁC BẢNG (TABLES)
-- =====================================================

-- ============ BẢNG VAI TRÒ ============
CREATE TABLE VaiTro (
    MaVaiTro INT PRIMARY KEY IDENTITY(1,1),
    TenVaiTro NVARCHAR(50) NOT NULL UNIQUE,
    MoTa NVARCHAR(255)
);
GO

-- ============ BẢNG NGƯỜI DÙNG ============
CREATE TABLE NguoiDung (
    MaND INT PRIMARY KEY IDENTITY(1,1),
    Email VARCHAR(100) NOT NULL UNIQUE,
    MatKhau VARCHAR(100) NOT NULL, -- Hỗ trợ SHA256 hash (64 ký tự)
    HoTen NVARCHAR(100) NOT NULL,
    MaVaiTro INT NOT NULL,
    TrangThai INT DEFAULT 1, -- 1 = Hoạt động, 0 = Khóa
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaVaiTro) REFERENCES VaiTro(MaVaiTro)
);
GO

-- ============ BẢNG KHÁCH HÀNG ============
CREATE TABLE KhachHang (
    MaKH INT PRIMARY KEY IDENTITY(1,1),
    TenKH NVARCHAR(100) NOT NULL,
    SDT VARCHAR(20),
    DiaChi NVARCHAR(255),
    NgayDangKy DATETIME DEFAULT GETDATE()
);
GO

-- ============ BẢNG XE ============
CREATE TABLE Xe (
    MaXe INT PRIMARY KEY IDENTITY(1,1),
    BienSo VARCHAR(20) NOT NULL UNIQUE,
    LoaiXe NVARCHAR(50), -- 'Xe máy', 'Ô tô', 'Xe đạp'
    MaKH INT,
    NgayDangKy DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH) ON DELETE CASCADE
);
GO

-- ============ BẢNG VÉ ============
CREATE TABLE Ve (
    MaVe INT PRIMARY KEY IDENTITY(1,1),
    LoaiVe NVARCHAR(50) NOT NULL, -- 'Vé ngày', 'Vé tháng', 'Vé năm'
    GiaTien DECIMAL(18,2) NOT NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    MaKH INT NOT NULL,
    NgayTao DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH) ON DELETE CASCADE
);
GO

-- ============ BẢNG VỊ TRÍ ĐỖ XE ============
CREATE TABLE ViTriDoXe (
    MaViTri INT PRIMARY KEY IDENTITY(1,1),
    TenViTri VARCHAR(20) NOT NULL UNIQUE, -- 'A01', 'A02', 'B01', etc.
    KhuVuc NVARCHAR(50) NOT NULL, -- 'Khu A - Xe Máy', 'Khu B - Ô Tô'
    LoaiXe NVARCHAR(50) NOT NULL, -- 'Xe máy', 'Ô tô'
    TrangThai NVARCHAR(50) DEFAULT N'Trống', -- 'Trống', 'Đang sử dụng', 'Bảo trì'
    MaLuot INT NULL, -- ID lượt vào hiện tại (nếu đang sử dụng)
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- ============ BẢNG LỊCH SỬ RA VÀO ============
CREATE TABLE LichSuRaVao (
    MaLuot INT PRIMARY KEY IDENTITY(1,1),
    BienSo VARCHAR(20) NOT NULL,
    LoaiXe NVARCHAR(50) NOT NULL, -- 'Xe máy', 'Ô tô'
    ThoiGianVao DATETIME NOT NULL DEFAULT GETDATE(),
    ThoiGianRa DATETIME NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Đang gửi', -- 'Đang gửi', 'Đã ra'
    TienThu DECIMAL(18,2) NULL,
    GhiChu NVARCHAR(255) NULL
);
GO

-- ============ BẢNG BÁO CÁO ============
CREATE TABLE BaoCao (
    MaBaoCao INT PRIMARY KEY IDENTITY(1,1),
    LoaiBaoCao NVARCHAR(100) NOT NULL,
    NguoiTao INT NOT NULL,
    TuNgay DATE NOT NULL,
    DenNgay DATE NOT NULL,
    ThoiGianTao DATETIME DEFAULT GETDATE(),
    
);
GO
-- ============ BẢNG NHẬT KÝ HỆ THỐNG ============
CREATE TABLE NhatKyHeThong (
    MaNhatKy INT PRIMARY KEY IDENTITY(1,1),
    MaNguoiDung INT NOT NULL,
    HanhDong NVARCHAR(100) NOT NULL,
    ChiTiet NVARCHAR(500),
    DiaChiIP VARCHAR(50),
    ThoiGian DATETIME DEFAULT GETDATE(),
    
);
GO

DECLARE @OldAdmin INT = 1; 
DECLARE @NewAdmin INT = 2;
SELECT * FROM NguoiDung WHERE MaND = @NewAdmin;

BEGIN TRANSACTION;

-- 1. CHUYỂN dữ liệu
UPDATE dbo.BaoCao
SET NguoiTao = @NewAdmin
WHERE NguoiTao = @OldAdmin;

UPDATE dbo.NhatKyHeThong
SET MaNguoiDung = @NewAdmin
WHERE MaNguoiDung = @OldAdmin;

-- 2. SAU CÙNG mới xóa admin cũ
DELETE FROM dbo.NguoiDung
WHERE MaND = @OldAdmin;

COMMIT;
GO

-- =====================================================
-- 3. TẠO STORED PROCEDURES
-- =====================================================

-- Procedure log hoạt động hệ thống
CREATE PROCEDURE SP_LogActivity
    @MaNguoiDung INT,
    @HanhDong NVARCHAR(100),
    @ChiTiet NVARCHAR(500),
    @DiaChiIP VARCHAR(50)
AS
BEGIN
    INSERT INTO NhatKyHeThong (MaNguoiDung, HanhDong, ChiTiet, DiaChiIP)
    VALUES (@MaNguoiDung, @HanhDong, @ChiTiet, @DiaChiIP);
END
GO


-- =====================================================
-- 4. THÊM DỮ LIỆU MẪU (SAMPLE DATA)
-- =====================================================

-- ============ VAI TRÒ ============
INSERT INTO VaiTro (TenVaiTro, MoTa) VALUES
(N'Admin', N'Quản trị viên hệ thống'),
(N'Nhân viên', N'Nhân viên quản lý bãi xe'),
(N'Bảo vệ', N'Bảo vệ gác cổng');
GO

-- ============ NGƯỜI DÙNG ============
-- Mật khẩu mặc định: 123456
INSERT INTO NguoiDung (Email, MatKhau, HoTen, MaVaiTro, TrangThai) VALUES
(N'admin@parking.com', N'123456', N'Quản trị viên', 1, 1),
(N'QuanLy@parking.com', N'123456', N'Quản trị viên', 1, 1);
GO

-- ============ KHÁCH HÀNG ============
INSERT INTO KhachHang (TenKH, SDT, DiaChi) VALUES
(N'Nguyễn Minh Đức', '0901234567', N'123 Lê Lợi, Q1, TP.HCM'),
(N'Trần Thị Hoa', '0912345678', N'456 Nguyễn Huệ, Q1, TP.HCM'),
(N'Lê Văn Nam', '0923456789', N'789 Hai Bà Trưng, Q3, TP.HCM'),
(N'Phạm Thị Mai', '0934567890', N'321 Trần Hưng Đạo, Q5, TP.HCM'),
(N'Hoàng Văn Long', '0945678901', N'654 Lý Thường Kiệt, Q10, TP.HCM');
GO

-- ============ XE ============
INSERT INTO Xe (BienSo, LoaiXe, MaKH) VALUES
('51F-12345', N'Xe máy', 1),
('51G-67890', N'Xe máy', 1),
('29A-11111', N'Ô tô', 2),
('59C-22222', N'Xe máy', 3),
('51H-33333', N'Xe máy', 4),
('30B-44444', N'Ô tô', 5);
GO

-- ============ VÉ ============
INSERT INTO Ve (LoaiVe, GiaTien, NgayBatDau, NgayKetThuc, MaKH) VALUES
(N'Vé tháng', 108000, '2024-01-01', '2024-02-01', 1),
(N'Vé tháng', 108000, '2024-01-01', '2024-02-01', 2),
(N'Vé năm', 1000000, '2024-01-01', '2025-01-01', 3),
(N'Vé ngày', 5000, '2024-01-15', '2024-01-16', 4);
GO

-- ============ VỊ TRÍ ĐỖ XE ============

-- Khu A - Xe máy (50 vị trí)
DECLARE @i INT = 1;
WHILE @i <= 50
BEGIN
    INSERT INTO ViTriDoXe (TenViTri, KhuVuc, LoaiXe, TrangThai)
    VALUES (
        'A' + RIGHT('00' + CAST(@i AS VARCHAR), 2), 
        N'Khu A - Xe Máy', 
        N'Xe máy', 
        N'Trống'
    );
    SET @i = @i + 1;
END

-- Khu B - Ô tô (30 vị trí)
SET @i = 1;
WHILE @i <= 30
BEGIN
    INSERT INTO ViTriDoXe (TenViTri, KhuVuc, LoaiXe, TrangThai)
    VALUES (
        'B' + RIGHT('00' + CAST(@i AS VARCHAR), 2), 
        N'Khu B - Ô Tô', 
        N'Ô tô', 
        N'Trống'
    );
    SET @i = @i + 1;
END
GO

-- Đặt một số vị trí đang sử dụng và bảo trì
UPDATE ViTriDoXe SET TrangThai = N'Đang sử dụng' WHERE TenViTri IN ('A01', 'A02', 'A05', 'B01', 'B03');
UPDATE ViTriDoXe SET TrangThai = N'Bảo trì' WHERE TenViTri IN ('A10', 'B10');
GO

-- ============ LỊCH SỬ RA VÀO ============
INSERT INTO LichSuRaVao (BienSo, LoaiXe, ThoiGianVao, ThoiGianRa, TrangThai, TienThu) VALUES
('51F-99999', N'Xe máy', DATEADD(HOUR, -5, GETDATE()), DATEADD(HOUR, -2, GETDATE()), N'Đã ra', 3000),
('29A-88888', N'Ô tô', DATEADD(HOUR, -8, GETDATE()), DATEADD(HOUR, -1, GETDATE()), N'Đã ra', 50000),
('51G-77777', N'Xe máy', DATEADD(HOUR, -10, GETDATE()), DATEADD(HOUR, -3, GETDATE()), N'Đã ra', 3000),
('51F-12345', N'Xe máy', DATEADD(HOUR, -2, GETDATE()), NULL, N'Đang gửi', NULL),
('29A-11111', N'Ô tô', DATEADD(HOUR, -3, GETDATE()), NULL, N'Đang gửi', NULL);
GO

-- Cập nhật MaLuot cho vị trí đang sử dụng
UPDATE ViTriDoXe SET MaLuot = (SELECT MaLuot FROM LichSuRaVao WHERE BienSo = '51F-12345') WHERE TenViTri = 'A01';
UPDATE ViTriDoXe SET MaLuot = (SELECT MaLuot FROM LichSuRaVao WHERE BienSo = '29A-11111') WHERE TenViTri = 'B01';
GO

-- =====================================================
-- 5. KIỂM TRA VÀ THỐNG KÊ
-- =====================================================

-- Kiểm tra số lượng bảng
SELECT COUNT(*) AS [Số bảng đã tạo] 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';
GO

-- Thống kê dữ liệu
SELECT 'VaiTro' AS [Bảng], COUNT(*) AS [Số dòng] FROM VaiTro
UNION ALL
SELECT 'NguoiDung', COUNT(*) FROM NguoiDung
UNION ALL
SELECT 'KhachHang', COUNT(*) FROM KhachHang
UNION ALL
SELECT 'Xe', COUNT(*) FROM Xe
UNION ALL
SELECT 'Ve', COUNT(*) FROM Ve
UNION ALL
SELECT 'ViTriDoXe', COUNT(*) FROM ViTriDoXe
UNION ALL
SELECT 'LichSuRaVao', COUNT(*) FROM LichSuRaVao
UNION ALL
SELECT 'BaoCao', COUNT(*) FROM BaoCao
UNION ALL
SELECT 'NhatKyHeThong', COUNT(*) FROM NhatKyHeThong;
GO

-- Kiểm tra trạng thái vị trí đỗ xe
SELECT 
    KhuVuc,
    COUNT(*) AS [Tổng],
    SUM(CASE WHEN TrangThai = N'Trống' THEN 1 ELSE 0 END) AS [Trống],
    SUM(CASE WHEN TrangThai = N'Đang sử dụng' THEN 1 ELSE 0 END) AS [Đang dùng],
    SUM(CASE WHEN TrangThai = N'Bảo trì' THEN 1 ELSE 0 END) AS [Bảo trì]
FROM ViTriDoXe
GROUP BY KhuVuc;
GO

-- =====================================================
-- 6. TẠO INDEX ĐỂ TỐI ƯU HIỆU NĂNG
-- =====================================================

CREATE INDEX IX_NguoiDung_Email ON NguoiDung(Email);
CREATE INDEX IX_Xe_BienSo ON Xe(BienSo);
CREATE INDEX IX_LichSuRaVao_BienSo ON LichSuRaVao(BienSo);
CREATE INDEX IX_LichSuRaVao_TrangThai ON LichSuRaVao(TrangThai);
CREATE INDEX IX_ViTriDoXe_TrangThai ON ViTriDoXe(TrangThai);
CREATE INDEX IX_Ve_MaKH ON Ve(MaKH);
GO
