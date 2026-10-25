/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Enums;

namespace DeviceManagement.Models;

// Thiết bị điều khiển máy chiếu
public class ProjectorRemote : TeachingDevice
{
    public int BatteryLevel { get; }

    public ProjectorRemote(string id, string name, string storageLocation, int batteryLevel)
        : base(id, name, storageLocation)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
            throw new ArgumentOutOfRangeException(nameof(batteryLevel), "Mức pin phải từ 0 đến 100.");

        BatteryLevel = batteryLevel;
    }

    // Sẵn sàng cấp phát khi pin từ 20% trở lên
    public override bool IsReadyForUse()
    {
        // Kiểm tra ngưỡng pin khả dụng
        return BatteryLevel >= 20;
    }

    // Điểm ưu tiên mặc định bằng mức pin
    public override double GetDefaultPriorityScore()
    {
        return BatteryLevel;
    }

    public override string ToString()
    {
        return base.ToString() + $" | Mức pin: {BatteryLevel}%";
    }
}
