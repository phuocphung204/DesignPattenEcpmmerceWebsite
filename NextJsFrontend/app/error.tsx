'use client'; // File error bắt buộc phải là Client Component

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  return (
    <div className="p-4 border border-red-500 rounded">
      <h2>Đã có lỗi xảy ra khi tải dữ liệu!</h2>
      <p className="text-sm text-gray-500">{error.message}</p>
      <button
        onClick={() => reset()} // Thử tải lại trang
        className="mt-2 bg-blue-500 text-white px-4 py-2 rounded"
      >
        Thử lại
      </button>
    </div>
  );
}