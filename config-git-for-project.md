## 📋 Yêu cầu trước khi thực hiện

1. Máy tính đã cài đặt **Git**.
2. Thư mục dự án đã được khởi tạo Git (đã chạy `git init` hoặc `git clone`).

---

## Các bước thực hiện

### Bước 1: Mở Terminal tại thư mục dự án

Mở Terminal (hoặc Git Bash, VS Code Terminal) và di chuyển vào thư mục gốc của dự án.

```bash
cd /duong-dan/toi/du-an-cua-ban

```

### Bước 2: Kiểm tra cấu hình hiện tại (Tùy chọn)

Trước khi thay đổi, bạn có thể kiểm tra xem dự án đang nhận email nào (thường là email Global).

```bash
git config user.email
git config user.name

```

### Bước 3: Thiết lập cấu hình Local

Sử dụng flag `--local` để ghi đè cấu hình chỉ riêng cho dự án này.

**1. Đặt tên hiển thị (Commit Author Name):**
Tên này sẽ hiển thị khi bạn thực hiện commit trong dự án. Nên để đầy đủ họ và tên.

```bash
git config --local user.name "Tên Của Bạn Tại Dự Án Này"

```

**2. Đặt Email (Commit Author Email):**
Email này sẽ hiển thị khi bạn thực hiện commit trong dự án. Nên dùng email liên quan đến dự án (ví dụ: email công ty, email trường học).

```bash
git config --local user.email "email-du-an@example.com"

```

### Bước 4: Kiểm tra lại

Chạy lại lệnh sau để đảm bảo Git đã nhận cấu hình mới:

```bash
git config user.email

```

> **Kết quả:** Nếu màn hình hiện ra email bạn vừa nhập ở Bước 3 thì bạn đã thành công.
