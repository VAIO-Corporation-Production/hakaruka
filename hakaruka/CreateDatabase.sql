-- データベース作成スクリプト
-- SQL Serverで実行してください

-- データベースを作成
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'hakaruka')
BEGIN
    CREATE DATABASE hakaruka;
END
GO

USE hakaruka;
GO

-- テーブルを作成
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ループ]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ループ] (
        [ID] INT IDENTITY(1,1) PRIMARY KEY,
        [Serial] NVARCHAR(50) NOT NULL,
        [Unit] NVARCHAR(50) NOT NULL,
        [Result] NVARCHAR(10) NOT NULL,
[Weight] DECIMAL(10,2) NOT NULL,
   [reg_time] DATETIME NOT NULL
    );
    
    PRINT 'テーブル [ループ] を作成しました。';
END
ELSE
BEGIN
    PRINT 'テーブル [ループ] は既に存在します。';
END
GO

-- サンプルデータを挿入（オプション）
-- INSERT INTO [dbo].[ループ] (Serial, Unit, Result, Weight, reg_time)
-- VALUES ('TEST001', '段ボール1箱', 'OK', 14.5, GETDATE());

-- データを確認
SELECT TOP 10 * FROM [dbo].[ループ] ORDER BY reg_time DESC;
GO
