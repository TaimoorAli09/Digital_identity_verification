import { getAllStudents } from "../api/student.api.js";
import { createStudentCard } from "./card.js";

document.addEventListener("DOMContentLoaded", initializePage);

function initializePage() {
  loadAllStudents();

  document
    .getElementById("add-student-button")
    .addEventListener("click", () => {
      window.location.href = "form.html";
    });
}

async function loadAllStudents() {
  const students = await getAllStudents();

  const container = document.querySelector(".content");
  container.innerHTML = "";

  students.forEach((student) => {
    const card = createStudentCard(student);

    container.appendChild(card);
  });
}