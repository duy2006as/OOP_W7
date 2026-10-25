/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Interfaces;
using DeviceManagement.Models;

namespace DeviceManagement.Services;

// Dịch vụ cấp phát thiết bị tổng quát
public class AllocationService<T> where T : TeachingDevice
{
    private readonly IRepository<T> repository;
    private ISelectionStrategy<T> strategy;

    public AllocationService(IRepository<T> repository, ISelectionStrategy<T> strategy)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    // Thay đổi chiến lược lựa chọn tại thời gian chạy
    public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
    {
        strategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
    }

    // Cấp phát thiết bị thỏa mãn điều kiện
    public T Allocate(Predicate<T>? additionalCondition)
    {
        // 1. Lấy danh sách từ repository
        var items = repository.GetAll();

        // 2 & 3. Lọc thiết bị sẵn sàng và thỏa mãn điều kiện bổ sung
        var candidates = items
            .Where(device => device.IsReadyForUse())
            .Where(device => additionalCondition == null || additionalCondition(device))
            .ToList();

        // 4. Dùng chiến lược hiện tại để chọn thiết bị
       T selected = strategy.Select(candidates.AsReadOnly());
         selected.Borrow();
         return selected;
    }
}
