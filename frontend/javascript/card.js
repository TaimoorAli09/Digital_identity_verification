export function createStudentCard(student) {
    try {
        const template = document.getElementById("student-card-template");
        
        if (!template) {
            console.error("❌ Template 'student-card-template' not found in DOM");
            return null;
        }

        const clone = template.content.cloneNode(true);

        // Debug: Log incoming student data
        console.log("📦 Student Data:", student);

        // Backend returns PascalCase (StudentId, Name, Age, etc.)
        // Set Text Values with safe fallbacks
        const studentIdEl = clone.querySelector(".student-id");
        if (studentIdEl) studentIdEl.innerText = student.StudentId || student.studentId || "N/A";

        const nameEl = clone.querySelector(".name-text");
        if (nameEl) nameEl.innerText = student.Name || student.name || "Unknown";

        const programEl = clone.querySelector(".program-tag");
        if (programEl) programEl.innerText = student.Program || student.program || "General";

        const ageEl = clone.querySelector(".age-val");
        if (ageEl) ageEl.innerText = student.Age || student.age || "0";

        const cnicEl = clone.querySelector(".cnic-val");
        if (cnicEl) cnicEl.innerText = student.Cnic || student.cnic || "---";

        // Set Student Photo
        const imgEl = clone.querySelector(".student-img");
        if (imgEl && (student.ImageUrl || student.imageUrl)) {
            imgEl.src = student.ImageUrl || student.imageUrl;
            imgEl.onerror = () => {
                console.warn("⚠️ Failed to load image:", student.ImageUrl || student.imageUrl);
                imgEl.src = "https://placehold.co/180x180?text=No+Photo";
            };
        }

        // Set QR Code
        const qrEl = clone.querySelector(".qr-img");
        if (qrEl && (student.qrBase64 || student.QrBase64)) {
            qrEl.src = `data:image/png;base64,${student.qrBase64 || student.QrBase64}`;
        }

        console.log("✅ Card created for:", student.Name || student.name);
        return clone;
    } catch (error) {
        console.error("❌ Error creating student card:", error);
        return null;
    }
}