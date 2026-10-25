/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Exceptions;
using DeviceManagement.Interfaces;

namespace DeviceManagement.Strategies;

// Chiến lược chọn thiết bị đầu tiên khả dụng
public class FirstAvailableStrategy<T> : ISelectionStrategy<T>
{
    // Chọn phần tử đầu tiên trong danh sách ứng viên
    public T Select(IReadOnlyList<T> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            throw new NoSuitableItemException("Danh sách ứng viên rỗng, không thể lựa chọn.");

        return candidates[0];
    }
}
