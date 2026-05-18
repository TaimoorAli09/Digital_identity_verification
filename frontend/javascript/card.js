export function createStudentCard(student) {
  const template = document.getElementById("student-card-template");

  const clone = template.content.cloneNode(true);

  const setText = (selector, text) => {
    const el = clone.querySelector(selector);
    if (el) el.innerText = text ?? "";
  };

  setText(".name", student.name);
  setText(".student-id", student.studentId || "");
  setText(".program", student.program || "");
  setText(".age", student.age ?? "");
  setText(".cnic", student.cnic || "N/A");

  // RIGHT SIDE
  const imageEl = clone.querySelector(".student-img") || clone.querySelector(".image");
  if (imageEl) {
    imageEl.src = student.imageUrl || "https://via.placeholder.com/150?text=No+Photo";
  }

  // QR (if backend sends it)
  const qrEl = clone.querySelector(".qr-img");
  if (qrEl) {
    if (student.qrBase64) {
      qrEl.src = `data:image/png;base64,${student.qrBase64}`;
    } else if (student.qrUrl) {
      qrEl.src = student.qrUrl;
    }
  }

  return clone;
}