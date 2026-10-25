/********************
Mã sinh viên: 202418887
Họ tên: Hoàng Đình Duy
********************/

using DeviceManagement.Enums;
using DeviceManagement.Exceptions;
using DeviceManagement.Interfaces;
using DeviceManagement.Models;
using DeviceManagement.Repositories;
using DeviceManagement.Services;
using DeviceManagement.Strategies;

namespace DeviceManagement;

public class Program
{
    // Các kho lưu trữ độc lập minh chứng tính generic
    private static readonly IRepository<ProjectorRemote> projectorRepo = new InMemoryRepository<ProjectorRemote>();
    private static readonly IRepository<AirConditionerRemote> acRepo = new InMemoryRepository<AirConditionerRemote>();
    private static readonly IRepository<WirelessMicrophone> micRepo = new InMemoryRepository<WirelessMicrophone>();

    // Các dịch vụ cấp phát
    private static readonly AllocationService<ProjectorRemote> projectorService =
        new(projectorRepo, new HighestPriorityStrategy<ProjectorRemote>(d => d.GetDefaultPriorityScore()));

    private static readonly AllocationService<AirConditionerRemote> acService =
        new(acRepo, new HighestPriorityStrategy<AirConditionerRemote>(d => d.GetDefaultPriorityScore()));

    private static readonly AllocationService<WirelessMicrophone> micService =
        new(micRepo, new FirstAvailableStrategy<WirelessMicrophone>());

    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        try { Console.Clear(); } catch { }

        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("====================================================================");
            Console.WriteLine("        HỆ THỐNG QUẢN LÝ THIẾT BỊ GIẢNG DẠY - TUẦN 7 OOP            ");
            Console.WriteLine("====================================================================");
            Console.WriteLine(" 1. Thêm thiết bị vào repository");
            Console.WriteLine(" 2. Thử thêm hai thiết bị trùng mã");
            Console.WriteLine(" 3. Tìm thiết bị theo mã");
            Console.WriteLine(" 4. Lọc các thiết bị sẵn sàng");
            Console.WriteLine(" 5. Chọn điều khiển máy chiếu có điểm cao nhất");
            Console.WriteLine(" 6. Chọn điều khiển điều hòa tương thích phòng");
            Console.WriteLine(" 7. Chọn micro đầu tiên sẵn sàng");
            Console.WriteLine(" 8. Đổi chiến lược trong thời gian chạy và quan sát kết quả");
            Console.WriteLine(" 9. Thử cấp phát khi không có ứng viên phù hợp");
            Console.WriteLine(" 10. Chứng minh thiết bị được chọn chuyển sang Borrowed");
            Console.WriteLine(" 11. Chứng minh Repository tổng quát dùng cho nhiều loại thiết bị");
            Console.WriteLine(" 12. Xem tất cả thiết bị hiện có trong các kho");
            Console.WriteLine(" 0. Thoát chương trình");
            Console.WriteLine("====================================================================");
            Console.Write("Vui lòng chọn chức năng: ");

            string? choice = Console.ReadLine();
            if (choice == null)
            {
                running = false;
                break;
            }

            string cleanChoice = choice.Trim('\uFEFF', ' ', '\t', '\r', '\n');
            Console.WriteLine();

