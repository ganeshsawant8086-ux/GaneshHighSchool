// Ganesh High School - Interactive Frontend Script

document.addEventListener('DOMContentLoaded', function () {
    initSidebar();
    initLogoDropdown();
    initSectionNavigation();
    initGlobalSearch();
    initModalHandlers();
    initAjaxForms();
    initAttendanceInteractive();
    initFeeCalculator();
    initNoticePost();
    initIdCardGenerator();
    initStockManagement();
});

// Toast notification helper
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const toast = document.createElement('div');
    toast.className = `toast-custom ${type === 'danger' ? 'toast-danger' : ''}`;
    toast.innerHTML = `
        <i class="bi ${type === 'danger' ? 'bi-exclamation-triangle-fill text-danger' : 'bi-check-circle-fill text-success'} fs-5"></i>
        <span>${message}</span>
    `;

    container.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateY(20px)';
        toast.style.transition = 'all 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3500);
}

// Global sidebar controls
window.openSidebar = function () {
    const sidebar = document.getElementById('appSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    if (sidebar) sidebar.classList.add('show');
    if (overlay) overlay.classList.add('show');
};

window.closeSidebar = function () {
    const sidebar = document.getElementById('appSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    if (sidebar) sidebar.classList.remove('show');
    if (overlay) overlay.classList.remove('show');
};

window.toggleSidebar = function () {
    const sidebar = document.getElementById('appSidebar');
    if (sidebar && sidebar.classList.contains('show')) {
        window.closeSidebar();
    } else {
        window.openSidebar();
    }
};

// 1. Sidebar Toggle and Overlay (Three Small Lines Menu in Left Corner)
function initSidebar() {
    const sidebar = document.getElementById('appSidebar');
    const toggleBtn = document.getElementById('menuToggleBtn');
    const closeBtn = document.getElementById('sidebarCloseBtn');
    const overlay = document.getElementById('sidebarOverlay');

    if (toggleBtn) {
        toggleBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            window.toggleSidebar();
        });
    }

    if (closeBtn) {
        closeBtn.addEventListener('click', window.closeSidebar);
    }

    if (overlay) {
        overlay.addEventListener('click', window.closeSidebar);
    }

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            window.closeSidebar();
            if (window.closeLogoDropdown) window.closeLogoDropdown();
        }
    });
}

// 2. School Logo Icon Click & Interactive Menu Dropdown
function initLogoDropdown() {
    const logoBtn = document.getElementById('topbarLogoIcon');
    const dropdown = document.getElementById('logoDropdownMenu');
    const closeBtn = document.getElementById('logoDropdownCloseBtn');

    window.openLogoDropdown = function () {
        if (dropdown) dropdown.classList.add('show');
    };

    window.closeLogoDropdown = function () {
        if (dropdown) dropdown.classList.remove('show');
    };

    window.toggleLogoDropdown = function () {
        if (dropdown) {
            if (dropdown.classList.contains('show')) {
                window.closeLogoDropdown();
            } else {
                window.openLogoDropdown();
            }
        }
    };

    if (logoBtn) {
        logoBtn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            window.toggleLogoDropdown();
        });
    }

    if (closeBtn) {
        closeBtn.addEventListener('click', function (e) {
            e.preventDefault();
            e.stopPropagation();
            window.closeLogoDropdown();
        });
    }

    // Close when clicking outside
    document.addEventListener('click', function (e) {
        if (dropdown && dropdown.classList.contains('show')) {
            if (!dropdown.contains(e.target) && !logoBtn.contains(e.target)) {
                window.closeLogoDropdown();
            }
        }
    });

    // Wire up links inside dropdown
    document.querySelectorAll('.dropdown-item-link').forEach(link => {
        link.addEventListener('click', function (e) {
            const section = this.getAttribute('data-section');
            if (section) {
                e.preventDefault();
                switchSection(section);
                window.closeLogoDropdown();
            }
        });
    });
}

