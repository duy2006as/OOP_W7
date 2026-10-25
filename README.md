# DỰ ÁN QUẢN LÝ THIẾT BỊ GIẢNG DẠY (OOP - TUẦN 7)
- **Họ và tên**: Hoàng Đình Duy
- **Mã sinh viên**: 202418887

## 1. Cấu trúc thư mục mã nguồn
```
D:\OOP\W7\
├── Enums/
│   └── DeviceStatus.cs           # Trạng thái thiết bị (Available, Borrowed, UnderMaintenance)
├── Interfaces/
│   ├── IIdentifiable.cs          # Interface định danh (Id)
│   ├── IRepository.cs            # Interface generic cho kho lưu trữ
│   └── ISelectionStrategy.cs     # Interface generic cho chiến lược lựa chọn
├── Models/
│   ├── TeachingDevice.cs         # Lớp cơ sở trừu tượng (kế thừa IIdentifiable)
│   ├── ProjectorRemote.cs        # Điều khiển máy chiếu
│   ├── AirConditionerRemote.cs   # Điều khiển điều hòa
│   └── WirelessMicrophone.cs     # Micro không dây
├── Repositories/
│   └── InMemoryRepository.cs     # Cài đặt kho lưu trữ trong bộ nhớ
├── Strategies/
│   ├── HighestPriorityStrategy.cs # Chiến lược chọn theo điểm ưu tiên cao nhất
│   └── FirstAvailableStrategy.cs  # Chiến lược chọn phần tử đầu tiên sẵn sàng
├── Services/
│   └── AllocationService.cs      # Dịch vụ cấp phát tổng quát (Generic)
├── Exceptions/
│   ├── DuplicateIdException.cs   # Ngoại lệ trùng mã
│   ├── NoSuitableItemException.cs # Ngoại lệ không tìm thấy thiết bị phù hợp
│   └── DeviceNotReadyException.cs # Ngoại lệ thiết bị chưa sẵn sàng
├── Program.cs                    # Console chạy 11 ca kiểm thử tương tác
└── W7.csproj
```

---

## 2. Danh sách 3 lỗi cố ý cài sẵn để sửa và commit lên GitHub

Chương trình hiện tại biên dịch không lỗi (`0 Error, 0 Warning`) và chạy được, nhưng chứa 3 lỗi logic nghiệp vụ nặng để sinh viên phát hiện, sửa dần và commit lên Git:

### Lỗi 1: `ProjectorRemote.cs` - Quên kiểm tra trạng thái Available khi xét sẵn sàng
- **Vị trí**: File `Models/ProjectorRemote.cs`, hàm `IsReadyForUse()`.
- **Mã hiện tại**:
  ```csharp
  public override bool IsReadyForUse()
  {
      return BatteryLevel >= 20;
  }
  ```
- **Hậu quả**: Khi thiết bị đang ở trạng thái `Borrowed` hoặc `UnderMaintenance`, chỉ cần pin >= 20% thì hàm vẫn trả về `true` (sai quy tắc: chỉ sẵn sàng khi `Status == Available` và `BatteryLevel >= 20`).
- **Cách sửa**:
  ```csharp
  public override bool IsReadyForUse()
  {
      return Status == DeviceStatus.Available && BatteryLevel >= 20;
  }
  ```
- **Gợi ý commit Git**: `fix(model): check Available status in ProjectorRemote.IsReadyForUse`

---

### Lỗi 2: `HighestPriorityStrategy.cs` - Ngược dấu so sánh điểm ưu tiên
- **Vị trí**: File `Strategies/HighestPriorityStrategy.cs`, hàm `Select()`.
- **Mã hiện tại**:
  ```csharp
  if (currentScore < bestScore) // Lỗi: chọn phần tử có điểm nhỏ nhất
  {
      bestScore = currentScore;
      bestItem = candidates[i];
  }
  ```
- **Hậu quả**: Khi chạy Kiểm thử 5 (Chọn điều khiển máy chiếu có điểm cao nhất), hệ thống lại chọn ra thiết bị có điểm/mức pin **thấp nhất**.
- **Cách sửa**: Sửa `<` thành `>`:
  ```csharp
  if (currentScore > bestScore)
  {
      bestScore = currentScore;
      bestItem = candidates[i];
  }
  ```
- **Gợi ý commit Git**: `fix(strategy): invert comparison to pick highest priority instead of lowest`

---

### Lỗi 3: `AllocationService.cs` - Quên gọi `Borrow()` khi cấp phát
- **Vị trí**: File `Services/AllocationService.cs`, hàm `Allocate()`.
- **Mã hiện tại**:
  ```csharp
  T selected = strategy.Select(candidates.AsReadOnly());
  return selected;
  ```
- **Hậu quả**: Khi kiểm thử mục 10 (Chứng minh thiết bị chuyển sang Borrowed), thiết bị sau khi cấp phát vẫn ở trạng thái `Available` vì chưa được gọi `selected.Borrow()`.
- **Cách sửa**: Bổ sung `selected.Borrow();` trước khi trả về:
  ```csharp
  T selected = strategy.Select(candidates.AsReadOnly());
  selected.Borrow();
  return selected;
  ```
- **Gợi ý commit Git**: `fix(service): mark selected device as borrowed upon allocation`

---

## 3. Hướng dẫn chạy và thực hiện 11 mục kiểm thử
Chạy lệnh sau tại thư mục `D:\OOP\W7`:
```powershell
dotnet run
```
Menu tương tác từ 1 đến 11 sẽ xuất hiện, cho phép nhập liệu bằng tay và quan sát kết quả kiểm thử.