            try
            {
                switch (cleanChoice)
                {
                    case "1":
                        TestCase1_AddDevice();
                        break;
                    case "2":
                        TestCase2_DuplicateId();
                        break;
                    case "3":
                        TestCase3_FindById();
                        break;
                    case "4":
                        TestCase4_FilterReadyDevices();
                        break;
                    case "5":
                        TestCase5_AllocateProjectorRemote();
                        break;
                    case "6":
                        TestCase6_AllocateAcRemote();
                        break;
                    case "7":
                        TestCase7_AllocateMicrophone();
                        break;
                    case "8":
                        TestCase8_ChangeStrategyRuntime();
                        break;
                    case "9":
                        TestCase9_NoSuitableCandidate();
                        break;
                    case "10":
                        TestCase10_VerifyStatusBorrowed();
                        break;
                    case "11":
                        TestCase11_DemonstrateGenericRepository();
                        break;
                    case "12":
                        ShowAllDevices();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Đã thoát chương trình. Tạm biệt!");
                        continue;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI]: {ex.Message}");
            }
        }
    }

    // 1. Thêm thiết bị vào repository
    private static void TestCase1_AddDevice()
    {
        Console.WriteLine("--- KIỂM THỬ 1: THÊM THIẾT BỊ VÀO REPOSITORY ---");
        Console.WriteLine("1. Nhập thủ công một thiết bị");
        Console.WriteLine("2. Nhập nhanh bộ 8 thiết bị tối thiểu theo đề bài");
        Console.Write("Chọn phương thức: ");
        string? subChoice = Console.ReadLine();

        if (subChoice?.Trim() == "2")
        {
            AddMinimumPresetManually();
            return;
        }

        Console.WriteLine("\n[NHẬP THÔNG TIN THIẾT BỊ MỚI]");
        var dev = InputDevice();
        try
        {
            AddDevice(dev);
            Console.WriteLine($"-> Đã thêm thành công: {dev}");
        }
        catch (DuplicateIdException ex)
        {
            Console.WriteLine($"[LỖI TRÙNG MÃ]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI]: {ex.Message}");
        }
    }

    // 2. Thử thêm hai thiết bị trùng mã
    private static void TestCase2_DuplicateId()
    {
        Console.WriteLine("--- KIỂM THỬ 2: THỬ THÊM THIẾT BỊ TRÙNG MÃ ---");

        Console.WriteLine("\n[BƯỚC 1]: Nhập thông tin thiết bị thứ nhất:");
        var devFirst = InputDevice();
        try
        {
            AddDevice(devFirst);
            Console.WriteLine($"-> Đã lưu thiết bị thứ nhất: {devFirst.Id} - {devFirst.Name}");
        }
        catch (DuplicateIdException ex)
        {
            Console.WriteLine($"[BÁO LỖI TRÙNG MÃ]: Thiết bị thứ nhất đã bị trùng mã trong hệ thống: {ex.DuplicateId}");
            Console.WriteLine("Vui lòng thử lại.");
            return;
        }

        Console.WriteLine("\n[BƯỚC 2]: Nhập thông tin thiết bị thứ hai:");
        var devSecond = InputDevice();
        try
        {
            AddDevice(devSecond);
            Console.WriteLine($"-> Thêm thiết bị thứ hai thành công: {devSecond.Id} - {devSecond.Name}. Mã không bị trùng.");
        }
        catch (DuplicateIdException ex)
        {
            Console.WriteLine("[BÁO LỖI TRÙNG MÃ]: Phát hiện trùng mã thiết bị!");
            Console.WriteLine($"  - Chi tiết ngoại lệ: {ex.Message}");
            Console.WriteLine($"  - Mã trùng lặp: {ex.DuplicateId}");
        }
    }

    // 3. Tìm thiết bị theo mã
    private static void TestCase3_FindById()
    {
        Console.WriteLine("--- KIỂM THỬ 3: TÌM THIẾT BỊ THEO MÃ ---");
        string id = PromptNonEmpty("Nhập mã thiết bị cần tìm: ");

        TeachingDevice? found = (TeachingDevice?)projectorRepo.FindById(id)
            ?? (TeachingDevice?)acRepo.FindById(id)
            ?? micRepo.FindById(id);

        if (found != null)
        {
            Console.WriteLine("-> Tìm thấy thiết bị trong hệ thống:");
            Console.WriteLine($"   {found}");
            Console.WriteLine($"   Sẵn sàng sử dụng: {found.IsReadyForUse()}");
            Console.WriteLine($"   Điểm ưu tiên mặc định: {found.GetDefaultPriorityScore()}");
        }
        else
        {
            Console.WriteLine($"-> Không tìm thấy thiết bị nào có mã '{id}'.");
        }
    }

    // 4. Lọc các thiết bị sẵn sàng
    private static void TestCase4_FilterReadyDevices()
    {
        Console.WriteLine("--- KIỂM THỬ 4: LỌC CÁC THIẾT BỊ SẴN SÀNG ---");

        var readyProjectors = projectorRepo.FindAll(d => d.IsReadyForUse());
        Console.WriteLine($"\n1. Máy chiếu sẵn sàng: {readyProjectors.Count} thiết bị");
        foreach (var p in readyProjectors)
            Console.WriteLine($"   - {p} | Sẵn sàng: {p.IsReadyForUse()}");

        var readyAcs = acRepo.FindAll(d => d.IsReadyForUse());
        Console.WriteLine($"\n2. Điều khiển điều hòa sẵn sàng: {readyAcs.Count} thiết bị");
        foreach (var ac in readyAcs)
            Console.WriteLine($"   - {ac} | Sẵn sàng: {ac.IsReadyForUse()}");

        var readyMics = micRepo.FindAll(d => d.IsReadyForUse());
        Console.WriteLine($"\n3. Micro không dây sẵn sàng: {readyMics.Count} thiết bị");
        foreach (var m in readyMics)
            Console.WriteLine($"   - {m} | Sẵn sàng: {m.IsReadyForUse()}");
    }

    // 5. Chọn điều khiển máy chiếu có điểm cao nhất
    private static void TestCase5_AllocateProjectorRemote()
    {
        Console.WriteLine("--- KIỂM THỬ 5: CHỌN ĐIỀU KHIỂN MÁY CHIẾU CÓ ĐIỂM CAO NHẤT ---");
        var all = projectorRepo.GetAll();
        if (all.Count == 0)
        {
            Console.WriteLine("Kho máy chiếu hiện chưa có thiết bị. Vui lòng thêm thiết bị ở mục 1 trước.");
            return;
        }

        Console.WriteLine("Danh sách máy chiếu hiện có trong kho:");
        foreach (var p in all)
            Console.WriteLine($"   - {p} | Sẵn sàng: {p.IsReadyForUse()} | Điểm ưu tiên: {p.GetDefaultPriorityScore()}");

        Console.Write("Nhập mức pin tối thiểu yêu cầu nếu muốn, hoặc nhấn Enter để bỏ qua: ");
        string? inputPin = Console.ReadLine();
        Predicate<ProjectorRemote>? condition = null;
        if (int.TryParse(inputPin, out int minPin))
        {
            condition = p => p.BatteryLevel >= minPin;
            Console.WriteLine($"-> Đang áp dụng điều kiện lọc: Mức pin >= {minPin}");
        }

        try
        {
            var selected = projectorService.Allocate(condition);
            Console.WriteLine("\n-> Kết quả cấp phát điều khiển máy chiếu:");
            Console.WriteLine($"   {selected}");
            Console.WriteLine($"   Điểm ưu tiên: {selected.GetDefaultPriorityScore()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI CẤP PHÁT]: {ex.Message}");
        }
    }

    // 6. Chọn điều khiển điều hòa tương thích với một phòng
    private static void TestCase6_AllocateAcRemote()
    {
        Console.WriteLine("--- KIỂM THỬ 6: CHỌN ĐIỀU KHIỂN ĐIỀU HÒA TƯƠNG THÍCH VỚI MỘT PHÒNG ---");
        var all = acRepo.GetAll();
        if (all.Count == 0)
        {
            Console.WriteLine("Kho điều hòa hiện chưa có thiết bị. Vui lòng thêm thiết bị ở mục 1 trước.");
            return;
        }

        Console.WriteLine("Danh sách điều khiển điều hòa trong kho:");
        foreach (var ac in all)
            Console.WriteLine($"   - {ac}");

        string room = PromptNonEmpty("\nNhập tên phòng cần mượn điều hòa ví dụ A203: ");

        try
        {
            var selected = acService.Allocate(device => device.IsCompatibleWith(room));
            Console.WriteLine($"\n-> Cấp phát thành công điều khiển điều hòa cho phòng {room}:");
            Console.WriteLine($"   {selected}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI CẤP PHÁT]: {ex.Message}");
        }
    }

    // 7. Chọn micro đầu tiên sẵn sàng
    private static void TestCase7_AllocateMicrophone()
    {
        Console.WriteLine("--- KIỂM THỬ 7: CHỌN MICRO ĐẦU TIÊN SẴN SÀNG ---");
        var all = micRepo.GetAll();
        if (all.Count == 0)
        {
            Console.WriteLine("Kho micro hiện chưa có thiết bị. Vui lòng thêm thiết bị ở mục 1 trước.");
            return;
        }

        Console.WriteLine("Danh sách micro hiện có trong kho:");
        foreach (var m in all)
            Console.WriteLine($"   - {m} | Sẵn sàng: {m.IsReadyForUse()}");

        Console.Write("Nhập chất lượng tín hiệu tối thiểu nếu muốn, hoặc nhấn Enter để bỏ qua: ");
        string? inputSig = Console.ReadLine();
        Predicate<WirelessMicrophone>? condition = null;
        if (int.TryParse(inputSig, out int minSig))
        {
            condition = m => m.SignalQuality >= minSig;
            Console.WriteLine($"-> Đang áp dụng điều kiện lọc: Tín hiệu >= {minSig}");
        }

        try
        {
            var selected = micService.Allocate(condition);
            Console.WriteLine("\n-> Cấp phát thành công micro đầu tiên sẵn sàng:");
            Console.WriteLine($"   {selected}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI CẤP PHÁT]: {ex.Message}");
        }
    }

    // 8. Đổi chiến lược trong thời gian chạy và quan sát kết quả
    private static void TestCase8_ChangeStrategyRuntime()
    {
        Console.WriteLine("--- KIỂM THỬ 8: ĐỔI CHIẾN LƯỢC TRONG THỜI GIAN CHẠY ---");
        Console.WriteLine("Chọn kho thiết bị để thử nghiệm:");
        Console.WriteLine("1. Kho Micro không dây");
        Console.WriteLine("2. Kho Máy chiếu");
        Console.Write("Lựa chọn: ");
        string? repoChoice = Console.ReadLine();

        if (repoChoice?.Trim() == "2")
        {
            TestChangeStrategyProjector();
        }
        else
        {
            TestChangeStrategyMicrophone();
        }
    }

    private static void TestChangeStrategyMicrophone()
    {
        Console.WriteLine("\nDanh sách Micro trong kho:");
        foreach (var m in micRepo.GetAll())
            Console.WriteLine($"   - {m} | Sẵn sàng: {m.IsReadyForUse()} | Điểm: {m.GetDefaultPriorityScore():F1}");

        Console.WriteLine("\nChọn chiến lược muốn áp dụng:");
        Console.WriteLine("1. FirstAvailableStrategy - Chọn phần tử đầu tiên sẵn sàng");
        Console.WriteLine("2. HighestPriorityStrategy - Chọn phần tử có điểm ưu tiên cao nhất");
        Console.Write("Lựa chọn: ");
        string? stChoice = Console.ReadLine();

        if (stChoice?.Trim() == "2")
        {
            micService.ChangeStrategy(new HighestPriorityStrategy<WirelessMicrophone>(m => m.GetDefaultPriorityScore()));
            Console.WriteLine("-> Đã đổi sang: HighestPriorityStrategy");
        }
        else
        {
            micService.ChangeStrategy(new FirstAvailableStrategy<WirelessMicrophone>());
            Console.WriteLine("-> Đã đổi sang: FirstAvailableStrategy");
        }

        try
        {
            var selected = micService.Allocate(null);
            Console.WriteLine($"-> Kết quả cấp phát: {selected.Id} - {selected.Name} | Điểm: {selected.GetDefaultPriorityScore():F1}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI]: {ex.Message}");
        }
    }

    private static void TestChangeStrategyProjector()
    {
        Console.WriteLine("\nDanh sách Máy chiếu trong kho:");
        foreach (var p in projectorRepo.GetAll())
            Console.WriteLine($"   - {p} | Sẵn sàng: {p.IsReadyForUse()} | Điểm: {p.GetDefaultPriorityScore():F1}");

        Console.WriteLine("\nChọn chiến lược muốn áp dụng:");
        Console.WriteLine("1. FirstAvailableStrategy - Chọn phần tử đầu tiên sẵn sàng");
        Console.WriteLine("2. HighestPriorityStrategy - Chọn phần tử có điểm ưu tiên cao nhất");
        Console.Write("Lựa chọn: ");
        string? stChoice = Console.ReadLine();

        if (stChoice?.Trim() == "1")
        {
            projectorService.ChangeStrategy(new FirstAvailableStrategy<ProjectorRemote>());
            Console.WriteLine("-> Đã đổi sang: FirstAvailableStrategy");
        }
        else
        {
            projectorService.ChangeStrategy(new HighestPriorityStrategy<ProjectorRemote>(p => p.GetDefaultPriorityScore()));
            Console.WriteLine("-> Đã đổi sang: HighestPriorityStrategy");
        }

        try
        {
            var selected = projectorService.Allocate(null);
            Console.WriteLine($"-> Kết quả cấp phát: {selected.Id} - {selected.Name} | Điểm: {selected.GetDefaultPriorityScore():F1}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI]: {ex.Message}");
        }
    }

    // 9. Thử cấp phát khi không có ứng viên phù hợp
    private static void TestCase9_NoSuitableCandidate()
    {
        Console.WriteLine("--- KIỂM THỬ 9: THỬ CẤP PHÁT KHI KHÔNG CÓ ỨNG VIÊN PHÙ HỢP ---");
        Console.Write("Nhập mã phòng chắc chắn không tồn tại ví dụ Z999: ");
        string room = Console.ReadLine() ?? "Z999";
        if (string.IsNullOrWhiteSpace(room)) room = "Z999";

        Console.WriteLine($"Đang yêu cầu cấp phát điều khiển điều hòa cho phòng '{room}'...");
        try
        {
            var selected = acService.Allocate(device => device.IsCompatibleWith(room));
            Console.WriteLine($"[BẤT THƯỜNG]: Cấp phát được thiết bị {selected.Id}");
        }
        catch (NoSuitableItemException ex)
        {
            Console.WriteLine("[THÀNH CÔNG]: Đã bắt được NoSuitableItemException như mong đợi!");
            Console.WriteLine($"  - Nội dung ngoại lệ: {ex.Message}");
        }
    }

    // 10. Chứng minh thiết bị được chọn chuyển sang Borrowed
    private static void TestCase10_VerifyStatusBorrowed()
    {
        Console.WriteLine("--- KIỂM THỬ 10: CHỨNG MINH THIẾT BỊ ĐƯỢC CHỌN CHUYỂN SANG BORROWED ---");
        var readyList = projectorRepo.FindAll(d => d.IsReadyForUse());
        if (readyList.Count == 0)
        {
            Console.WriteLine("Kho máy chiếu không có thiết bị sẵn sàng. Vui lòng thêm thiết bị trước ở mục 1.");
            return;
        }

        Console.WriteLine("Danh sách máy chiếu sẵn sàng trong kho:");
        foreach (var p in readyList)
            Console.WriteLine($"   - {p.Id}: {p.Name} | Trạng thái: {p.Status}");

        string chosenId = PromptNonEmpty("Nhập mã thiết bị muốn cấp phát mượn: ");
        var sample = readyList.FirstOrDefault(d => d.Id.Equals(chosenId, StringComparison.OrdinalIgnoreCase));
        if (sample == null)
        {
            Console.WriteLine($"Mã '{chosenId}' không có trong danh sách sẵn sàng. Chọn mặc định: {readyList[0].Id}");
            sample = readyList[0];
        }

        try
        {
            Console.WriteLine($"Trạng thái TRƯỚC KHI cấp phát của {sample.Id}: {sample.Status}");

            // Thực hiện cấp phát
            var allocated = projectorService.Allocate(d => d.Id == sample.Id);
            Console.WriteLine($"Trạng thái SAU KHI cấp phát của {allocated.Id}: {allocated.Status}");

            if (allocated.Status == DeviceStatus.Borrowed)
            {
                Console.WriteLine("-> XÁC NHẬN: Thiết bị đã chuyển sang trạng thái Borrowed thành công!");
                Console.Write("Bạn có muốn trả thiết bị về Available không? y/n: ");
                if (Console.ReadLine()?.Trim().ToLower() == "y")
                {
                    allocated.Return();
                    Console.WriteLine($"-> Đã trả thiết bị. Trạng thái hiện tại: {allocated.Status}");
                }
            }
            else
            {
                Console.WriteLine($"[CẢNH BÁO HOẶC LỖI]: Thiết bị vẫn đang ở trạng thái '{allocated.Status}' chưa chuyển sang Borrowed!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LỖI]: {ex.Message}");
        }
    }

    // 11. Chứng minh Repository tổng quát dùng cho nhiều loại thiết bị
    private static void TestCase11_DemonstrateGenericRepository()
    {
        Console.WriteLine("--- KIỂM THỬ 11: CHỨNG MINH REPOSITORY TỔNG QUÁT TÁI SỬ DỤNG MÃ ---");
        Console.WriteLine("Hệ thống đang sử dụng cùng một lớp InMemoryRepository cho 3 loại thiết bị:");
        Console.WriteLine($"1. Kho ProjectorRemote: Kiểu = {projectorRepo.GetType().FullName}");
        Console.WriteLine($"   - Số lượng thiết bị hiện tại: {projectorRepo.GetAll().Count}");
        Console.WriteLine($"2. Kho AirConditionerRemote: Kiểu = {acRepo.GetType().FullName}");
        Console.WriteLine($"   - Số lượng thiết bị hiện tại: {acRepo.GetAll().Count}");
        Console.WriteLine($"3. Kho WirelessMicrophone: Kiểu = {micRepo.GetType().FullName}");
        Console.WriteLine($"   - Số lượng thiết bị hiện tại: {micRepo.GetAll().Count}");
        Console.WriteLine("\n-> Kết luận: Chỉ cần viết 1 lớp InMemoryRepository duy nhất, không cần tạo ProjectorRemoteRepository, AirConditionerRemoteRepository hay MicrophoneRepository riêng biệt.");

        Console.Write("\nBạn có muốn nhập thêm 1 thiết bị mới để kiểm tra kho hoạt động bình thường không? y/n: ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            var newDev = InputDevice();
            try
            {
                AddDevice(newDev);
                Console.WriteLine($"-> Đã lưu thiết bị {newDev.Id} vào kho tương ứng thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI]: {ex.Message}");
            }
        }
    }

    // 12. Xem tất cả thiết bị
    private static void ShowAllDevices()
    {
        Console.WriteLine("=== DANH SÁCH TẤT CẢ THIẾT BỊ TRONG CÁC KHO ===");
        Console.WriteLine("1. Kho Điều khiển máy chiếu:");
        var prjs = projectorRepo.GetAll();
        if (prjs.Count == 0) Console.WriteLine("   Trống");
        foreach (var p in prjs) Console.WriteLine($"   - {p}");

        Console.WriteLine("\n2. Kho Điều khiển điều hòa:");
        var acs = acRepo.GetAll();
        if (acs.Count == 0) Console.WriteLine("   Trống");
        foreach (var ac in acs) Console.WriteLine($"   - {ac}");

        Console.WriteLine("\n3. Kho Micro không dây:");
        var mics = micRepo.GetAll();
        if (mics.Count == 0) Console.WriteLine("   Trống");
        foreach (var m in mics) Console.WriteLine($"   - {m}");
    }

    // Hàm nhập thông tin thiết bị từ bàn phím
    private static TeachingDevice InputDevice()
    {
        Console.WriteLine("Chọn loại thiết bị:");
        Console.WriteLine("1. Điều khiển máy chiếu");
        Console.WriteLine("2. Điều khiển điều hòa");
        Console.WriteLine("3. Micro không dây");
        Console.Write("Lựa chọn: ");
        string type = Console.ReadLine()?.Trim() ?? "1";

        string id = PromptNonEmpty("Mã thiết bị: ");
        string name = PromptNonEmpty("Tên thiết bị: ");
        string location = PromptNonEmpty("Vị trí lưu trữ: ");

        switch (type)
        {
            case "2":
                Console.Write("Danh sách phòng tương thích cách nhau bằng dấu phẩy ví dụ A201, A202: ");
                string? roomsInput = Console.ReadLine();
                var rooms = (roomsInput ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                return new AirConditionerRemote(id, name, location, rooms);

            case "3":
                int micBattery = PromptIntRange("Mức pin từ 0 đến 100: ", 0, 100);
                int signal = PromptIntRange("Chất lượng tín hiệu từ 0 đến 100: ", 0, 100);
                return new WirelessMicrophone(id, name, location, micBattery, signal);

            case "1":
            default:
                int projBattery = PromptIntRange("Mức pin từ 0 đến 100: ", 0, 100);
                return new ProjectorRemote(id, name, location, projBattery);
        }
    }

    // Kiểm tra xem mã thiết bị đã tồn tại trong bất kỳ kho nào chưa
    private static bool IsIdExists(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        return projectorRepo.FindById(id) != null
            || acRepo.FindById(id) != null
            || micRepo.FindById(id) != null;
    }

    // Phân loại và thêm vào kho tương ứng
    private static void AddDevice(TeachingDevice dev)
    {
        if (IsIdExists(dev.Id))
            throw new DuplicateIdException(dev.Id);

        if (dev is ProjectorRemote p)
            projectorRepo.Add(p);
        else if (dev is AirConditionerRemote a)
            acRepo.Add(a);
        else if (dev is WirelessMicrophone m)
            micRepo.Add(m);
    }

    private static void AddMinimumPresetManually()
    {
        Console.WriteLine("\n--- Khởi tạo 8 thiết bị mẫu theo yêu cầu đề bài ---");
        Console.Write("Bạn có muốn tạo tự động bộ dữ liệu mẫu để tiết kiệm thời gian gõ? y/n: ");
        string? opt = Console.ReadLine();

        if (opt?.Trim().ToLower() == "y")
        {
            TryAddDeviceSilent(new ProjectorRemote("PRJ01", "ĐK Máy chiếu Sony 1", "Kho A", 15));
            TryAddDeviceSilent(new ProjectorRemote("PRJ02", "ĐK Máy chiếu Panasonic 2", "Kho A", 50));
            TryAddDeviceSilent(new ProjectorRemote("PRJ03", "ĐK Máy chiếu Epson 3", "Kho B", 85));

            TryAddDeviceSilent(new AirConditionerRemote("AC01", "ĐK Điều hòa Daikin", "Kho A", new[] { "A201", "A202" }));
            TryAddDeviceSilent(new AirConditionerRemote("AC02", "ĐK Điều hòa LG", "Kho B", new[] { "A203", "A204" }));

            TryAddDeviceSilent(new WirelessMicrophone("MIC01", "Micro Shure 1", "Kho C", 20, 80));
            TryAddDeviceSilent(new WirelessMicrophone("MIC02", "Micro Sennheiser 2", "Kho C", 70, 30));
            TryAddDeviceSilent(new WirelessMicrophone("MIC03", "Micro Sony 3", "Kho C", 90, 85));

            Console.WriteLine("-> Đã khởi tạo thành công 8 thiết bị mẫu vào kho!");
        }
        else
        {
            Console.WriteLine("Vui lòng sử dụng mục 1 để nhập thủ công từng thiết bị.");
        }
    }

    private static void TryAddDeviceSilent(TeachingDevice dev)
    {
        try { AddDevice(dev); } catch { }
    }

    // Helper nhập chuỗi không rỗng
    private static string PromptNonEmpty(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();
            Console.WriteLine("Không được để trống. Vui lòng nhập lại.");
        }
    }

    // Helper nhập số nguyên trong khoảng
    private static int PromptIntRange(string message, int min, int max)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();
            if (int.TryParse(input, out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Giá trị phải là số nguyên từ {min} đến {max}. Vui lòng nhập lại.");
        }
    }
}
