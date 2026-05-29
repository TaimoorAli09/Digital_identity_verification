import { getAllStudents } from "../services/student.api.js";
import { createStudentCard } from "./card.js"; // Ensure this path is correct

document.addEventListener("DOMContentLoaded", initializePage);

async function initializePage() {
    // 1. Setup Button
    const addBtn = document.getElementById("add-student-button");
    if(addBtn) {
        addBtn.addEventListener("click", () => {
            window.location.href = "form.html";
        });
    }

    // 2. Load Data
    await loadAllStudents();
}

async function loadAllStudents() {
    try {
        const students = await getAllStudents();
        const container = document.querySelector(".content");
        
        if (!container) {
            console.error("❌ Container '.content' not found");
            return;
        }

        console.log("📊 Backend returned students:", students);

        if (!students || students.length === 0) {
            container.innerHTML = "<p style='padding: 40px; text-align: center; color: #999;'>No students found</p>";
            return;
        }

        container.innerHTML = "";

        let successCount = 0;
        students.forEach((student) => {
            const card = createStudentCard(student);
            if (card) {
                container.appendChild(card);
                successCount++;
            }
        });

        console.log(`✅ Successfully rendered ${successCount}/${students.length} cards`);
    } catch (error) {
        console.error("❌ Failed to load students:", error);
        const container = document.querySelector(".content");
        if (container) {
            container.innerHTML = `<p style='padding: 40px; text-align: center; color: #e11d48;'>Error loading students: ${error.message}</p>`;
        }
    }
}