// 2. Section Switcher (All options work properly on click)
window.switchSection = function (sectionId) {
    if (!sectionId) return;

    // Normalize section identifier (e.g., 'students' -> 'students-section')
    if (!sectionId.endsWith('-section')) {
        sectionId = sectionId + '-section';
    }

    // Hide all sections
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(sec => sec.classList.remove('active'));

    // Show target section
    const targetSection = document.getElementById(sectionId);
    if (targetSection) {
        targetSection.classList.add('active');
    } else {
        const defaultSec = document.getElementById('dashboard-section');
        if (defaultSec) defaultSec.classList.add('active');
        sectionId = 'dashboard-section';
    }

    // Update active nav link in sidebar
    const navLinks = document.querySelectorAll('.sidebar-nav-link');
    navLinks.forEach(link => {
        if (link.getAttribute('data-section') === sectionId) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });

    // Update active module tab buttons
    const tabBtns = document.querySelectorAll('.module-tab-btn');
    tabBtns.forEach(btn => {
        if (btn.getAttribute('data-section') === sectionId) {
            btn.classList.add('active');
        } else {
            btn.classList.remove('active');
        }
    });

    // Close sidebar drawer smoothly after selecting an option
    const sidebar = document.getElementById('appSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    if (sidebar) sidebar.classList.remove('show');
    if (overlay) overlay.classList.remove('show');

    // Close school logo dropdown if open
    if (window.closeLogoDropdown) window.closeLogoDropdown();

    // Smooth scroll to top of content view
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

// Global function alias for inline onclick handlers
function switchSection(sectionId) {
    window.switchSection(sectionId);
}

function initSectionNavigation() {
    // Sidebar nav links
    document.querySelectorAll('.sidebar-nav-link').forEach(link => {
        link.addEventListener('click', function (e) {
            const section = this.getAttribute('data-section');
            if (section) {
                e.preventDefault();
                switchSection(section);
                const hash = this.getAttribute('href');
                if (hash && hash.startsWith('/#')) {
                    history.pushState(null, null, hash.replace('/', ''));
                }
            }
        });
    });

    // Module tab buttons
    document.querySelectorAll('.module-tab-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const section = this.getAttribute('data-section');
            if (section) {
                switchSection(section);
                const hash = '#' + section.replace('-section', '');
                history.pushState(null, null, hash);
            }
        });
    });

    // Check URL hash on page load (e.g., #students, #teachers)
    if (window.location.hash) {
        const hash = window.location.hash.substring(1);
        const sectionId = hash + '-section';
        if (document.getElementById(sectionId)) {
            switchSection(sectionId);
        }
    }
}

// 3. Global Search functionality (Lef-Right Searching Panel)
function initGlobalSearch() {
    const searchInput = document.getElementById('globalSearchInput');
    const clearBtn = document.getElementById('searchClearBtn');
    if (!searchInput) return;

    function doSearch() {
        const query = searchInput.value.toLowerCase().trim();
        if (clearBtn) {
            clearBtn.style.display = query.length > 0 ? 'flex' : 'none';
        }

        // Filter active section rows or cards
        const activeSection = document.querySelector('.content-section.active');
        if (!activeSection) return;

        // Filter table rows
        const rows = activeSection.querySelectorAll('tbody tr');
        rows.forEach(row => {
            const text = row.innerText.toLowerCase();
            row.style.display = text.includes(query) ? '' : 'none';
        });

        // Filter grid cards (teachers, passouts, classes, notices)
        const cards = activeSection.querySelectorAll('.teacher-card, .passout-card-v2, .class-card, .notice-item-card');
        cards.forEach(card => {
            const text = card.innerText.toLowerCase();
            card.style.display = text.includes(query) ? '' : 'none';
        });
    }

    searchInput.addEventListener('input', doSearch);

    if (clearBtn) {
        clearBtn.addEventListener('click', function () {
            searchInput.value = '';
            doSearch();
            searchInput.focus();
        });
    }
}

