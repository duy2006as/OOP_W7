/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

namespace DeviceManagement.Exceptions;

// Ngoại lệ khi không tìm thấy thiết bị phù hợp
public class NoSuitableItemException : Exception
{
    public NoSuitableItemException(string message = "Không tìm thấy thiết bị phù hợp với yêu cầu.")
        : base(message)
    {
    }
}
