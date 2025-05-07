CREATE OR ALTER PROCEDURE GetAllDevices
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.Id,
        d.Name,
        d.IsEnabled,
        CASE
            WHEN s.DeviceId IS NOT NULL THEN 'SW'
            WHEN pc.DeviceId IS NOT NULL THEN 'P'
            WHEN e.DeviceId IS NOT NULL THEN 'ED'
            END AS DeviceType,
        s.BatteryPercentage,
        e.IpAddress,
        e.NetworkName,
        pc.OperationSystem,
        d.RowVersion
    FROM Device d
             LEFT JOIN Smartwatch s ON d.Id = s.DeviceId
             LEFT JOIN PersonalComputer pc ON d.Id = pc.DeviceId
             LEFT JOIN Embedded e ON d.Id = e.DeviceId;
END
