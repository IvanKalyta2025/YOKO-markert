import { getToken } from "./storage.js";
import { showSection, setAvatarEverywhere } from "./ui.js";
import { logout } from "./session.js";

export async function loadProfileData() {
  const token = getToken();
  if (!token) {
    showSection("login");
    return;
  }

  try {
    const response = await fetch("/api/Profile", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (response.ok) {
      const profile = await response.json();

      const fullName =
        `${profile.firstName || ""} ${profile.lastName || ""}`.trim() ||
        "Ukjent Bruker";
      const fullNameEl = document.getElementById("view-fullname");
      if (fullNameEl) fullNameEl.innerText = fullName;

      // avatar
      const serverAvatar = profile.avatarUrl || "";
      if (serverAvatar) {
        const busted =
          serverAvatar +
          (serverAvatar.includes("?") ? "&" : "?") +
          "v=" +
          Date.now();
        setAvatarEverywhere(busted);
      } else {
        setAvatarEverywhere("default-avatar.svg");
      }

      // fill edit fields
      const setValue = (id, value) => {
        const el = document.getElementById(id);
        if (el) el.value = value ?? "";
      };

      setValue("edit-firstname", profile.firstName || "");
      setValue("edit-lastname", profile.lastName || "");
      setValue("edit-age", profile.age ?? "");
      setValue("edit-gender", profile.gender || "");
      setValue("edit-hobby", profile.hobby || "");
      setValue("edit-birthplace", profile.myPlaceOfBirth || "");

      // view labels
      const setText = (id, value) => {
        const el = document.getElementById(id);
        if (el) el.innerText = value;
      };

      setText("view-age", profile.age ?? "—");
      setText("view-gender", profile.gender || "—");
      setText("view-hobby", profile.hobby || "—");
      setText("view-birthplace", profile.myPlaceOfBirth || "—");

      showSection("profile-view");
      return;
    }

    if (response.status === 404) {
      showSection("profile-edit");
      return;
    }

    // anything else -> logout
    logout();
  } catch (error) {
    console.error(error);
    showSection("login");
  }
}

export async function handleProfileUpdate(event) {
  event.preventDefault();

  const token = getToken();
  const formData = new FormData(event.target);

  const btn = event.target.querySelector("button[type='submit']");
  const originalText = btn?.innerText || "Lagre endringer";
  if (btn) {
    btn.innerText = "Lagrer...";
    btn.disabled = true;
  }

  try {
    const response = await fetch("/api/Profile", {
      method: "POST",
      headers: { Authorization: `Bearer ${token}` },
      body: formData,
    });

    if (response.ok) {
      await loadProfileData();
    } else {
      const err = await response.text().catch(() => "");
      alert(`Kunne ikke lagre profil. (${response.status}) ${err}`);
    }
  } catch {
    alert("Serverfeil.");
  } finally {
    if (btn) {
      btn.innerText = originalText;
      btn.disabled = false;
    }
  }
}
