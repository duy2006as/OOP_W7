/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Enums;
using DeviceManagement.Exceptions;
using DeviceManagement.Interfaces;

namespace DeviceManagement.Models;

// Lớp cơ sở trừu tượng cho các thiết bị giảng dạy
public abstract class TeachingDevice : IIdentifiable
{
    public string Id { get; }
    public string Name { get; }
    public string StorageLocation { get; }
    public DeviceStatus Status { get; protected set; }

    protected TeachingDevice(string id, string name, string storageLocation)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Mã thiết bị không được để trống.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên thiết bị không được để trống.", nameof(name));
        if (string.IsNullOrWhiteSpace(storageLocation))
            throw new ArgumentException("Vị trí lưu trữ không được để trống.", nameof(storageLocation));

        Id = id;
        Name = name;
        StorageLocation = storageLocation;
        Status = DeviceStatus.Available;
    }

    // Kiểm tra khả năng sẵn sàng sử dụng
    public abstract bool IsReadyForUse();

    // Điểm ưu tiên mặc định
    public abstract double GetDefaultPriorityScore();

    // Mượn thiết bị
    public void Borrow()
    {
        if (!IsReadyForUse())
            throw new DeviceNotReadyException(Id, Status);

        Status = DeviceStatus.Borrowed;
    }

    // Trả thiết bị về kho
    public void Return()
    {
        if (Status != DeviceStatus.Borrowed)
            throw new InvalidOperationException($"Thiết bị {Id} đang ở trạng thái {Status}, không thể trả.");

        Status = DeviceStatus.Available;
    }

    public override string ToString()
    {
        return $"{GetType().Name} | ID: {Id} | Tên: {Name} | Vị trí: {StorageLocation} | Trạng thái: {Status}";
    }
}
