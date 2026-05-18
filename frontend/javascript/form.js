import { createStudent } from "../api/student.api.js";

document.addEventListener("DOMContentLoaded", initializeForm);

function initializeForm() {
  const form = document.getElementById("formdata");

  form.addEventListener("submit", (event) => sendData(event, form));
}

async function sendData(event, form) {
  event.preventDefault();

  try {
    const formData = new FormData(form);

    const result = await createStudent(formData);

    console.log(result);

    alert("Student Created Successfully");

    window.location.href = "../admin.html";
  } catch (error) {
    console.error(error);

    alert(error.message);
  }
}
