// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(function () {
	function formValidateRequired(id) {
		var el = document.getElementById(id);
		if (!el) return false;
		if (el.value.trim() === "") {
			el.style.border = '2px solid red';
			return false;
		}
		el.style.border = '';
		return true;
	}

	async function submitForm(form) {
		var url = form.getAttribute('action') || window.location.pathname;
		var btn = form.querySelector('button[type="submit"]');
		var formData = new FormData(form);

		// Use processingBtn from common.js if available
		if (typeof processingBtn === 'function' && btn && btn.id) {
			processingBtn(btn.id);
		}

		try {
			var resp = await fetch(url, {
				method: 'POST',
				headers: { 'X-Requested-With': 'XMLHttpRequest' },
				body: formData
			});
			var json = await resp.json();
			if (json && json.success) {
				if (typeof successAlert === 'function') {
					successAlert(json.message || 'Submitted');
				} else if (window.Swal) {
					Swal.fire('Success', json.message || 'Submitted', 'success');
				}
				form.reset();
			} else {
				var msg = (json && json.message) ? json.message : 'Submission failed';
				if (typeof errorAlert === 'function') errorAlert(msg); else if (window.Swal) Swal.fire('Error', msg, 'error');
			}
		} catch (e) {
			console.error(e);
			if (typeof errorAlert === 'function') errorAlert('An error occurred'); else if (window.Swal) Swal.fire('Error', 'An error occurred', 'error');
		} finally {
			if (typeof returnDefaultBtn === 'function' && btn && btn.id) {
				returnDefaultBtn(btn.id);
			}
		}
	}

	document.addEventListener('DOMContentLoaded', function () {
		// Main contact form
		var mainForm = document.getElementById('mainContactForm');
		if (mainForm) {
			mainForm.addEventListener('submit', function (e) {
				e.preventDefault();

				var ok = true;
				ok = formValidateRequired('FirstName') && ok;
				ok = formValidateRequired('LastName') && ok;
				ok = formValidateRequired('Email') && ok;
				ok = formValidateRequired('Message') && ok;

				// validateEmail from common.js
				if (ok && typeof validateEmail === 'function') {
					ok = validateEmail('Email');
				}

				if (!ok) {
					if (typeof infoAlert === 'function') infoAlert('Please correct the highlighted fields');
					else if (window.Swal) Swal.fire('Validation', 'Please correct the highlighted fields', 'warning');
					return;
				}

				submitForm(mainForm);
			});
		}

		// Modal contact form
		var modalForm = document.getElementById('modalContactForm');
		if (modalForm) {
			modalForm.addEventListener('submit', function (e) {
				e.preventDefault();
				var ok = true;
				// modal_Name, modal_Email, modal_Message
				ok = formValidateRequired('modal_Name') && ok;
				ok = formValidateRequired('modal_Email') && ok;
				ok = formValidateRequired('modal_Message') && ok;

				if (ok && typeof validateEmail === 'function') {
					ok = validateEmail('modal_Email');
				}

				if (!ok) {
					if (typeof infoAlert === 'function') infoAlert('Please correct the highlighted fields');
					else if (window.Swal) Swal.fire('Validation', 'Please correct the highlighted fields', 'warning');
					return;
				}

				submitForm(modalForm);
			});
		}

		// Newsletter subscription form
		var newsletterForm = document.getElementById('newsletterForm');
		if (newsletterForm) {
			newsletterForm.addEventListener('submit', function (e) {
				e.preventDefault();

				var ok = formValidateRequired('newsletterEmail');

				// Validate email format
				if (ok && typeof validateEmail === 'function') {
					ok = validateEmail('newsletterEmail');
				}

				if (!ok) {
					if (typeof infoAlert === 'function') infoAlert('Please enter a valid email address');
					else if (window.Swal) Swal.fire('Validation', 'Please enter a valid email address', 'warning');
					return;
				}

				submitForm(newsletterForm);
			});
		}
	});
})();
