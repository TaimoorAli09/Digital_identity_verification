const BASE_URL = "http://localhost:5299/api/Student";


// create student api call
export async function createStudent(formData) {
  const response = await fetch(`${BASE_URL}/create`, {
    method: "POST",
    body: formData,
  });

  
  return await response.json();
}

export async function getAllStudents() {
  const response = await fetch(`${BASE_URL}/getall`);

  return await response.json();
}

export async function deleteStudent(id) {
  const response = await fetch(`${BASE_URL}/delete/${id}`, {
    method: "DELETE",
  });

  return await response.json();
}
