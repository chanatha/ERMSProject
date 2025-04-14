-- Get all WorkTasks
ALTER PROCEDURE GetAllWorkTasks
AS
BEGIN
    SELECT * FROM Tasks
END
Go

-- Get WorkTask by Id
ALTER PROCEDURE GetWorkTaskById
    @Id INT
AS
BEGIN
    SELECT * FROM Tasks WHERE Id = @Id
END
Go

-- Insert WorkTask
ALTER PROCEDURE InsertWorkTask
    @Title NVARCHAR(100),
    @Description NVARCHAR(500),
    @Status NVARCHAR(50),
    @Priority NVARCHAR(50),
    @DueDate DATE,
    @ProjectId INT,
    @EmployeeId INT = NULL
AS
BEGIN
    INSERT INTO Tasks (Title, Description, Status, Priority, DueDate, ProjectId, EmployeeId)
    VALUES (@Title, @Description, @Status, @Priority, @DueDate, @ProjectId, @EmployeeId)
END
Go

-- Update WorkTask
ALTER PROCEDURE UpdateWorkTask
    @Id INT,
    @Title NVARCHAR(100),
    @Description NVARCHAR(500),
    @Status NVARCHAR(50),
    @Priority NVARCHAR(50),
    @DueDate DATE,
    @ProjectId INT,
    @EmployeeId INT = NULL
AS
BEGIN
    UPDATE Tasks
    SET Title = @Title,
        Description = @Description,
        Status = @Status,
        Priority = @Priority,
        DueDate = @DueDate,
        ProjectId = @ProjectId,
        EmployeeId = @EmployeeId
    WHERE Id = @Id
END
Go

-- Delete WorkTask
ALTER PROCEDURE DeleteWorkTask
    @Id INT
AS
BEGIN
    DELETE FROM Tasks WHERE Id = @Id
END
Go