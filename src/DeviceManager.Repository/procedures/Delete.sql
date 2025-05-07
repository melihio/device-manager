CREATE OR ALTER PROCEDURE DeleteDevice
    @Id NVARCHAR(50),
    @Type NVARCHAR(50)
    AS
BEGIN
    SET NOCOUNT ON;

    IF @Type = 'SW'
        BEGIN
            DELETE FROM Smartwatch WHERE DeviceId = @Id;
    END
    ELSE IF @Type = 'ED'
        BEGIN
            DELETE FROM Embedded WHERE DeviceId = @Id;
    END
    ELSE IF @Type = 'P'
        BEGIN
            DELETE FROM PersonalComputer WHERE DeviceId = @Id;
    END
    DELETE FROM Device WHERE Id = @Id;
END