// Inside your student.api.js
// ❗ CHANGE THIS TO YOUR ACTIVE MACHINE IP FROM IPCONFIG (Match your appsettings)
// const BASE_URL = "http://192.168.1.10:5299/api/Student"; 
const BASE_URL = "/api/Student";

async function safeParse(response) {
  const text = await response.text();
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}

// CREATE
export async function createStudent(formData) {
  const response = await fetch(`${BASE_URL}/create`, {
    method: "POST",
    body: formData,
  });
  if (!response.ok) {
    throw new Error(await response.text());
  }
  return await response.json();
}

// VERIFY SCAN
export async function verifyStudentCard(token, signature) {
  const response = await fetch(`${BASE_URL}/verify`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ token, signature }),
  });

  const data = await safeParse(response);
  if (!response.ok) {
    throw new Error(data.status || data || "Verification failed");
  }
  return data;
}

// GET ALL
export async function getAllStudents() {
  try {
    console.log(`🔄 Fetching from: ${BASE_URL}/getall`);
    const response = await fetch(`${BASE_URL}/getall`);
    console.log(`📊 Response status: ${response.status}`);
    
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`HTTP ${response.status}: ${errorText}`);
    }
    return await response.json();
  } catch (error) {
    console.error(`❌ getAllStudents failed:`, error);
    throw error;
  }
}

// DELETE
export async function deleteStudent(id) {
  const response = await fetch(`${BASE_URL}/delete/${id}`, {
    method: "DELETE",
  });
  if (!response.ok) {
    throw new Error(await response.text());
  }
  return await response.json();
}