import { getCurrentEmail, setLastEmail } from "./storage.js";
import { showSection } from "./ui.js";
import { logout } from "./session.js";

// helper: prefill public change password email from login form
export function prefillEmailToPublicChangePassword() {
  const loginEmailInput = document.querySelector("#login input[name='email']");
  const email = (loginEmailInput?.value || getCurrentEmail() || "").trim();

  const cpEmail = document.getElementById("cp-email");
  if (cpEmail && email) cpEmail.value = email;

  showSection("change-password-public");
}

// CHANGE PASSWORD (PUBLIC, NO TOKEN REQUIRED)
export async function handleChangePasswordPublic(event) {
  event.preventDefault();

  const email = document.getElementById("cp-email")?.value.trim() || "";
  const currentPassword =
    document.getElementById("cp-current-password")?.value || "";
  const newPassword = document.getElementById("cp-new-password")?.value || "";
  const confirmPassword =
    document.getElementById("cp-confirm-password")?.value || "";

  if (newPassword !== confirmPassword) {
    alert("Passordene er ikke like.");
    return;
  }

  const btn = event.target.querySelector("button[type='submit']");
  const originalText = btn?.innerText || "Bytt passord";
  if (btn) {
    btn.innerText = "Bytter...";
    btn.disabled = true;
  }

  try {
    const resp = await fetch("/Auth/change-password", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        Email: email,
        CurrentPassword: currentPassword,
        NewPassword: newPassword,
      }),
    });

    if (resp.ok) {
      alert("Passord endret! Du kan logge inn nå.");

      // keep email for login convenience
      setLastEmail(email);

      // clear fields
      const clear = (id) => {
        const el = document.getElementById(id);
        if (el) el.value = "";
      };
      clear("cp-current-password");
      clear("cp-new-password");
      clear("cp-confirm-password");

      showSection("login");
      return;
    }

    const err = await resp.text().catch(() => "");
    alert(`Kunne ikke endre passord. (${resp.status}) ${err}`.trim());
  } catch {
    alert("Serverfeil.");
  } finally {
    if (btn) {
      btn.innerText = originalText;
      btn.disabled = false;
    }
  }
}

// CHANGE PASSWORD (LOGGED IN)
export async function handleChangePasswordLoggedIn(event) {
  event.preventDefault();

  const email = getCurrentEmail();
  const currentPassword =
    document.getElementById("current-password")?.value || "";
  const newPassword = document.getElementById("new-password")?.value || "";
  const confirmPassword =
    document.getElementById("confirm-password")?.value || "";

  if (!email) {
    alert("Fant ikke e-post. Logg inn på nytt.");
    logout();
    return;
  }

  if (newPassword !== confirmPassword) {
    alert("Passordene er ikke like.");
    return;
  }

  const btn = event.target.querySelector("button[type='submit']");
  const originalText = btn?.innerText || "Bytt passord";
  if (btn) {
    btn.innerText = "Bytter...";
    btn.disabled = true;
  }

  try {
    const token = localStorage.getItem("token"); // optional
    const headers = { "Content-Type": "application/json" };
    if (token) headers.Authorization = `Bearer ${token}`;

    const resp = await fetch("/Auth/change-password", {
      method: "POST",
      headers,
      body: JSON.stringify({
        Email: email,
        CurrentPassword: currentPassword,
        NewPassword: newPassword,
      }),
    });

    if (resp.ok) {
      alert("Passord endret!");
      const clear = (id) => {
        const el = document.getElementById(id);
        if (el) el.value = "";
      };
      clear("current-password");
      clear("new-password");
      clear("confirm-password");
      showSection("profile-view");
      return;
    }

    const err = await resp.text().catch(() => "");
    alert(`Kunne ikke endre passord. (${resp.status}) ${err}`.trim());
  } catch {
    alert("Serverfeil.");
  } finally {
    if (btn) {
      btn.innerText = originalText;
      btn.disabled = false;
    }
  }
}
