import { setLastEmail, setToken } from "./storage.js";
import { showSection } from "./ui.js";
import { loadProfileData } from "./profile.js";

export async function handleAuth(event, url, type) {
  event.preventDefault();

  const formData = new FormData(event.target);
  const data = Object.fromEntries(formData.entries());

  if (data.email) setLastEmail(data.email);

  try {
    const response = await fetch(url, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorText = await response.text().catch(() => "Ukjent feil");
      alert("Feil: " + errorText);
      return;
    }

    if (type === "register") {
      await performAutoLogin(data.email, data.password);
    } else {
      const result = await response.json();
      processLoginSuccess(result);
    }
  } catch (e) {
    console.error(e);
    alert("Kunne ikke koble til serveren.");
  }
}

async function performAutoLogin(email, password) {
  try {
    const response = await fetch("/Auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    if (response.ok) {
      const result = await response.json();
      processLoginSuccess(result);
    } else {
      alert("Konto opprettet! Vennligst logg inn.");
      showSection("login");
    }
  } catch {
    showSection("login");
  }
}

function processLoginSuccess(result) {
  const token = result.token || result.Token;
  if (token) {
    setToken(token);
    loadProfileData();
  }
}
