/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

namespace DeviceManagement.Exceptions;

// Ngoại lệ khi trùng lặp mã định danh
public class DuplicateIdException : Exception
{
    public string DuplicateId { get; }

    public DuplicateIdException(string duplicateId)
        : base($"Phần tử có mã '{duplicateId}' đã tồn tại trong hệ thống.")
    {
        DuplicateId = duplicateId;
    }
}
