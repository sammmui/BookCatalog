CREATE TABLE Users (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Email            NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash     NVARCHAR(500) NULL,
    DisplayName      NVARCHAR(100) NOT NULL,
    AvatarPath       NVARCHAR(500) NULL,
    GoogleId         NVARCHAR(100) NULL,
    SubscriptionPlan NVARCHAR(20) NOT NULL DEFAULT 'Free',
    DailyGoalPages   INT NOT NULL DEFAULT 30,
    CreatedAt        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE Authors (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(200) NOT NULL,
    BirthPlace  NVARCHAR(200) NULL,
    BirthYear   INT NULL,
    Latitude    FLOAT NULL,
    Longitude   FLOAT NULL
);

CREATE TABLE Genres (
    Id    INT IDENTITY(1,1) PRIMARY KEY,
    Name  NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Books (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Title        NVARCHAR(300) NOT NULL,
    [Year]       INT NULL,
    Pages        INT NOT NULL DEFAULT 0,
    Isbn         NVARCHAR(20) NULL,
    Description  NVARCHAR(MAX) NULL,
    CoverPath    NVARCHAR(500) NULL,
    AuthorId     INT NOT NULL,
    CONSTRAINT FK_Books_Authors FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
);
CREATE INDEX IX_Books_Title ON Books(Title);

CREATE TABLE BookGenres (
    BookId   INT NOT NULL,
    GenreId  INT NOT NULL,
    CONSTRAINT PK_BookGenres PRIMARY KEY (BookId, GenreId),
    CONSTRAINT FK_BookGenres_Books  FOREIGN KEY (BookId)  REFERENCES Books(Id)  ON DELETE CASCADE,
    CONSTRAINT FK_BookGenres_Genres FOREIGN KEY (GenreId) REFERENCES Genres(Id) ON DELETE CASCADE
);

CREATE TABLE UserBooks (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    UserId       INT NOT NULL,
    BookId       INT NOT NULL,
    Status       NVARCHAR(20) NOT NULL DEFAULT 'WantToRead'
                 CHECK (Status IN ('WantToRead','Reading','Read')),
    Rating       INT NULL CHECK (Rating BETWEEN 1 AND 5),
    CurrentPage  INT NOT NULL DEFAULT 0,
    IsFavourite  BIT NOT NULL DEFAULT 0,
    StartedAt    DATE NULL,
    FinishedAt   DATE NULL,
    AddedAt      DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_UserBooks UNIQUE (UserId, BookId),
    CONSTRAINT FK_UserBooks_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UserBooks_Books FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE CASCADE
);

CREATE TABLE ReadingLogs (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    UserBookId  INT NOT NULL,
    LogDate     DATE NOT NULL DEFAULT CAST(SYSUTCDATETIME() AS DATE),
    PagesRead   INT NOT NULL CHECK (PagesRead > 0),
    CONSTRAINT FK_ReadingLogs_UserBooks FOREIGN KEY (UserBookId) REFERENCES UserBooks(Id) ON DELETE CASCADE
);
CREATE INDEX IX_ReadingLogs_Date ON ReadingLogs(LogDate);

CREATE TABLE UserAchievements (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    UserId           INT NOT NULL,
    AchievementCode  NVARCHAR(50) NOT NULL,
    EarnedAt         DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_UserAchievements UNIQUE (UserId, AchievementCode),
    CONSTRAINT FK_UA_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE Notes (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    UserBookId  INT NOT NULL,
    Content     NVARCHAR(MAX) NOT NULL,
    SortOrder   INT NOT NULL DEFAULT 0,
    UpdatedAt   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Notes_UserBooks FOREIGN KEY (UserBookId) REFERENCES UserBooks(Id) ON DELETE CASCADE
);