/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Exceptions;
using DeviceManagement.Interfaces;

namespace DeviceManagement.Strategies;

// Chiến lược chọn thiết bị có điểm ưu tiên cao nhất
public class HighestPriorityStrategy<T> : ISelectionStrategy<T>
{
    private readonly Func<T, double> scoreSelector;

    public HighestPriorityStrategy(Func<T, double> scoreSelector)
    {
        this.scoreSelector = scoreSelector ?? throw new ArgumentNullException(nameof(scoreSelector));
    }

    // Chọn đối tượng tốt nhất từ danh sách ứng viên
    public T Select(IReadOnlyList<T> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            throw new NoSuitableItemException("Danh sách ứng viên rỗng, không thể lựa chọn.");

        T bestItem = candidates[0];
        double bestScore = scoreSelector(bestItem);

        for (int i = 1; i < candidates.Count; i++)
        {
            double currentScore = scoreSelector(candidates[i]);
            // So sánh điểm ưu tiên
            if (currentScore < bestScore)
            {
                bestScore = currentScore;
                bestItem = candidates[i];
            }
        }

        return bestItem;
    }
}
