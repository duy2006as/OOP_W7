# DỰ ÁN QUẢN LÝ THIẾT BỊ GIẢNG DẠY (OOP - TUẦN 7)

- **Họ và tên**: Hoàng Đình Duy
- **Mã sinh viên**: 202418887

---

## 1. Giới thiệu dự án
Hệ thống quản lý các thiết bị dùng trong phòng học giảng dạy tại trường đại học bao gồm:
- Điều khiển máy chiếu (`ProjectorRemote`)
- Điều khiển điều hòa (`AirConditionerRemote`)
- Micro không dây (`WirelessMicrophone`)

Dự án được xây dựng dựa trên hai trọng tâm thiết kế hướng đối tượng:
1. **Đa hình (Polymorphism)**: Thay đổi linh hoạt hành vi kiểm tra khả năng sẵn sàng (`IsReadyForUse`), tính điểm ưu tiên (`GetDefaultPriorityScore`) và thuật toán lựa chọn thiết bị (`ISelectionStrategy<T>`).
2. **Tổng quát hóa (Generics)**: Xây dựng kho lưu trữ dữ liệu (`IRepository<T>`) và dịch vụ cấp phát (`AllocationService<T>`) có thể tái sử dụng cho nhiều loại thiết bị mà không cần sao chép mã.

---

## 2. Cấu trúc thư mục mã nguồn
```
D:\OOP\W7\
├── Enums/
│   └── DeviceStatus.cs           # Định nghĩa các trạng thái: Available, Borrowed, UnderMaintenance
├── Interfaces/
│   ├── IIdentifiable.cs          # Interface định danh duy nhất (Id)
│   ├── IRepository.cs            # Interface generic cho kho lưu trữ
│   └── ISelectionStrategy.cs     # Interface generic cho chiến lược lựa chọn
├── Models/
│   ├── TeachingDevice.cs         # Lớp cơ sở trừu tượng cho thiết bị giảng dạy
│   ├── ProjectorRemote.cs        # Điều khiển máy chiếu
│   ├── AirConditionerRemote.cs   # Điều khiển điều hòa
│   └── WirelessMicrophone.cs     # Micro không dây
├── Repositories/
│   └── InMemoryRepository.cs     # Cài đặt kho lưu trữ bộ nhớ dùng Dictionary<string, T>
├── Strategies/
│   ├── HighestPriorityStrategy.cs # Chiến lược chọn theo điểm ưu tiên cao nhất
│   └── FirstAvailableStrategy.cs  # Chiến lược chọn phần tử đầu tiên sẵn sàng
├── Services/
│   └── AllocationService.cs      # Dịch vụ cấp phát thiết bị tổng quát
├── Exceptions/
│   ├── DuplicateIdException.cs   # Ngoại lệ khi trùng mã định danh
│   ├── NoSuitableItemException.cs # Ngoại lệ khi không tìm thấy thiết bị phù hợp
│   └── DeviceNotReadyException.cs # Ngoại lệ khi thiết bị chưa sẵn sàng cấp phát
├── Program.cs                    # Chương trình console thực hiện các ca kiểm thử
├── .gitignore                    # Loại trừ thư mục bin/, obj/ khi đẩy lên Git
└── W7.csproj
```

---

## 3. Kiến trúc thiết kế chính

### 3.1. Phân cấp thực thể thiết bị
- `TeachingDevice` là lớp cơ sở trừu tượng kế thừa `IIdentifiable`, triển khai các thuộc tính cơ bản (`Id`, `Name`, `StorageLocation`, `Status`) và các hành vi mượn/trả (`Borrow()`, `Return()`).
- Các lớp dẫn xuất (`ProjectorRemote`, `AirConditionerRemote`, `WirelessMicrophone`) ghi đè các phương thức trừu tượng:
  - `IsReadyForUse()`: Kiểm tra điều kiện pin, sóng và trạng thái tùy loại thiết bị.
  - `GetDefaultPriorityScore()`: Tính điểm ưu tiên cấp phát riêng biệt.

### 3.2. Kho lưu trữ tổng quát (Generic Repository Pattern)
- `IRepository<T> where T : IIdentifiable` cung cấp các phương thức `Add`, `Remove`, `FindById`, `GetAll`, `FindAll`.
- `InMemoryRepository<T>` sử dụng cấu trúc `Dictionary<string, T>` giúp tối ưu tra cứu với độ phức tạp $O(1)$ và bảo vệ tính đóng gói bằng cách trả về `IReadOnlyList<T>`.

### 3.3. Chiến lược lựa chọn (Strategy Pattern)
- `ISelectionStrategy<T>` định nghĩa giao diện thuật toán lựa chọn.
- `HighestPriorityStrategy<T>` và `FirstAvailableStrategy<T>` cho phép hoán đổi thuật toán lựa chọn thiết bị ngay trong thời gian chạy (`Runtime Strategy Switch`) thông qua `AllocationService<T>`.

---

## 4. Hướng dẫn biên dịch và chạy chương trình

### Yêu cầu môi trường:
- .NET SDK 8.0 trở lên (khuyên dùng .NET 10.0).

### Lệnh thực thi:
Tại thư mục `D:\OOP\W7`, mở cửa sổ lệnh (Terminal / PowerShell) và chạy:
```powershell
dotnet run
```

---

## 5. Danh mục các chức năng kiểm thử trong Console

Chương trình cung cấp giao diện menu tương tác hỗ trợ kiểm thử 11 yêu cầu kỹ thuật:

1. **Thêm thiết bị vào repository**: Nhập thủ công thông tin thiết bị (Mã, Tên, Vị trí, Mức pin, Phòng tương thích, Tín hiệu sóng).
2. **Thử thêm hai thiết bị trùng mã**: Kiểm chứng hệ thống nhận diện và xử lý ngoại lệ `DuplicateIdException`.
3. **Tìm thiết bị theo mã**: Nhập mã định danh để tra cứu thông tin thiết bị trên hệ thống kho.
4. **Lọc các thiết bị sẵn sàng**: Hiển thị danh sách các thiết bị thỏa mãn điều kiện `IsReadyForUse()`.
5. **Chọn điều khiển máy chiếu có điểm cao nhất**: Kiểm thử thuật toán `HighestPriorityStrategy`.
6. **Chọn điều khiển điều hòa tương thích phòng**: Cấp phát điều khiển điều hòa theo mã phòng học được nhập vào.
7. **Chọn micro đầu tiên sẵn sàng**: Kiểm thử thuật toán `FirstAvailableStrategy`.
8. **Đổi chiến lược trong thời gian chạy**: Thực nghiệm chuyển đổi linh hoạt giữa các chiến lược lựa chọn.
9. **Thử cấp phát khi không có ứng viên phù hợp**: Kiểm chứng hệ thống phát sinh và xử lý ngoại lệ `NoSuitableItemException`.
10. **Chứng minh thiết bị được chọn chuyển sang Borrowed**: Kiểm tra trạng thái thiết bị trước và sau khi được cấp phát và trả về kho.
11. **Chứng minh Repository tổng quát dùng cho nhiều loại thiết bị**: Minh chứng việc tái sử dụng lớp `InMemoryRepository<T>` cho nhiều loại đối tượng khác nhau mà không sao chép mã.
12. **Xem tất cả thiết bị hiện có trong các kho**: Hiển thị chi tiết toàn bộ thiết bị đang quản lý.
