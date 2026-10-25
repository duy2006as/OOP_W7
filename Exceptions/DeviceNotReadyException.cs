/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Enums;

namespace DeviceManagement.Exceptions;

// Ngoại lệ khi thiết bị chưa sẵn sàng cấp phát
public class DeviceNotReadyException : Exception
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceNotReadyException(string deviceId, DeviceStatus currentStatus, string message = "Thiết bị không ở trạng thái sẵn sàng để cấp phát.")
        : base(message)
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}
