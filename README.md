# ⚔️ Dũng Sĩ: Hồi Sinh Tại Hồ Hoàn Kiếm (Demo)

![Godot Engine](https://img.shields.io/badge/Godot_4-%23FFFFFF.svg?style=for-the-badge&logo=godot-engine)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Status](https://img.shields.io/badge/Status-Alpha_Release-red?style=for-the-badge)

Một dự án game nhập vai hành động 2D (Action RPG) phong cách Pixel Art, được phát triển trên nền tảng **Godot Engine 4** với ngôn ngữ **C#**. Trò chơi lấy bối cảnh giả tưởng tại Hồ Hoàn Kiếm, nơi người chơi hóa thân thành Dũng sĩ để sinh tồn và chiến đấu chống lại đội quân Quái Xương.

---

## 📸 Hình Ảnh Demo (Screenshots)

<p align="center">
  <img src="menu_start.png" width="800" alt="Màn hình Start Game">
  <br>
  <i>Giao diện Khởi động (Start Menu)</i>
</p>

<p align="center">
  <img src="giaodienGame.png" width="800" alt="Giao diện Gameplay">
  <br>
  <i>Giao diện chiến đấu với Quái vật và hiển thị Cấp độ (Gameplay)</i>
</p>

<p align="center">
  <img src="test.png" width="800" alt="Test Tính Năng">
  <br>
  <i>Giao diện thiết kế trên Godot Engine (Editor View)</i>
</p>

<p align="center">
  <img src="menu_gameover.png" width="800" alt="Màn hình Game Over">
  <br>
  <i>Giao diện Kết thúc (Game Over)</i>
</p>

---

## 🧠 Kiến Thức Yêu Cầu (Prerequisites)

Để tiếp cận và tự tay phát triển được dự án này, người học cần trang bị một số nền tảng kiến thức cơ bản:

1. **Lập Trình Hướng Đối Tượng (OOP) với C#:** Hiểu rõ cách khai báo Class, sử dụng thuộc tính (Properties), phương thức (Methods) và khái niệm Kế thừa (Inheritance) — ví dụ như `Player` kế thừa từ `CharacterBody2D`.
2. **Toán Học Không Gian Trục Tọa Độ:** Sử dụng `Vector2` để tính toán trục tọa độ (X, Y). Sử dụng hàm `DistanceTo()` để tính toán khoảng cách vật lý giữa 2 đối tượng nhằm kích hoạt tấn công.
3. **Kiến thức nền tảng về Godot Engine 4:**
   - Hiểu cấu trúc **Cây Node (Scene Tree)** và cách các Node giao tiếp với nhau (Sử dụng `GetNode<T>()`).
   - Phân biệt được sự khác nhau giữa vòng lặp `_Process` (Render UI/Logic nhẹ) và `_PhysicsProcess` (Xử lý di chuyển, va chạm vật lý).
   - Biết cách nối và sử dụng các Sự kiện (Signals) như `AnimationFinished`.

---

## 🎮 Tính Năng Hiện Tại (Core Features)

Dự án hiện đang ở phiên bản Demo nhưng đã hoàn thiện các cơ chế cốt lõi của một tựa game RPG:

* **Hệ thống Combat Vật Lý:** Xử lý va chạm (Collision) chính xác. Vũ khí (Kiếm) có tầm đánh riêng (130px), quái vật có tầm cắn riêng (110px). Hỗ trợ cơ chế sát thương ngẫu nhiên và **Tỷ lệ Chí Mạng (Critical Hit 20%)**.
* **AI Quái Vật (Enemy AI):** Quái xương tự động đi tuần tra ngẫu nhiên trên bản đồ (Wander State). Khi phát hiện người chơi trong tầm nhìn (600px), tự động chuyển sang trạng thái truy đuổi (Chase State) và tấn công.
* **Hệ thống Cày Cấp (Dynamic Leveling):** 
  * **Dũng sĩ:** Giết quái để tích lũy EXP và lên cấp. Mỗi lần lên cấp sẽ tự động hồi máu, tăng Máu tối đa (+20) và tăng Sát thương vật lý cơ bản (+5). UI hiển thị cấp độ và EXP theo thời gian thực.
  * **Tiến hóa Quái Vật:** Quái vật không cố định sức mạnh. Cứ Dũng sĩ lên 2 cấp, quái vật sẽ tự động thăng 1 cấp, tăng mạnh máu (+30) và sát thương, giúp game luôn giữ được độ khó.
* **Cơ Chế Hồi Sinh (Respawn System):** Quái vật sau khi bị tiêu diệt sẽ rơi vào trạng thái "giả chết" (vô hiệu hóa hình ảnh và va chạm), sau đó đếm ngược 5 giây để tái sinh tại một tọa độ an toàn (chỉ giới hạn trong khu vực bãi đất trống, không bị kẹt vào tường rừng).
* **UI/UX Menu:** Tích hợp giao diện UI tương tác bao gồm Màn hình Khởi động (Start Menu) và Màn hình Kết thúc (Game Over). Người chơi tương tác bằng thao tác Click chuột trực quan.

---

## 🛠️ Công Việc Đang Tiến Hành & Cần Khắc Phục (To-Do & Refactoring)

Những khía cạnh kỹ thuật và thiết kế đang được rà soát để tối ưu hóa trong ngắn hạn:

- [ ] **Refactor Hệ thống Chuyển động (Animation):** Đồng bộ hóa Animation chém kiếm khớp hoàn toàn với nhịp độ (Delta Time) của quá trình gây sát thương. Tránh tình trạng vung kiếm chưa chạm đích nhưng sát thương đã nổ.
- [ ] **Tách biệt Logic (Decoupling):** Đưa hệ thống xử lý HP và Damage ra một Class/Interface riêng (ví dụ: `IDamageable`) để dễ dàng áp dụng cho các loại quái vật và vật thể khác sau này, thay vì code cứng vào `Player.cs` và `Skeleton.cs`.
- [ ] **Tối ưu hóa Spawner:** Tách logic sinh sản quái vật ra một Node riêng biệt (`EnemySpawner`) quản lý vòng đời của nhiều quái vật cùng lúc (Object Pooling) thay vì quái tự hồi sinh chính nó.
- [ ] **Cân bằng Game (Balancing):** Chỉnh sửa lại hệ số scale máu và sát thương giữa Dũng sĩ và Quái vật ở các mốc Level cao (Level 7 - 10) để tránh mất cân bằng.

---

## 🚀 Định Hướng Phát Triển Tương Lai (Roadmap)

Dự án được định hướng mở rộng thành một tựa game sinh tồn RPG mang yếu tố Roguelite.

### Giai đoạn 1: Cải thiện Trải nghiệm (Polishing)
* **Audio/VFX:** Bổ sung nhạc nền (BGM), hiệu ứng âm thanh (SFX) khi vung kiếm, báo động quái vật, tiếng bước chân. Thêm hệ thống `Floating Text` để hiển thị số sát thương bay lên khi đánh trúng.
* **Hệ thống Vật phẩm (Loot System):** Quái vật có xác suất rớt ra Bình Máu (Hồi phục) hoặc Tiền Vàng (Tiền tệ).

### Giai đoạn 2: Đa dạng hóa Gameplay
* **Kỹ Năng Active:** Bổ sung thanh Mana. Thiết kế kỹ năng Lướt (Dash) để né đòn và chiêu thức "Chém Lốc Xoáy" (Tornado Slash) gây sát thương diện rộng (AOE).
* **Hệ thống Quái vật Mới:** Thêm Quái Xương Cung Thủ (đánh xa) và Phù Thủy Xương (Tạo bẫy độc).
* **Cơ chế Boss Fight:** Xuất hiện Boss Khổng Lồ ở Cấp độ 10 với bộ kỹ năng độc lập, yêu cầu người chơi phải hit-and-run thay vì đứng tank sát thương.

### Giai đoạn 3: Thế giới và Cốt truyện
* **Bản đồ mở rộng:** Mở cổng dịch chuyển sang các khu vực bản đồ mới (Cột cờ Hà Nội, Đền Ngọc Sơn,...).
* **Save/Load System:** Lưu trạng thái màn chơi, cấp độ và trang bị vào máy tính người dùng.

---

## 💻 Hướng Dẫn Cài Đặt (Cho Developer)

Dự án được viết hoàn toàn bằng C# trên môi trường .NET. Để chạy trực tiếp mã nguồn:

1. Yêu cầu tải và cài đặt [Godot Engine 4 (.NET version)](https://godotengine.org/).
2. Đảm bảo máy tính đã cài đặt .NET SDK 6.0 trở lên.
3. Clone repository này về máy:
   ```bash
   git clone https://github.com/HOANGTHI2509/Lap_Trinh_Game.git
   ```
4. Mở Godot, chọn `Import` và trỏ vào file `project.godot`.
5. Đợi hệ thống Build lại file `Dung si.sln`. Nhấn `F5` để tận hưởng!
