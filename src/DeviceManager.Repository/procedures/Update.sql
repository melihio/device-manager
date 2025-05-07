CREATE OR ALTER PROCEDURE UpdateDevice
    @Id NVARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @Type NVARCHAR(50),
    @BatteryPercentage INT = NULL,
    @OperationSystem NVARCHAR(100) = NULL,
    @IpAddress NVARCHAR(50) = NULL,
    @NetworkName NVARCHAR(100) = NULL,
    @OriginalRowVersion ROWVERSION
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Device
    SET Name = @Name,
        IsEnabled = @IsEnabled
    WHERE Id = @Id AND RowVersion = @OriginalRowVersion;

    IF @@ROWCOUNT = 0
        THROW 50001, 'Device was modified by another process.', 1;

    IF @Type = 'SW'
        BEGIN
            UPDATE Smartwatch
            SET BatteryPercentage = @BatteryPercentage
            WHERE DeviceId = @Id;
        END
    ELSE IF @Type = 'P'
        BEGIN
            UPDATE PersonalComputer
            SET OperationSystem = @OperationSystem
            WHERE DeviceId = @Id;
        END
    ELSE IF @Type = 'ED'
        BEGIN
            UPDATE Embedded
            SET IpAddress = @IpAddress,
                NetworkName = @NetworkName
            WHERE DeviceId = @Id;
        END
END
