# Wolffun

Dự án game pixel destruction 2D được phát triển bằng Unity. Người chơi sử dụng vũ khí để phá hủy các vật thể pixel, thu thập XP và nâng cấp để tiến qua các level.

## Hướng dẫn mở project và chạy

### Yêu cầu hệ thống
- Unity 2022.3 LTS
- Windows/Mac/Linux
- Git

### Clone repository
1. Mở terminal hoặc command prompt
2. Chạy lệnh: `git clone https://github.com/Bunhiacopxki/Wolffun.git`
3. Chờ quá trình clone hoàn tất

### Cách mở project
1. Tải và cài đặt Unity Hub từ [unity.com](https://unity.com/download)
2. Mở Unity Hub, chọn "Add" > "Add project from disk"
3. Chọn thư mục `Wolffun` vừa clone (chứa file `Wolffun.sln`)
4. Unity sẽ tự động load project và các dependencies

### Cách chạy game
1. Sau khi mở project, mở scene InGameScene
2. Nhấn nút "Play" ở trên cùng của Unity Editor
3. Game sẽ chạy trong cửa sổ Game view

## Kiến trúc code

Dự án được tổ chức theo mô hình MVC với các system chính sau:

### Managers (Quản lý toàn cục)
- **GameManager**: Điều khiển luồng game chính, quản lý level progression, pause/resume, và các event toàn cục
- **XPManager**: Quản lý hệ thống XP và level up của người chơi
- **UpgradeManager**: Xử lý logic nâng cấp và hiển thị UI chọn upgrade
- **SawManager**: Quản lý các saw (vũ khí chính) và vị trí đặt chúng

### GamePlay (Logic gameplay)
- **TapToDestroyController**: Xử lý input tap để phá hủy vật thể
- **WinConditionTracker**: Theo dõi điều kiện thắng level
- **LevelData**: ScriptableObject chứa dữ liệu level (objects, obstacles, weapon slots)
- **BottomCollector**: Thu thập các debris rơi xuống đáy

### PixelObject (Hệ thống pixel destruction)
- **PixelObjectRoot**: Root component cho các vật thể pixel
- **PixelDamageSystem**: Xử lý damage và phá hủy pixel
- **PixelSplitSystem**: Chia nhỏ vật thể khi bị phá hủy
- **PixelVisualBuilder**: Tạo visual cho pixel objects
- **PixelDebrisFactory**: Tạo hiệu ứng debris khi phá hủy

### Player (Hệ thống người chơi)
- **PlayerLevelSystem**: Quản lý level và XP của người chơi
- **UpgradeApplier**: Áp dụng các upgrade đã chọn
- **UpgradeCardView/UpgradePopupView**: UI hiển thị upgrade options

### Level (Quản lý level)
- **LevelController**: Điều khiển spawning và logic level
- **LevelSpawner**: Spawn các objects theo dữ liệu level
- **LevelLayoutSpawner**: Tạo layout cho level
- **ObstacleView**: Hiển thị obstacles

### Weapon (Vũ khí)
- **Saw**: Vũ khí chính dạng cưa quay
- **Laser**: Vũ khí laser (có thể mở rộng)

## Hướng dẫn sử dụng Level Editor Tool

Level Editor Tool là một custom editor window trong Unity cho phép tạo và chỉnh sửa level một cách trực quan.

### Cách mở Level Editor
1. Trong Unity Editor, vào menu **Tools > Pixel Destruction > Level Editor**
2. Hoặc tìm trong Project window file `LevelData` và double-click để mở

### Giao diện chính
- **Level Data**: Chọn LevelData asset để chỉnh sửa
- **Level Info**: Hiển thị thông tin tổng quan level (số objects, obstacles, weapon slots)
- **Placement Mode**: Chọn loại element để đặt (SpawnObject, Obstacle, WeaponSlot)
- **Selection**: Chỉnh sửa element đã chọn

### Cách sử dụng trong Scene View
1. Chọn **Placement Mode** (ví dụ: SpawnObject)
2. Điền thông tin template (shape, material, scale, etc.)
3. Trong Scene View:
   - **Shift + Left Click**: Đặt element mới tại vị trí chuột
   - **Ctrl + Left Click**: Chọn element gần nhất để chỉnh sửa
4. Sử dụng handle để di chuyển element đã chọn
5. Nhấn **Delete Selected** để xóa element

### Tính năng Grid Snap
- Bật/tắt **Use Grid Snap** để căn chỉnh theo grid
- Điều chỉnh **Grid Size** để thay đổi độ lớn grid

### Lưu thay đổi
- Nhấn **Save Asset** để lưu LevelData
- Nhấn **Clear All** để xóa tất cả elements (có xác nhận)

## Những điều sẽ cải thiện nếu có thêm thời gian

### Gameplay Enhancements
- **Âm thanh**: Thêm sound effects cho việc phá hủy, upgrade, level up
- **Vũ khí mới**: Thêm nhiều loại vũ khí (bomb, drill, etc.)
- **Power-ups**: Temporary boosts như slow motion, multi-tap

### UI/UX Improvements
- **Tutorial system**: Hướng dẫn cho người chơi mới
- **Settings menu**: Điều chỉnh âm lượng, graphics quality
- **Level selection**: UI chọn level với preview
- **Responsive design**: Tối ưu cho nhiều screen size

### Technical Improvements
- **Object pooling**: Tối ưu performance cho pixel debris
- **Save system**: Lưu progress người chơi (current level, upgrades)
- **Analytics**: Theo dõi metrics gameplay để balance
- **Modding support**: Cho phép custom levels và assets
- **Multiplayer**: Chế độ chơi cùng nhau (local hoặc online)

### Code Architecture
- **Unit tests**: Thêm test cho các system quan trọng
- **Asset bundles**: Optimize loading và memory usage

### Content Expansion
- **Nhiều levels hơn**: Tạo level generator hoặc editor nâng cao
- **Boss fights**: Special levels với boss có pattern phức tạp
- **Themes**: Nhiều visual themes (space, underwater, etc.)
- **Achievements**: System thành tích và rewards
- **Daily challenges**: Content mới mỗi ngày