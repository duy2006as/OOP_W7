/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Interfaces;

namespace DeviceManagement.Interfaces;

// Giao diện kho lưu trữ tổng quát
public interface IRepository<T> where T : IIdentifiable
{
    void Add(T item);
    bool Remove(string id);
    T? FindById(string id);
    IReadOnlyList<T> GetAll();
    IReadOnlyList<T> FindAll(Predicate<T> condition);
}