// 4. Modal Handlers
function openModal(id) {
    const modal = document.getElementById(id);
    if (modal) {
        modal.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(id) {
    const modal = document.getElementById(id);
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = '';
    }
}

window.openModal = openModal;
window.closeModal = closeModal;

window.openAddStudentModal = function () { openModal('addStudentModal'); };
window.closeAddStudentModal = function () { closeModal('addStudentModal'); };
function openAddStudentModal() { window.openAddStudentModal(); }
function closeAddStudentModal() { window.closeAddStudentModal(); }

window.openAddTeacherModal = function () { openModal('addTeacherModal'); };
window.closeAddTeacherModal = function () { closeModal('addTeacherModal'); };
function openAddTeacherModal() { window.openAddTeacherModal(); }
function closeAddTeacherModal() { window.closeAddTeacherModal(); }

window.openAddPassoutModal = function () { openModal('addPassoutModal'); };
window.closeAddPassoutModal = function () { closeModal('addPassoutModal'); };
function openAddPassoutModal() { window.openAddPassoutModal(); }
function closeAddPassoutModal() { window.closeAddPassoutModal(); }

// View Student Record Profile Modal handler
window.viewStudentRecord = function (id, name, standard, gender, age, mobile) {
    const badge = document.getElementById('viewStuIdBadge');
    if (badge) badge.textContent = '#' + id;

    const nameEl = document.getElementById('viewStuName');
    if (nameEl) nameEl.textContent = name;

    const initialsEl = document.getElementById('viewStuInitials');
    if (initialsEl) initialsEl.textContent = (name && name.length > 0) ? name[0] : 'S';

    const stdEl = document.getElementById('viewStuStandard');
    if (stdEl) stdEl.textContent = 'Class ' + standard + 'th Standard';

    const genEl = document.getElementById('viewStuGender');
    if (genEl) genEl.textContent = gender;

    const ageEl = document.getElementById('viewStuAge');
    if (ageEl) ageEl.textContent = age + ' Years';

    const mobEl = document.getElementById('viewStuMobile');
    if (mobEl) mobEl.textContent = mobile || 'Not Provided';

    const mobLink = document.getElementById('viewStuMobileLink');
    if (mobLink) {
        mobLink.href = (mobile && mobile !== 'N/A') ? 'tel:' + mobile : '#';
    }

    const editBtn = document.getElementById('viewStuEditBtn');
    if (editBtn) {
        editBtn.href = '/Students/Edit/' + id;
    }

    const idCardBtn = document.getElementById('viewStuIdCardBtn');
    if (idCardBtn) {
        idCardBtn.onclick = function () {
            window.closeViewStudentModal();
            if (window.viewStudentIDCard) {
                window.viewStudentIDCard(id);
            }
        };
    }

    openModal('viewStudentModal');
};

window.closeViewStudentModal = function () {
    closeModal('viewStudentModal');
};
function closeViewStudentModal() {
    window.closeViewStudentModal();
}

function initModalHandlers() {
    // Backdrop click to close
    document.querySelectorAll('.modal-backdrop-custom').forEach(modal => {
        modal.addEventListener('click', function (e) {
            if (e.target === this) {
                this.style.display = 'none';
                document.body.style.overflow = '';
            }
        });
    });

    // Escape key
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            document.querySelectorAll('.modal-backdrop-custom').forEach(m => {
                m.style.display = 'none';
            });
            document.body.style.overflow = '';
        }
    });
}

