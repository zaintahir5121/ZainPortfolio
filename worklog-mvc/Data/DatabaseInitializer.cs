using Microsoft.EntityFrameworkCore;

namespace WorkLogApp.Data;

/// <summary>
/// Idempotent schema initializer — safe to run on every startup.
/// Handles new installs, upgrades from any older version, and missing columns.
/// No EF migration files needed.
/// </summary>
public static class DatabaseInitializer
{
    public static void Initialize(AppDbContext ctx, ILogger logger)
    {
        try
        {
            // ── 1. Ensure the database itself exists ─────────────────────────
            // EnsureCreated() creates the full schema for brand-new databases.
            // For existing databases it is a no-op (returns false).
            bool created = ctx.Database.EnsureCreated();
            if (created)
                logger.LogInformation("WorkLog: database created for the first time.");

            // ── 2. Ensure every table exists (idempotent) ────────────────────
            // Handles databases that existed before some tables were added.
            EnsureAllTables(ctx, logger);

            // ── 3. Ensure every column exists (idempotent) ───────────────────
            // Handles databases where columns were added in later versions.
            EnsureAllColumns(ctx, logger);

            // ── 4. Seed default users if empty ───────────────────────────────
            AppDbContext.Seed(ctx);
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == -1 || ex.Number == 2)
        {
            logger.LogError("WorkLog: cannot reach SQL Server — check your connection string and that SQL Server is running.\n{Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "WorkLog: database initialization failed.");
            throw;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // TABLE CREATION  (each block is a no-op if the table already exists)
    // ─────────────────────────────────────────────────────────────────────────
    static void EnsureAllTables(AppDbContext ctx, ILogger logger)
    {
        // Users
        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Users')
            CREATE TABLE Users (
                Id           INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
                Username     NVARCHAR(100)  NOT NULL DEFAULT N'',
                PasswordHash NVARCHAR(500)  NOT NULL DEFAULT N'',
                Name         NVARCHAR(200)  NOT NULL DEFAULT N'',
                Role         NVARCHAR(50)   NOT NULL DEFAULT N'employee',
                CreatedAt    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
            )
            """, "Users", logger);

        // LogEntries
        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LogEntries')
            CREATE TABLE LogEntries (
                Id          INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_LogEntries PRIMARY KEY,
                UserId      INT            NOT NULL,
                Date        DATE           NOT NULL DEFAULT GETDATE(),
                Project     NVARCHAR(200)  NOT NULL DEFAULT N'',
                Description NVARCHAR(2000) NOT NULL DEFAULT N'',
                Hours       DECIMAL(5,2)   NOT NULL DEFAULT 0,
                Tags        NVARCHAR(500)  NOT NULL DEFAULT N'',
                CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_LogEntries_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )
            """, "LogEntries", logger);

        // RecurringEntries
        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RecurringEntries')
            CREATE TABLE RecurringEntries (
                Id            INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_RecurringEntries PRIMARY KEY,
                UserId        INT            NOT NULL,
                Project       NVARCHAR(200)  NOT NULL DEFAULT N'',
                Description   NVARCHAR(1000) NOT NULL DEFAULT N'',
                Hours         DECIMAL(5,2)   NOT NULL DEFAULT 0,
                Tags          NVARCHAR(200)  NOT NULL DEFAULT N'',
                Schedule      NVARCHAR(100)  NOT NULL DEFAULT N'weekdays',
                IsActive      BIT            NOT NULL DEFAULT 1,
                LastFiredDate DATE           NULL,
                CreatedAt     DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_RecurringEntries_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )
            """, "RecurringEntries", logger);

        // Achievements
        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Achievements')
            CREATE TABLE Achievements (
                Id          INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_Achievements PRIMARY KEY,
                UserId      INT            NOT NULL,
                Title       NVARCHAR(300)  NOT NULL DEFAULT N'',
                Description NVARCHAR(2000) NOT NULL DEFAULT N'',
                Date        DATE           NOT NULL DEFAULT GETDATE(),
                Category    NVARCHAR(50)   NOT NULL DEFAULT N'work',
                CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_Achievements_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )
            """, "Achievements", logger);

        // Experiences
        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Experiences')
            CREATE TABLE Experiences (
                Id          INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_Experiences PRIMARY KEY,
                UserId      INT            NOT NULL,
                Company     NVARCHAR(200)  NOT NULL DEFAULT N'',
                Role        NVARCHAR(200)  NOT NULL DEFAULT N'',
                Location    NVARCHAR(200)  NOT NULL DEFAULT N'',
                StartDate   DATE           NOT NULL DEFAULT GETDATE(),
                EndDate     DATE           NULL,
                IsCurrent   BIT            NOT NULL DEFAULT 0,
                Description NVARCHAR(2000) NOT NULL DEFAULT N'',
                Tags        NVARCHAR(500)  NOT NULL DEFAULT N'',
                CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_Experiences_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )
            """, "Experiences", logger);

        // LearningItems
        Exec(ctx, """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LearningItems')
            CREATE TABLE LearningItems (
                Id            INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_LearningItems PRIMARY KEY,
                UserId        INT            NOT NULL,
                Title         NVARCHAR(300)  NOT NULL DEFAULT N'',
                Type          NVARCHAR(50)   NOT NULL DEFAULT N'course',
                Source        NVARCHAR(200)  NOT NULL DEFAULT N'',
                Status        NVARCHAR(50)   NOT NULL DEFAULT N'in-progress',
                Notes         NVARCHAR(2000) NOT NULL DEFAULT N'',
                StartedDate   DATE           NULL,
                CompletedDate DATE           NULL,
                Rating        INT            NOT NULL DEFAULT 0,
                CreatedAt     DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT FK_LearningItems_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            )
            """, "LearningItems", logger);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // COLUMN ADDITIONS  (each block is a no-op if the column already exists)
    // Add an entry here whenever a new column is added to a model.
    // ─────────────────────────────────────────────────────────────────────────
    static void EnsureAllColumns(AppDbContext ctx, ILogger logger)
    {
        // LogEntries.Tags
        Col(ctx, "LogEntries",       "Tags",          "NVARCHAR(500)  NOT NULL DEFAULT N''",  logger);
        // RecurringEntries additions
        Col(ctx, "RecurringEntries", "Tags",          "NVARCHAR(200)  NOT NULL DEFAULT N''",  logger);
        Col(ctx, "RecurringEntries", "IsActive",      "BIT            NOT NULL DEFAULT 1",    logger);
        Col(ctx, "RecurringEntries", "LastFiredDate", "DATE           NULL",                  logger);
        Col(ctx, "RecurringEntries", "Schedule",      "NVARCHAR(100)  NOT NULL DEFAULT N'weekdays'", logger);
        // LearningItems.Notes
        Col(ctx, "LearningItems",    "Notes",         "NVARCHAR(2000) NOT NULL DEFAULT N''",  logger);
        // Experience.Tags
        Col(ctx, "Experiences",      "Tags",          "NVARCHAR(500)  NOT NULL DEFAULT N''",  logger);
    }

    // ─────────────────────────────────────────────────────────────────────────

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
