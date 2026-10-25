/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Exceptions;
using DeviceManagement.Interfaces;

namespace DeviceManagement.Repositories;

// Cài đặt kho lưu trữ trong bộ nhớ dùng Dictionary
public class InMemoryRepository<T> : IRepository<T> where T : IIdentifiable
{
    private readonly Dictionary<string, T> _items = new(StringComparer.OrdinalIgnoreCase);

    // Thêm phần tử vào kho
    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Phần tử thêm vào không được null.");

        if (_items.ContainsKey(item.Id))
            throw new DuplicateIdException(item.Id);

        _items.Add(item.Id, item);
    }

    // Xóa phần tử theo mã
    public bool Remove(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return _items.Remove(id);
    }

    // Tìm kiếm phần tử theo mã
    public T? FindById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return default;

        return _items.TryGetValue(id, out var item) ? item : default;
    }

    // Lấy toàn bộ danh sách, đóng gói dưới dạng ReadOnlyList
    public IReadOnlyList<T> GetAll()
    {
        return _items.Values.ToList().AsReadOnly();
    }

    // Lọc danh sách theo điều kiện
    public IReadOnlyList<T> FindAll(Predicate<T> condition)
    {
        if (condition == null)
            return GetAll();

        return _items.Values.Where(item => condition(item)).ToList().AsReadOnly();
    }
}