// 5. AJAX Forms - Add Data & Show Data Reactively
function initAjaxForms() {
    // Quick Add Student
    const studentForm = document.getElementById('quickStudentForm');
    if (studentForm) {
        studentForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const studentData = {
                FirstName: document.getElementById('stuFirstName').value.trim(),
                LastName: document.getElementById('stuLastName').value.trim(),
                StudentGender: document.getElementById('stuGender').value,
                Standard: parseInt(document.getElementById('stuStandard').value, 10),
                Age: parseInt(document.getElementById('stuAge').value, 10) || 14,
                MobileNumber: document.getElementById('stuMobile').value.trim()
            };

            if (!studentData.FirstName || !studentData.LastName || !studentData.StudentGender || !studentData.Standard) {
                showToast('Please fill out all required fields!', 'danger');
                return;
            }

            try {
                const res = await fetch('/Home/QuickAddStudent', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(studentData)
                });

                const data = await res.json();
                if (data.success) {
                    showToast(data.message, 'success');
                    closeAddStudentModal();
                    studentForm.reset();

                    // Prepend student row to table immediately!
                    const s = data.data;
                    const tbody = document.getElementById('studentsTableBody');
                    if (tbody) {
                        const tr = document.createElement('tr');
                        tr.id = `student-row-${s.id || s.ID}`;
                        tr.style.backgroundColor = '#ecfdf5';
                        tr.innerHTML = `
                            <td><span class="fw-bold text-primary">#${s.id || s.ID}</span></td>
                            <td>
                                <div class="user-avatar-cell">
                                    <div class="avatar-initials">${s.firstName[0]}${s.lastName[0]}</div>
                                    <div>
                                        <p class="avatar-info-name">${s.firstName} ${s.lastName}</p>
                                        <p class="avatar-info-sub">Student ID: ${s.id || s.ID}</p>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <span class="badge-pill-custom badge-class">Class ${s.standard}th</span>
                            </td>
                            <td>
                                <span class="badge-pill-custom ${s.studentGender === 'Male' ? 'badge-gender-male' : 'badge-gender-female'}">
                                    ${s.studentGender}
                                </span>
                            </td>
                            <td>${s.age || '14'} yrs</td>
                            <td>
                                <a href="tel:${s.mobileNumber}" class="text-decoration-none text-dark">
                                    <i class="bi bi-telephone text-primary me-1"></i>${s.mobileNumber}
                                </a>
                            </td>
                            <td>
                                <div class="action-btn-group">
                                    <button class="action-btn text-danger action-btn-danger" onclick="deleteStudent(${s.id || s.ID})" title="Delete">
                                        <i class="bi bi-trash"></i>
                                    </button>
                                </div>
                            </td>
                        `;
                        tbody.prepend(tr);

                        // Increment counters
                        const statCount = document.getElementById('totalStudentsStat');
                        if (statCount) statCount.textContent = parseInt(statCount.textContent || '0') + 1;
                        const badgeCount = document.getElementById('sidebarStudentCount');
                        if (badgeCount) badgeCount.textContent = parseInt(badgeCount.textContent || '0') + 1;
                    }
                } else {
                    showToast(data.message || 'Error adding student', 'danger');
                }
            } catch (err) {
                console.error(err);
                showToast('Network error while saving student', 'danger');
            }
        });
    }

    // Quick Add Teacher
    const teacherForm = document.getElementById('quickTeacherForm');
    if (teacherForm) {
        teacherForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const teacherData = {
                FullName: document.getElementById('teachFullName').value.trim(),
                SubjectExpertise: document.getElementById('teachSubject').value.trim(),
                Qualification: document.getElementById('teachQualification').value.trim(),
                YearsOfExperience: parseInt(document.getElementById('teachExperience').value, 10) || 1,
                Phone: document.getElementById('teachPhone').value.trim(),
                Email: document.getElementById('teachEmail').value.trim(),
                ProfessionalDescription: document.getElementById('teachDesc').value.trim()
            };

            try {
                const res = await fetch('/Home/QuickAddTeacher', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(teacherData)
                });

                const data = await res.json();
                if (data.success) {
                    showToast(data.message, 'success');
                    closeAddTeacherModal();
                    teacherForm.reset();

                    // Add teacher card to grid
                    const t = data.data;
                    const grid = document.getElementById('teachersGridContainer');
                    if (grid) {
                        const card = document.createElement('div');
                        card.className = 'teacher-card';
                        card.innerHTML = `
                            <div class="teacher-header">
                                <div class="teacher-avatar">${t.fullName.split(' ').map(n=>n[0]).join('').substring(0,2)}</div>
                                <div class="teacher-meta">
                                    <h5>${t.fullName}</h5>
                                    <p class="teacher-subject">${t.subjectExpertise}</p>
                                </div>
                            </div>
                            <div class="teacher-details">
                                <div><i class="bi bi-mortarboard me-1 text-primary"></i> ${t.qualification}</div>
                                <div><i class="bi bi-clock-history me-1 text-primary"></i> ${t.yearsOfExperience} Years Exp.</div>
                                <div><i class="bi bi-envelope me-1 text-primary"></i> ${t.email}</div>
                                <div><i class="bi bi-telephone me-1 text-primary"></i> ${t.phone}</div>
                            </div>
                        `;
                        grid.prepend(card);

                        const teacherStat = document.getElementById('totalTeachersStat');
                        if (teacherStat) teacherStat.textContent = parseInt(teacherStat.textContent || '0') + 1;
                    }
                } else {
                    showToast(data.message || 'Error adding teacher', 'danger');
                }
            } catch (err) {
                console.error(err);
                showToast('Failed to add teacher record', 'danger');
            }
        });
    }

    // Quick Add Passout
    const passoutForm = document.getElementById('quickPassoutForm');
    if (passoutForm) {
        passoutForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const passoutData = {
                Year: parseInt(document.getElementById('passYear').value, 10),
                FullName: document.getElementById('passFullName').value.trim(),
                Percentage: parseFloat(document.getElementById('passPercentage').value),
                Specialization: document.getElementById('passSpecialization').value.trim(),
                ExampleContact: document.getElementById('passContact').value.trim()
            };

            try {
                const res = await fetch('/Home/QuickAddPassout', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(passoutData)
                });

                const data = await res.json();
                if (data.success) {
                    showToast(data.message, 'success');
                    closeAddPassoutModal();
                    passoutForm.reset();

                    // Prepend card to passout grid
                    const p = data.data;
                    const grid = document.getElementById('passoutsGridContainer');
                    if (grid) {
                        const card = document.createElement('div');
                        card.className = 'passout-card-v2';
                        card.innerHTML = `
                            <div class="passout-header">
                                <span class="passout-year-badge">Class 10th - ${p.year}</span>
                                <span class="passout-score">${p.percentage}%</span>
                            </div>
                            <h4 class="passout-name">${p.fullName}</h4>
                            <div class="passout-spec">
                                <i class="bi bi-award-fill text-warning"></i>
                                <span>Stream: ${p.specialization || 'General'}</span>
                            </div>
                            <div class="passout-contact">
                                <span><i class="bi bi-envelope me-1"></i>${p.exampleContact || 'N/A'}</span>
                                <span class="badge bg-success-subtle text-success">Distinction</span>
                            </div>
                        `;
                        grid.prepend(card);

                        const passStat = document.getElementById('totalPassoutsStat');
                        if (passStat) passStat.textContent = parseInt(passStat.textContent || '0') + 1;
                    }
                } else {
                    showToast(data.message || 'Error adding passout', 'danger');
                }
            } catch (err) {
                console.error(err);
                showToast('Failed to add passout record', 'danger');
            }
        });
    }
}

