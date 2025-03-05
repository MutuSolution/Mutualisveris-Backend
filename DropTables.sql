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
IF SCHEMA_ID(N'User') IS NULL EXEC(N'CREATE SCHEMA [User];');

IF SCHEMA_ID(N'Cart') IS NULL EXEC(N'CREATE SCHEMA [Cart];');

IF SCHEMA_ID(N'Catalog') IS NULL EXEC(N'CREATE SCHEMA [Catalog];');

IF SCHEMA_ID(N'Order') IS NULL EXEC(N'CREATE SCHEMA [Order];');

IF SCHEMA_ID(N'Security') IS NULL EXEC(N'CREATE SCHEMA [Security];');

CREATE TABLE [Catalog].[Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [ParentCategoryId] int NULL,
    [IsVisible] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Catalog].[Categories] ([Id])
);

CREATE TABLE [Catalog].[ProductReports] (
    [Id] int NOT NULL IDENTITY,
    [ProductId] int NOT NULL,
    [Message] nvarchar(max) NULL,
    [IsChecked] bit NOT NULL,
    CONSTRAINT [PK_ProductReports] PRIMARY KEY ([Id])
);

CREATE TABLE [Security].[Roles] (
    [Id] nvarchar(450) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

CREATE TABLE [Security].[Users] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [RefreshToken] nvarchar(max) NULL,
    [Role] nvarchar(max) NULL,
    [RefreshTokenExpiryDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [RegisteredAt] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Catalog].[Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(250) NOT NULL,
    [Description] nvarchar(2000) NULL,
    [Price] decimal(18,2) NOT NULL,
    [StockQuantity] int NOT NULL DEFAULT 0,
    [SKU] nvarchar(50) NOT NULL,
    [IsPublic] bit NOT NULL DEFAULT CAST(1 AS bit),
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NULL DEFAULT (GETUTCDATE()),
    [CategoryId] int NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Catalog].[Categories] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Security].[RoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [Description] nvarchar(max) NULL,
    [Group] nvarchar(max) NULL,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Security].[Roles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [User].[Addresses] (
    [Id] int NOT NULL IDENTITY,
    [Street] nvarchar(200) NOT NULL,
    [City] nvarchar(100) NOT NULL,
    [Country] nvarchar(100) NOT NULL,
    [ZipCode] nvarchar(20) NOT NULL,
    [Type] int NOT NULL,
    [UserId] nvarchar(450) NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Addresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Cart].[Carts] (
    [Id] int NOT NULL IDENTITY,
    [CreatedAt] datetime2 NOT NULL,
    [UserId] nvarchar(450) NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Carts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Order].[Orders] (
    [Id] int NOT NULL IDENTITY,
    [OrderDate] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [UserId] nvarchar(450) NULL,
    [ApplicationUserId] nvarchar(450) NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [Security].[Users] ([Id]),
    CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Security].[UserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Security].[UserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Security].[UserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Security].[Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Security].[UserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Catalog].[Likes] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(450) NULL,
    [ProductId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Likes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Likes_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Catalog].[Products] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Catalog].[ProductImages] (
    [Id] int NOT NULL IDENTITY,
    [ImageUrl] nvarchar(500) NOT NULL,
    [IsMain] bit NOT NULL DEFAULT CAST(0 AS bit),
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Catalog].[Products] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Cart].[CartItems] (
    [Id] int NOT NULL IDENTITY,
    [Quantity] int NOT NULL,
    [ProductId] int NOT NULL,
    [CartId] int NOT NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Cart].[Carts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Catalog].[Products] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Order].[OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [ProductId] int NOT NULL,
    [OrderId] int NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order].[Orders] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Catalog].[Products] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Order].[Payments] (
    [Id] int NOT NULL IDENTITY,
    [Amount] decimal(18,2) NOT NULL,
    [Method] int NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [OrderId] int NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Order].[Orders] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Addresses_UserId] ON [User].[Addresses] ([UserId]);

CREATE INDEX [IX_CartItems_CartId] ON [Cart].[CartItems] ([CartId]);

CREATE INDEX [IX_CartItems_ProductId] ON [Cart].[CartItems] ([ProductId]);

CREATE UNIQUE INDEX [IX_Carts_UserId] ON [Cart].[Carts] ([UserId]) WHERE [UserId] IS NOT NULL;

CREATE INDEX [IX_Categories_Name] ON [Catalog].[Categories] ([Name]);

CREATE INDEX [IX_Categories_ParentCategoryId] ON [Catalog].[Categories] ([ParentCategoryId]);

CREATE INDEX [IX_Likes_Id] ON [Catalog].[Likes] ([Id]);

CREATE INDEX [IX_Likes_ProductId] ON [Catalog].[Likes] ([ProductId]);

CREATE INDEX [IX_Likes_UserName] ON [Catalog].[Likes] ([UserName]);

CREATE INDEX [IX_OrderItems_OrderId] ON [Order].[OrderItems] ([OrderId]);

CREATE INDEX [IX_OrderItems_ProductId] ON [Order].[OrderItems] ([ProductId]);

CREATE INDEX [IX_Orders_ApplicationUserId] ON [Order].[Orders] ([ApplicationUserId]);

CREATE INDEX [IX_Orders_UserId] ON [Order].[Orders] ([UserId]);

CREATE UNIQUE INDEX [IX_Payments_OrderId] ON [Order].[Payments] ([OrderId]);

CREATE INDEX [IX_ProductImages_ProductId] ON [Catalog].[ProductImages] ([ProductId]);

CREATE INDEX [IX_ProductReports_Id] ON [Catalog].[ProductReports] ([Id]);

CREATE INDEX [IX_ProductReports_ProductId] ON [Catalog].[ProductReports] ([ProductId]);

CREATE INDEX [IX_Products_CategoryId] ON [Catalog].[Products] ([CategoryId]);

CREATE INDEX [IX_Products_Name] ON [Catalog].[Products] ([Name]);

CREATE UNIQUE INDEX [IX_Products_SKU] ON [Catalog].[Products] ([SKU]);

CREATE INDEX [IX_Products_Visibility] ON [Catalog].[Products] ([IsPublic], [IsDeleted]);

CREATE INDEX [IX_RoleClaims_RoleId] ON [Security].[RoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [Security].[Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_UserClaims_UserId] ON [Security].[UserClaims] ([UserId]);

CREATE INDEX [IX_UserLogins_UserId] ON [Security].[UserLogins] ([UserId]);

CREATE INDEX [IX_UserRoles_RoleId] ON [Security].[UserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [Security].[Users] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [Security].[Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250305130124_ConfigAndEntitesUpdated', N'9.0.2');

COMMIT;
GO

