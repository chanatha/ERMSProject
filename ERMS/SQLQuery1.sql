-- Get all WorkTasks
CREATE PROCEDURE GetAllWorkTasks
AS
BEGIN
    SELECT * FROM WorkTasks
END
Go

-- Get WorkTask by Id
CREATE PROCEDURE GetWorkTaskById
    @Id INT
AS
BEGIN
    SELECT * FROM WorkTasks WHERE Id = @Id
END
Go

-- Insert WorkTask
CREATE PROCEDURE InsertWorkTask
    @Title NVARCHAR(100),
    @Description NVARCHAR(500),
    @Status NVARCHAR(50),
    @Priority NVARCHAR(50),
    @DueDate DATE,
    @ProjectId INT,
    @EmployeeId INT = NULL
AS
BEGIN
    INSERT INTO WorkTasks (Title, Description, Status, Priority, DueDate, ProjectId, EmployeeId)
    VALUES (@Title, @Description, @Status, @Priority, @DueDate, @ProjectId, @EmployeeId)
END
Go

-- Update WorkTask
CREATE PROCEDURE UpdateWorkTask
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
    UPDATE WorkTasks
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
CREATE PROCEDURE DeleteWorkTask
    @Id INT
AS
BEGIN
    DELETE FROM WorkTasks WHERE Id = @Id
END
Go

-- Get all Employees
CREATE PROCEDURE GetAllEmployees
AS
BEGIN
    SELECT * FROM Employees
END
Go

-- Get Employee by Id
CREATE PROCEDURE GetEmployeeById
    @Id INT
AS
BEGIN
    SELECT * FROM Employees WHERE Id = @Id
END
Go

-- Insert Employee
CREATE PROCEDURE InsertEmployee
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(255),
    @Department NVARCHAR(100),
    @Position NVARCHAR(100),
    @HireDate DATE
AS
BEGIN
    INSERT INTO Employees (FirstName, LastName, Email, Department, Position, HireDate)
    VALUES (@FirstName, @LastName, @Email, @Department, @Position, @HireDate)
END
Go

-- Update Employee
CREATE PROCEDURE UpdateEmployee
    @Id INT,
    @FirstName NVARCHAR(50),
    @LastName NVARCHAR(50),
    @Email NVARCHAR(255),
    @Department NVARCHAR(100),
    @Position NVARCHAR(100),
    @HireDate DATE
AS
BEGIN
    UPDATE Employees
    SET FirstName = @FirstName,
        LastName = @LastName,
        Email = @Email,
        Department = @Department,
        Position = @Position,
        HireDate = @HireDate
    WHERE Id = @Id
END
Go

-- Delete Employee
CREATE PROCEDURE DeleteEmployee
    @Id INT
AS
BEGIN
    DELETE FROM Employees WHERE Id = @Id
END
Go

-- Get all Projects
CREATE PROCEDURE GetAllProjects
AS
BEGIN
    SELECT * FROM Projects
END
Go

-- Get Project by Id
CREATE PROCEDURE GetProjectById
    @Id INT
AS
BEGIN
    SELECT * FROM Projects WHERE Id = @Id
END
Go

-- Insert Project
CREATE PROCEDURE InsertProject
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    INSERT INTO Projects (Name, Description, StartDate, EndDate)
    VALUES (@Name, @Description, @StartDate, @EndDate)
END
Go

-- Update Project
CREATE PROCEDURE UpdateProject
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    UPDATE Projects
    SET Name = @Name,
        Description = @Description,
        StartDate = @StartDate,
        EndDate = @EndDate
    WHERE Id = @Id
END
Go

-- Delete Project
CREATE PROCEDURE DeleteProject
    @Id INT
AS
BEGIN
    DELETE FROM Projects WHERE Id = @Id
END