// Delete student helper
async function deleteStudent(id) {
    if (!confirm('Are you sure you want to remove this student record?')) return;

    try {
        const res = await fetch(`/Home/QuickDeleteStudent?id=${id}`, { method: 'POST' });
        const data = await res.json();
        if (data.success) {
            showToast('Student deleted successfully', 'success');
            const row = document.getElementById(`student-row-${id}`);
            if (row) row.remove();

            const statCount = document.getElementById('totalStudentsStat');
            if (statCount) statCount.textContent = Math.max(0, parseInt(statCount.textContent || '0') - 1);
        } else {
            showToast(data.message || 'Failed to delete student', 'danger');
        }
    } catch (err) {
        showToast('Error communicating with server', 'danger');
    }
}

// 6. Interactive Attendance Marker
function initAttendanceInteractive() {
    window.toggleAttendance = function (btn, studentName) {
        if (btn.classList.contains('btn-success')) {
            btn.classList.remove('btn-success');
            btn.classList.add('btn-danger');
            btn.innerHTML = '<i class="bi bi-x-circle me-1"></i> Absent';
            showToast(`${studentName} marked as Absent`, 'danger');
        } else {
            btn.classList.remove('btn-danger');
            btn.classList.add('btn-success');
            btn.innerHTML = '<i class="bi bi-check-circle me-1"></i> Present';
            showToast(`${studentName} marked as Present`, 'success');
        }
        recalcAttendance();
    };
}

function recalcAttendance() {
    const presentBtns = document.querySelectorAll('.attendance-toggle-btn.btn-success').length;
    const totalBtns = document.querySelectorAll('.attendance-toggle-btn').length;
    const percent = totalBtns > 0 ? Math.round((presentBtns / totalBtns) * 100) : 95;
    
    const countEl = document.getElementById('todayPresentCount');
    if (countEl) countEl.textContent = `${presentBtns} / ${totalBtns}`;
    
    const badgeEl = document.getElementById('todayAttendancePercent');
    if (badgeEl) badgeEl.textContent = `${percent}% Rate`;
}

// 7. Interactive Fee Calculator
function initFeeCalculator() {
    const feeSelect = document.getElementById('feeClassSelect');
    const busCheck = document.getElementById('feeBusCheckbox');
    const labCheck = document.getElementById('feeLabCheckbox');
    const feeOutput = document.getElementById('feeTotalOutput');

    if (!feeSelect || !feeOutput) return;

    function updateFee() {
        let base = parseInt(feeSelect.value, 10) || 20000;
        let bus = busCheck && busCheck.checked ? 6000 : 0;
        let lab = labCheck && labCheck.checked ? 2500 : 0;
        let total = base + bus + lab;
        feeOutput.textContent = `₹${total.toLocaleString('en-IN')}`;
    }

    feeSelect.addEventListener('change', updateFee);
    if (busCheck) busCheck.addEventListener('change', updateFee);
    if (labCheck) labCheck.addEventListener('change', updateFee);
    updateFee();
}

