/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Enums;

namespace DeviceManagement.Models;

// Thiết bị điều khiển điều hòa
public class AirConditionerRemote : TeachingDevice
{
    public IReadOnlySet<string> CompatibleRooms { get; }

    public AirConditionerRemote(string id, string name, string storageLocation, IEnumerable<string> compatibleRooms)
        : base(id, name, storageLocation)
    {
        var rooms = new HashSet<string>(compatibleRooms ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase);
        CompatibleRooms = rooms;
    }

    // Kiểm tra tính tương thích với phòng
    public bool IsCompatibleWith(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            return false;
        return CompatibleRooms.Contains(roomId.Trim());
    }

    // Sẵn sàng khi đang ở trạng thái Available
    public override bool IsReadyForUse()
    {
        return Status == DeviceStatus.Available;
    }

    // Điểm ưu tiên mặc định
    public override double GetDefaultPriorityScore()
    {
        return 100.0;
    }

    public override string ToString()
    {
        var roomsStr = string.Join(", ", CompatibleRooms);
        return base.ToString() + $" | Phòng tương thích: {roomsStr}";
    }
}
