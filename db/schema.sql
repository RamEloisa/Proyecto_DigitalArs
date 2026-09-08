IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Roles] (
    [ID_Role] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([ID_Role])
);

CREATE TABLE [Users] (
    [ID_User] int NOT NULL IDENTITY,
    [Full_Name] nvarchar(150) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [Password_Hasheada] nvarchar(max) NOT NULL,
    [DNI] nvarchar(20) NOT NULL,
    [Alias] nvarchar(50) NOT NULL,
    [ID_Role] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([ID_User]),
    CONSTRAINT [FK_Users_Roles_ID_Role] FOREIGN KEY ([ID_Role]) REFERENCES [Roles] ([ID_Role]) ON DELETE NO ACTION
);

CREATE TABLE [Accounts] (
    [ID_Account] int NOT NULL IDENTITY,
    [ID_User] int NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Date] datetime2 NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([ID_Account]),
    CONSTRAINT [FK_Accounts_Users_ID_User] FOREIGN KEY ([ID_User]) REFERENCES [Users] ([ID_User]) ON DELETE NO ACTION
);

CREATE TABLE [Transactions] (
    [ID_Transaction] int NOT NULL IDENTITY,
    [ID_Account] int NOT NULL,
    [Type] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Date_Transaction] datetime2 NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([ID_Transaction]),
    CONSTRAINT [FK_Transactions_Accounts_ID_Account] FOREIGN KEY ([ID_Account]) REFERENCES [Accounts] ([ID_Account]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Accounts_ID_User] ON [Accounts] ([ID_User]);

CREATE UNIQUE INDEX [IX_Roles_Name] ON [Roles] ([Name]);

CREATE INDEX [IX_Transactions_Date_Transaction] ON [Transactions] ([Date_Transaction]);

CREATE INDEX [IX_Transactions_ID_Account] ON [Transactions] ([ID_Account]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE INDEX [IX_Users_ID_Role] ON [Users] ([ID_Role]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260827173110_InitialCreate', N'10.0.11');

COMMIT;
GO