// 8. Interactive Notice Board Post
function initNoticePost() {
    window.postNewNotice = function () {
        const titleInput = document.getElementById('newNoticeTitle');
        const descInput = document.getElementById('newNoticeDesc');
        const categoryInput = document.getElementById('newNoticeCategory');

        if (!titleInput || !descInput) return;
        const title = titleInput.value.trim();
        const desc = descInput.value.trim();
        const category = categoryInput ? categoryInput.value : 'General';

        if (!title || !desc) {
            showToast('Please enter title and description for the notice', 'danger');
            return;
        }

        const noticesContainer = document.getElementById('noticesListContainer');
        if (noticesContainer) {
            const card = document.createElement('div');
            card.className = 'custom-card notice-item-card mb-3';
            card.innerHTML = `
                <div class="custom-card-body d-flex gap-3 align-items-start">
                    <div class="card-icon-badge bg-primary-subtle text-primary">
                        <i class="bi bi-megaphone-fill"></i>
                    </div>
                    <div class="flex-grow-1">
                        <div class="d-flex align-items-center justify-content-between mb-1">
                            <h5 class="fw-bold mb-0">${title}</h5>
                            <span class="badge bg-primary-subtle text-primary">${category}</span>
                        </div>
                        <p class="text-muted small mb-1">${desc}</p>
                        <span class="text-secondary small"><i class="bi bi-clock me-1"></i> Posted Just now</span>
                    </div>
                </div>
            `;
            noticesContainer.prepend(card);
            showToast('Notice published successfully!', 'success');
            titleInput.value = '';
            descInput.value = '';
        }
    };
}

// 9. Interactive Student ID Card Generator & Preview Controls
function initIdCardGenerator() {
    const select = document.getElementById('idCardStudentSelect');
    if (!select) return;

    window.updateStudentIdCard = function (studentId) {
        let opt = null;
        if (studentId) {
            opt = select.querySelector(`option[value="${studentId}"]`);
            if (opt) select.value = studentId;
        } else {
            opt = select.options[select.selectedIndex];
        }
        if (!opt) return;

        const name = opt.getAttribute('data-name') || 'Student Name';
        const standard = opt.getAttribute('data-standard') || '10';
        const gender = opt.getAttribute('data-gender') || 'Male';
        const mobile = opt.getAttribute('data-mobile') || '9876543210';
        const idVal = opt.value || '1';
        const idFormatted = String(idVal).padStart(3, '0');
        const idBarcodeFormatted = String(idVal).padStart(4, '0');

        // Extract initials
        const parts = name.trim().split(/\s+/);
        let initials = 'ST';
        if (parts.length >= 2) {
            initials = (parts[0][0] || '') + (parts[parts.length - 1][0] || '');
        } else if (parts.length === 1 && parts[0].length > 0) {
            initials = parts[0].substring(0, 2);
        }

        const avatarText = document.getElementById('idCardAvatarText');
        const avatarBox = document.getElementById('idCardAvatar');
        const nameEl = document.getElementById('idCardFullName');
        const rollEl = document.getElementById('idCardRollNo');
        const standardEl = document.getElementById('idCardStandard');
        const genderEl = document.getElementById('idCardGender');
        const mobileEl = document.getElementById('idCardMobile');
        const barcodeText = document.getElementById('idCardBarcodeText');

        if (avatarText) avatarText.textContent = initials.toUpperCase();
        if (avatarBox) {
            if (gender === 'Female') {
                avatarBox.style.background = 'linear-gradient(135deg, #ec4899, #be185d)';
            } else {
                avatarBox.style.background = 'linear-gradient(135deg, #3b82f6, #1d4ed8)';
            }
        }
        if (nameEl) nameEl.textContent = name;
        if (rollEl) rollEl.textContent = `GHS-2026-${idFormatted}`;
        if (standardEl) standardEl.textContent = `Class ${standard}th`;
        if (genderEl) genderEl.textContent = gender;
        if (mobileEl) mobileEl.textContent = mobile;
        if (barcodeText) barcodeText.textContent = `*GHS-2026-${idBarcodeFormatted}*`;
    };

    window.prevStudentIdCard = function () {
        if (select.selectedIndex > 0) {
            select.selectedIndex--;
            window.updateStudentIdCard(select.value);
        } else {
            select.selectedIndex = select.options.length - 1;
            window.updateStudentIdCard(select.value);
        }
    };

    window.nextStudentIdCard = function () {
        if (select.selectedIndex < select.options.length - 1) {
            select.selectedIndex++;
            window.updateStudentIdCard(select.value);
        } else {
            select.selectedIndex = 0;
            window.updateStudentIdCard(select.value);
        }
    };

    window.viewStudentIDCard = function (studentId) {
        if (window.switchSection) {
            window.switchSection('dashboard-section');
        }
        if (select) {
            select.value = studentId;
            window.updateStudentIdCard(studentId);
        }
        const cardContainer = document.getElementById('idCardContainer');
        if (cardContainer) {
            cardContainer.scrollIntoView({ behavior: 'smooth', block: 'center' });
            cardContainer.style.outline = '3px solid #2563eb';
            cardContainer.style.boxShadow = '0 0 25px rgba(37, 99, 235, 0.4)';
            setTimeout(() => {
                cardContainer.style.outline = '';
                cardContainer.style.boxShadow = '';
            }, 1800);
        }
        showToast(`Loaded ID Card for student #${studentId}`, 'success');
    };

    window.printStudentIdCard = function () {
        showToast('Opening ID Card print layout...', 'success');
        setTimeout(() => {
            window.print();
        }, 300);
    };

    window.copyCardDetails = function () {
        const name = document.getElementById('idCardFullName')?.textContent || '';
        const roll = document.getElementById('idCardRollNo')?.textContent || '';
        const std = document.getElementById('idCardStandard')?.textContent || '';
        const mobile = document.getElementById('idCardMobile')?.textContent || '';
        const text = `Ganesh High School ID Card\nName: ${name}\nRoll: ${roll}\nClass: ${std}\nEmergency Contact: ${mobile}`;

        if (navigator.clipboard) {
            navigator.clipboard.writeText(text).then(() => {
                showToast('Student ID details copied to clipboard!', 'success');
            }).catch(() => {
                showToast('ID info: ' + name + ' (' + roll + ')', 'success');
            });
        } else {
            showToast('ID info: ' + name + ' (' + roll + ')', 'success');
        }
    };

    // Initialize with first student if available
    if (select.options.length > 0) {
        window.updateStudentIdCard(select.value);
    }
}

