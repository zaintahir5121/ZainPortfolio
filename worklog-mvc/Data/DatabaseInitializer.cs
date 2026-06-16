using Microsoft.EntityFrameworkCore;

namespace WorkLogApp.Data;

public static class DatabaseInitializer
{
    public static void Initialize(AppDbContext ctx, ILogger logger)
    {
        bool isSqlite = ctx.Database.ProviderName?.Contains("Sqlite", StringComparison.OrdinalIgnoreCase) == true;

        try
        {
            bool created = ctx.Database.EnsureCreated();
            if (created) logger.LogInformation("WorkLog: database created.");

            if (isSqlite) { AppDbContext.Seed(ctx); return; }

            EnsureAllTables(ctx, logger);
            EnsureAllColumns(ctx, logger);
            AppDbContext.Seed(ctx);
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == -1 || ex.Number == 2)
        {
            logger.LogError("WorkLog: cannot reach SQL Server.\n{Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "WorkLog: database initialization failed.");
            throw;
        }
    }

    static void EnsureAllTables(AppDbContext ctx, ILogger logger)
    {
        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
            CREATE TABLE Users (
                Id           INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
                Username     NVARCHAR(100)  NOT NULL DEFAULT N'',
                PasswordHash NVARCHAR(500)  NOT NULL DEFAULT N'',
                Name         NVARCHAR(200)  NOT NULL DEFAULT N'',
                Role         NVARCHAR(50)   NOT NULL DEFAULT N'user',
                ApiKey       NVARCHAR(100)  NULL,
                CreatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
            )
            """, "Users", logger);

        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Notes')
            CREATE TABLE Notes (
                Id         INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_Notes PRIMARY KEY,
                UserId     INT            NOT NULL,
                Title      NVARCHAR(500)  NOT NULL DEFAULT N'',
                Content    NVARCHAR(MAX)  NOT NULL DEFAULT N'',
                Color      NVARCHAR(50)   NOT NULL DEFAULT N'default',
                IsPinned   BIT            NOT NULL DEFAULT 0,
                IsArchived BIT            NOT NULL DEFAULT 0,
                IsDeleted  BIT            NOT NULL DEFAULT 0,
                NoteType   NVARCHAR(20)   NOT NULL DEFAULT N'note',
                CreatedAt  DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                UpdatedAt  DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )
            """, "Notes", logger);

        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ChecklistItems')
            CREATE TABLE ChecklistItems (
                Id        INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_ChecklistItems PRIMARY KEY,
                NoteId    INT            NOT NULL,
                Text      NVARCHAR(1000) NOT NULL DEFAULT N'',
                IsChecked BIT            NOT NULL DEFAULT 0,
                SortOrder INT            NOT NULL DEFAULT 0,
                CONSTRAINT FK_ChecklistItems_Notes FOREIGN KEY (NoteId) REFERENCES Notes(Id) ON DELETE CASCADE
            )
            """, "ChecklistItems", logger);

        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WorkEntries')
            CREATE TABLE WorkEntries (
                Id         INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_WorkEntries PRIMARY KEY,
                UserId     INT            NOT NULL,
                RawText    NVARCHAR(MAX)  NOT NULL DEFAULT N'',
                LogDate    DATETIME2      NOT NULL,
                CreatedAt  DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                TotalHours REAL           NOT NULL DEFAULT 0,
                IsAiParsed BIT            NOT NULL DEFAULT 0,
                CONSTRAINT FK_WorkEntries_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_WorkEntries_UserId')
                CREATE INDEX IX_WorkEntries_UserId ON WorkEntries (UserId);
            """, "WorkEntries", logger);

        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WorkTasks')
            CREATE TABLE WorkTasks (
                Id           INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_WorkTasks PRIMARY KEY,
                WorkEntryId  INT            NOT NULL,
                Description  NVARCHAR(MAX)  NOT NULL DEFAULT N'',
                Hours        REAL           NOT NULL DEFAULT 0,
                Project      NVARCHAR(200)  NULL,
                Category     NVARCHAR(50)   NOT NULL DEFAULT N'general',
                SortOrder    INT            NOT NULL DEFAULT 0,
                CONSTRAINT FK_WorkTasks_WorkEntries FOREIGN KEY (WorkEntryId) REFERENCES WorkEntries(Id) ON DELETE CASCADE
            )
            """, "WorkTasks", logger);
    }

    static void EnsureAllColumns(AppDbContext ctx, ILogger logger)
    {
        Col(ctx, "Users", "ApiKey", "NVARCHAR(100) NULL", logger);
    }

    static void Exec(AppDbContext ctx, string sql, string name, ILogger logger)
    {
        try   { ctx.Database.ExecuteSqlRaw(sql); }
        catch (Exception ex) { logger.LogError(ex, "WorkLog: failed ensuring table {Name}.", name); throw; }
    }

    static void Col(AppDbContext ctx, string table, string column, string definition, ILogger logger)
    {
        var sql = $"""
            IF NOT EXISTS (
                SELECT 1 FROM sys.columns
                WHERE object_id = OBJECT_ID(N'{table}') AND name = N'{column}'
            )
                ALTER TABLE {table} ADD {column} {definition}
            """;
        try   { ctx.Database.ExecuteSqlRaw(sql); }
        catch (Exception ex) { logger.LogError(ex, "WorkLog: failed ensuring column {Table}.{Column}.", table, column); throw; }
    }
}
