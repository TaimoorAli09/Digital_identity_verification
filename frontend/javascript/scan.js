// scan.js
import { verifyStudentCard } from "../api/student.api.js";

document.addEventListener("DOMContentLoaded", initializeURLVerification);

async function initializeURLVerification() {
    console.log("🚀 Scan Script Initialized");

    try {
        // 1. Extract and Decode parameters from the URL
        const urlParams = new URLSearchParams(window.location.search);
        
        // Use decodeURIComponent to fix Base64 symbols like +, /, and =
        const tokenParam = urlParams.get("token") ? decodeURIComponent(urlParams.get("token")) : null;
        const signatureParam = urlParams.get("signature") ? decodeURIComponent(urlParams.get("signature")) : null;

        console.log("📦 URL Params Found:", { token: tokenParam, signature: signatureParam });

        // Hide the loading text once processing begins
        const loadingEl = document.getElementById("loading-state");
        if (loadingEl) loadingEl.style.display = "none";

        // Validation Check
        if (!tokenParam || !signatureParam) {
            console.error("❌ Missing Token or Signature in URL");
            showFailureUI(
                "INVALID QR CODE",
                "The scanned link is missing security credentials."
            );
            return;
        }

        console.log("📡 Sending data to Backend for verification...");
        
        // 2. Call the API
        const studentData = await verifyStudentCard(tokenParam, signatureParam);

        console.log("✅ Backend Response Received:", studentData);

        // 3. Render the UI
        showSuccessUI(studentData);

    } catch (error) {
        console.error("❌ Verification Flow Failed:", error);
        
        // This will catch Network errors (Backend down) or 400 Bad Requests (Fake QR)
        showFailureUI(
            "ACCESS DENIED",
            error.message || "Security validation failed."
        );
    }
}

function showSuccessUI(student) {
    console.log("🎨 Rendering Success UI for:", student.name || student.Name);
    
    const panel = document.getElementById("result-panel");
    const badge = document.getElementById("status-badge");

    if (!panel || !badge) {
        console.error("❌ Could not find 'result-panel' or 'status-badge' in HTML");
        return;
    }

    // Update Badge
    badge.className = "status-badge valid";
    badge.innerText = "✔ VERIFIED ENTRY PERMITTED";

    // Update Text Details (handling both camelCase and PascalCase)
    setElementText("verified-name", student.name || student.Name);
    setElementText("verified-program", student.program || student.Program);
    setElementText("verified-cnic", student.cnic || student.Cnic);
    setElementText("verified-age", `${student.age || student.Age || 0} Years Old`);

    // Handle Image
    const avatarImg = document.getElementById("verified-avatar");
    const currentImgUrl = student.imageUrl || student.ImageUrl;
    
    if (avatarImg && currentImgUrl) {
        avatarImg.src = currentImgUrl;
        avatarImg.style.display = "block";
    }

    // Show the panel
    panel.classList.remove("hidden");
}

function showFailureUI(statusTitle, errorMessage) {
    const panel = document.getElementById("result-panel");
    const badge = document.getElementById("status-badge");

    if (badge) {
        badge.className = "status-badge invalid";
        badge.innerText = `❌ ${statusTitle}`;
    }

    setElementText("verified-name", "Security Warning");
    setElementText("verified-program", errorMessage);
    
    // Hide the image on failure
    const avatarImg = document.getElementById("verified-avatar");
    if (avatarImg) avatarImg.style.display = "none";

    if (panel) panel.classList.remove("hidden");
}

// Helper function to prevent script crashes if an ID is missing in HTML
function setElementText(id, text) {
    const el = document.getElementById(id);
    if (el) {
        el.innerText = text || "N/A";
    } else {
        console.warn(`⚠️ Warning: HTML element with ID '${id}' was not found.`);
    }
}