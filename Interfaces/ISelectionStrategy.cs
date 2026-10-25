/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

namespace DeviceManagement.Interfaces;

// Giao diện chiến lược lựa chọn thiết bị
public interface ISelectionStrategy<T>
{
    T Select(IReadOnlyList<T> candidates);
}
