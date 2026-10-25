/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Enums;

namespace DeviceManagement.Models;

// Thiết bị micro không dây
public class WirelessMicrophone : TeachingDevice
{
    public int BatteryLevel { get; }
    public int SignalQuality { get; }

    public WirelessMicrophone(string id, string name, string storageLocation, int batteryLevel, int signalQuality)
        : base(id, name, storageLocation)
    {
        if (batteryLevel < 0 || batteryLevel > 100)
            throw new ArgumentOutOfRangeException(nameof(batteryLevel), "Mức pin phải từ 0 đến 100.");
        if (signalQuality < 0 || signalQuality > 100)
            throw new ArgumentOutOfRangeException(nameof(signalQuality), "Chất lượng tín hiệu phải từ 0 đến 100.");

        BatteryLevel = batteryLevel;
        SignalQuality = signalQuality;
    }

    // Sẵn sàng khi Available, pin >= 30% và tín hiệu >= 40%
    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available && BatteryLevel >= 30 && SignalQuality >= 40;
    }

    // Điểm ưu tiên bằng trung bình cộng mức pin và chất lượng tín hiệu
    public override double GetDefaultPriorityScore()
    {
        return (BatteryLevel + SignalQuality) / 2.0;
    }

    public override string ToString()
    {
        return base.ToString() + $" | Pin: {BatteryLevel}% | Tín hiệu: {SignalQuality}% | Điểm ưu tiên: {GetDefaultPriorityScore():F1}";
    }
}
