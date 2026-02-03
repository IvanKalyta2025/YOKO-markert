let initialRenderDone = false;

export function endInitialRender() {
  if (initialRenderDone) return;
  initialRenderDone = true;
  document.documentElement.classList.remove("js-loading");
}

export function showSection(id) {
  document.body.classList.toggle("landing-mode", id === "landing");

  document.querySelectorAll(".form-section").forEach((s) => {
    s.classList.remove("active");
    s.style.display = "none";
  });

  const target = document.getElementById(id);
  if (target) {
    target.style.display = "flex";
    // small delay for CSS transitions
    setTimeout(() => target.classList.add("active"), 10);
  }

  endInitialRender();
}

export function setAvatarEverywhere(src) {
  if (!src) return;
  const view = document.getElementById("view-avatar");
  const edit = document.getElementById("edit-avatar-preview");
  if (view) view.src = src;
  if (edit) edit.src = src;
}

export function previewImage(input) {
  if (input.files && input.files[0]) {
    const reader = new FileReader();
    reader.onload = (e) => {
      const img = document.getElementById("edit-avatar-preview");
      if (img) img.src = e.target?.result;
    };
    reader.readAsDataURL(input.files[0]);
  }
}
