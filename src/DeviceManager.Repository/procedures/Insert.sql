CREATE OR ALTER PROCEDURE AddDevice
    @Id               NVARCHAR(50),
    @Name             NVARCHAR(100),
    @IsEnabled        BIT,
    @Type             NVARCHAR(50),
    @BatteryPercentage INT             = NULL,
    @OperationSystem  NVARCHAR(100)    = NULL,
    @IpAddress        NVARCHAR(50)     = NULL,
    @NetworkName      NVARCHAR(100)    = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Device (Id, Name, IsEnabled)
    VALUES (@Id, @Name, @IsEnabled);

    IF @Type = 'SW'
        BEGIN
            INSERT INTO Smartwatch (BatteryPercentage, DeviceId)
            VALUES (@BatteryPercentage, @Id);
        END
    ELSE IF @Type = 'ED'
        BEGIN
            INSERT INTO Embedded (IpAddress, NetworkName, DeviceId)
            VALUES (@IpAddress, @NetworkName, @Id);
        END
    ELSE IF @Type = 'PC'
        BEGIN
            INSERT INTO PersonalComputer (OperationSystem, DeviceId)
            VALUES (@OperationSystem, @Id);
        END
END

SELECT * FROM DEVICE