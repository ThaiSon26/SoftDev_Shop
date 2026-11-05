# Hướng Dẫn Đóng Góp Vào Dự Án (Contributing Guide)

Chào mừng bạn đến với dự án **Website Bán Phần Mềm Bản Quyền - Nhóm 6**.

## 1. Quy trình làm việc (Git Workflow)
- Mọi thành viên **không được commit trực tiếp lên nhánh `main`**.
- Mỗi tính năng hoặc sửa lỗi phải được tạo trên một nhánh riêng từ nhánh `main`:
  - Tính năng mới: `feature/ten-tinh-nang` (Ví dụ: `feature/product-detail`)
  - Sửa lỗi: `fix/ten-loi` (Ví dụ: `fix/cart-calculation-error`)

## 2. Quy chuẩn đặt tên Commit (Conventional Commits)
- `feat:` Khi thêm tính năng mới.
- `fix:` Khi sửa lỗi.
- `docs:` Khi chỉnh sửa tài liệu.
- `test:` Khi viết mã kiểm thử Unit Test.
- `chore:` Khi cấu hình thư viện, cài đặt dự án.

## 3. Quy trình gửi Pull Request (PR)
1. Đẩy nhánh của bạn lên GitHub: `git push origin feature/...`
2. Mở Pull Request trỏ vào nhánh `main`.
3. Chờ hệ thống GitHub Actions kiểm tra (CI Build & Test).
4. Maintainer sẽ xem xét, nhận xét (Review code) và chấp thuận (Approve) trước khi tiến hành Merge.