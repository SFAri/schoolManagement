BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250327045945_Grade_Add_GradeShortName')
BEGIN
    ALTER TABLE [Students] ADD [DateOfBirth] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250327045945_Grade_Add_GradeShortName')
BEGIN
    ALTER TABLE [Students] ADD [Height] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250327045945_Grade_Add_GradeShortName')
BEGIN
    ALTER TABLE [Students] ADD [Photo] varbinary(max) NOT NULL DEFAULT 0x;
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250327045945_Grade_Add_GradeShortName')
BEGIN
    ALTER TABLE [Students] ADD [Weight] real NOT NULL DEFAULT CAST(0 AS real);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250327045945_Grade_Add_GradeShortName')
BEGIN
    ALTER TABLE [Grades] ADD [GradeShortName] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250327045945_Grade_Add_GradeShortName')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250327045945_Grade_Add_GradeShortName', N'6.0.36');
END;
GO

COMMIT;
GO