// 10. School Clerk Stock & Material Inventory Controls
function initStockManagement() {
    window.openAddStockModal = function () {
        const modal = document.getElementById('addStockModal');
        if (modal) modal.classList.add('show');
    };

    window.closeAddStockModal = function () {
        const modal = document.getElementById('addStockModal');
        if (modal) modal.classList.remove('show');
    };

    window.openRestockModal = function (id, itemName) {
        const modal = document.getElementById('restockStockModal');
        const idInput = document.getElementById('restockItemId');
        const nameEl = document.getElementById('restockItemNameText');
        if (idInput) idInput.value = id;
        if (nameEl) nameEl.textContent = itemName;
        if (modal) modal.classList.add('show');
    };

    window.closeRestockModal = function () {
        const modal = document.getElementById('restockStockModal');
        if (modal) modal.classList.remove('show');
    };

    window.filterStockCategory = function (category, btn) {
        const buttons = document.querySelectorAll('.stock-filter-btn');
        buttons.forEach(b => b.classList.remove('active', 'btn-dark'));
        buttons.forEach(b => b.classList.add('btn-outline-secondary'));

        if (btn) {
            btn.classList.add('active', 'btn-dark');
            btn.classList.remove('btn-outline-secondary');
        }

        const rows = document.querySelectorAll('#stockTableBody tr');
        rows.forEach(r => {
            const rowCat = r.getAttribute('data-category');
            if (category === 'all' || rowCat === category) {
                r.style.display = '';
            } else {
                r.style.display = 'none';
            }
        });
    };

    window.searchStockTable = function (query) {
        const term = (query || '').toLowerCase().trim();
        const rows = document.querySelectorAll('#stockTableBody tr');
        rows.forEach(r => {
            const name = r.getAttribute('data-name') || '';
            const sku = r.getAttribute('data-sku') || '';
            const text = r.textContent.toLowerCase();
            if (!term || name.includes(term) || sku.includes(term) || text.includes(term)) {
                r.style.display = '';
            } else {
                r.style.display = 'none';
            }
        });
    };

    window.quickAdjustStock = async function (id, change) {
        try {
            const resp = await fetch(`/Home/QuickAdjustStock?id=${id}&change=${change}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
            const data = await resp.json();
            if (data.success) {
                const qtyEl = document.getElementById(`stockQty-${id}`);
                const statusEl = document.getElementById(`stockStatus-${id}`);
                const barEl = document.getElementById(`stockBar-${id}`);
                const valEl = document.getElementById(`stockVal-${id}`);

                if (qtyEl) {
                    const unitPart = qtyEl.textContent.split(' ').slice(1).join(' ') || 'Units';
                    qtyEl.textContent = `${data.newQuantity} ${unitPart}`;
                }

                if (statusEl) {
                    statusEl.textContent = data.newStatus;
                    statusEl.className = 'badge rounded-pill px-2 py-1 ' + 
                        (data.newStatus === 'In Stock' ? 'bg-success-subtle text-success' : 
                        (data.newStatus === 'Low Stock' ? 'bg-warning-subtle text-warning fw-bold' : 'bg-danger-subtle text-danger'));
                }

                if (barEl) {
                    barEl.className = 'progress-bar ' + 
                        (data.newQuantity <= 0 ? 'bg-danger' : (data.newStatus === 'Low Stock' ? 'bg-warning' : 'bg-success'));
                }

                if (valEl && data.totalValue !== undefined) {
                    valEl.textContent = `₹${Math.round(data.totalValue).toLocaleString('en-IN')}`;
                }

                showToast(data.message, change > 0 ? 'success' : 'info');
            } else {
                showToast(data.message || 'Could not adjust stock', 'danger');
            }
        } catch (e) {
            showToast('Network error while updating stock', 'danger');
        }
    };

    window.deleteStockItem = async function (id, itemName) {
        if (!confirm(`Are you sure you want to delete '${itemName}' from stock inventory?`)) return;

        try {
            const resp = await fetch(`/Home/QuickDeleteStock?id=${id}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
            const data = await resp.json();
            if (data.success) {
                const row = document.querySelector(`#stockTableBody tr[data-id="${id}"]`);
                if (row) {
                    row.style.transition = 'all 0.3s ease';
                    row.style.opacity = '0';
                    row.style.transform = 'scale(0.95)';
                    setTimeout(() => row.remove(), 300);
                }
                showToast(data.message, 'success');
            } else {
                showToast(data.message || 'Error deleting item', 'danger');
            }
        } catch (e) {
            showToast('Network error deleting stock item', 'danger');
        }
    };

    window.printStockInventory = function () {
        showToast('Preparing School Material Inventory Sheet for printing...', 'info');
        setTimeout(() => window.print(), 350);
    };

    // Form: Add Stock
    const stockForm = document.getElementById('quickStockForm');
    if (stockForm) {
        stockForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            const payload = {
                itemName: document.getElementById('stockItemName')?.value?.trim(),
                category: document.getElementById('stockCategory')?.value,
                itemCode: document.getElementById('stockItemCode')?.value?.trim(),
                quantity: parseInt(document.getElementById('stockQuantity')?.value, 10) || 0,
                unit: document.getElementById('stockUnit')?.value || 'Units',
                minThreshold: parseInt(document.getElementById('stockMinThreshold')?.value, 10) || 10,
                unitPrice: parseFloat(document.getElementById('stockUnitPrice')?.value) || 0,
                location: document.getElementById('stockLocation')?.value?.trim() || 'Central Store Room',
                supplier: document.getElementById('stockSupplier')?.value?.trim() || null,
                remarks: document.getElementById('stockRemarks')?.value?.trim() || null
            };

            try {
                const resp = await fetch('/Home/QuickAddStock', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });
                const res = await resp.json();
                if (res.success) {
                    showToast(res.message, 'success');
                    window.closeAddStockModal();
                    stockForm.reset();
                    setTimeout(() => window.location.reload(), 800);
                } else {
                    showToast(res.message || 'Validation error saving stock', 'danger');
                }
            } catch (err) {
                showToast('Failed to connect to server', 'danger');
            }
        });
    }

    // Form: Restock Bulk
    const restockForm = document.getElementById('quickRestockForm');
    if (restockForm) {
        restockForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            const id = parseInt(document.getElementById('restockItemId')?.value, 10);
            const addQty = parseInt(document.getElementById('restockAddQty')?.value, 10);

            try {
                const resp = await fetch(`/Home/QuickRestockItem?id=${id}&addQuantity=${addQty}`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                });
                const res = await resp.json();
                if (res.success) {
                    showToast(res.message, 'success');
                    window.closeRestockModal();
                    setTimeout(() => window.location.reload(), 700);
                } else {
                    showToast(res.message || 'Error restocking item', 'danger');
                }
            } catch (err) {
                showToast('Failed to restock item', 'danger');
            }
        });
    }
}


