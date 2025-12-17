/* =====================================================
   RESET DATABASE
===================================================== */
DROP DATABASE IF EXISTS ParkingDB;
GO
CREATE DATABASE ParkingDB;
GO
USE ParkingDB;
GO

/* =====================================================
   TABLE VaiTro
===================================================== */
CREATE TABLE VaiTro (
    MaVaiTro INT IDENTITY PRIMARY KEY,
    TenVaiTro NVARCHAR(50)
);

INSERT INTO VaiTro (TenVaiTro)
VALUES (N'Admin'), (N'Staff');
GO

/* =====================================================
   TABLE NguoiDung
===================================================== */
CREATE TABLE NguoiDung (
    MaND INT IDENTITY PRIMARY KEY,
    Email VARCHAR(50) UNIQUE NOT NULL,
    MatKhau VARCHAR(50) NOT NULL,
    HoTen NVARCHAR(100),
    MaVaiTro INT NOT NULL,
    TrangThai BIT DEFAULT 1,
    FOREIGN KEY (MaVaiTro) REFERENCES VaiTro(MaVaiTro)
);
GO
INSERT INTO NguoiDung
    (Email, MatKhau, HoTen, MaVaiTro, TrangThai)
VALUES
    (
        'admin@parking.com','123456',N'Quản trị hệ thống',
        (SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Admin'),1
    );
GO

/* =====================================================
   TABLE KhachHang
===================================================== */
CREATE TABLE KhachHang (
    MaKH INT IDENTITY PRIMARY KEY,
    TenKH NVARCHAR(100),
    SDT VARCHAR(15),
    DiaChi NVARCHAR(255)
);
GO

/* =====================================================
   TABLE Xe
===================================================== */
CREATE TABLE Xe (
    MaXe INT IDENTITY PRIMARY KEY,
    BienSo VARCHAR(20) UNIQUE NOT NULL,
    LoaiXe NVARCHAR(20) NOT NULL,  -- Xe máy / Ô tô
    MaKH INT NULL,
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
GO

/* =====================================================
   TABLE Ve
===================================================== */
CREATE TABLE Ve (
    MaVe INT IDENTITY PRIMARY KEY,
    LoaiVe NVARCHAR(50),   -- Vé tháng
    GiaTien INT,
    NgayBatDau DATE,
    NgayKetThuc DATE,
    MaKH INT,
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
);
GO

/* =====================================================
   TABLE LichSuRaVao
===================================================== */
CREATE TABLE LichSuRaVao (
    MaLuot INT IDENTITY PRIMARY KEY,
    BienSo VARCHAR(20),
    LoaiXe NVARCHAR(20),          -- Xe máy / Ô tô
    ThoiGianVao DATETIME NOT NULL,
    ThoiGianRa DATETIME NULL,
    TrangThai NVARCHAR(20),
    TienThu INT NULL
);
GO

/* =====================================================
   TABLE DoanhThu
===================================================== */
CREATE TABLE DoanhThu (
    ID INT IDENTITY PRIMARY KEY,
    BienSo VARCHAR(20),
    LoaiXe NVARCHAR(20),
    SoTien INT,
    NgayThu DATETIME DEFAULT GETDATE(),
    GhiChu NVARCHAR(255)
);
GO

/* =====================================================
   VIEW DoanhThu
===================================================== */
CREATE OR ALTER VIEW VW_DoanhThu AS
SELECT 
    MaLuot,
    BienSo,
    LoaiXe,
    ThoiGianVao,
    ThoiGianRa,
    TienThu AS SoTien
FROM LichSuRaVao
WHERE TienThu IS NOT NULL;
GO

/* =====================================================
   PROCEDURE XE VÀO
===================================================== */
CREATE OR ALTER PROCEDURE SP_XeVao
    @BienSo VARCHAR(20),
    @LoaiXe NVARCHAR(20)
AS
BEGIN
    INSERT INTO LichSuRaVao
        (BienSo, LoaiXe, ThoiGianVao, TrangThai)
    VALUES
        (@BienSo, @LoaiXe, GETDATE(), N'Đang gửi');
END
GO

/* =====================================================
   PROCEDURE XE RA
===================================================== */
CREATE OR ALTER PROCEDURE SP_XeRa
    @BienSo VARCHAR(20),
    @MatThe BIT = 0
AS
BEGIN
    DECLARE 
        @MaLuot INT,
        @LoaiXe NVARCHAR(20),
        @PhiGui INT,
        @LyDo NVARCHAR(255);

    SELECT TOP 1 
        @MaLuot = MaLuot,
        @LoaiXe = LoaiXe
    FROM LichSuRaVao
    WHERE BienSo = @BienSo
      AND TrangThai = N'Đang gửi'
    ORDER BY MaLuot DESC;

    IF @MaLuot IS NULL
    BEGIN
        RAISERROR(N'Xe không có trong hầm!', 16, 1);
        RETURN;
    END;

    -- Phí theo loại xe
    IF @LoaiXe = N'Ô tô'
        SET @PhiGui = 50000;
    ELSE
        SET @PhiGui = 3000;

    SET @LyDo = N'Thu phí theo loại xe: ' + @LoaiXe;

    IF @MatThe = 1
    BEGIN
        SET @PhiGui = @PhiGui + 50000;
        SET @LyDo = @LyDo + N', mất thẻ (+50.000đ)';
    END

    UPDATE LichSuRaVao
    SET ThoiGianRa = GETDATE(),
        TrangThai = N'Đã ra',
        TienThu = @PhiGui
    WHERE MaLuot = @MaLuot;

    INSERT INTO DoanhThu
        (BienSo, LoaiXe, SoTien, GhiChu)
    VALUES
        (@BienSo, @LoaiXe, @PhiGui, @LyDo);

    SELECT @LyDo AS ThongBao, @PhiGui AS SoTienThu;
END
GO
