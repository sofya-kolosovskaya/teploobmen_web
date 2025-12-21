// Основные JavaScript функции для сайта

// Инициализация при загрузке страницы
document.addEventListener('DOMContentLoaded', function () {
    // Инициализация тултипов
    initTooltips();

    // Инициализация валидации форм
    initFormValidation();

    // Подсветка активной навигации
    highlightActiveNav();

    // Анимация загрузки
    initLoadingAnimations();
});

// Инициализация Bootstrap тултипов
function initTooltips() {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Валидация форм
function initFormValidation() {
    // Автоматическая замена запятой на точку в числовых полях
    document.querySelectorAll('input[type="number"]').forEach(function (input) {
        input.addEventListener('blur', function () {
            var value = this.value;
            if (value && value.includes(',')) {
                this.value = value.replace(',', '.');
            }
        });
    });

    // Проверка минимальных/максимальных значений
    document.querySelectorAll('input[min], input[max]').forEach(function (input) {
        input.addEventListener('change', function () {
            var min = parseFloat(this.getAttribute('min'));
            var max = parseFloat(this.getAttribute('max'));
            var value = parseFloat(this.value);

            if (!isNaN(value)) {
                if (!isNaN(min) && value < min) {
                    this.value = min;
                    showToast('Значение не может быть меньше ' + min, 'warning');
                }
                if (!isNaN(max) && value > max) {
                    this.value = max;
                    showToast('Значение не может быть больше ' + max, 'warning');
                }
            }
        });
    });
}

// Подсветка активного пункта меню
function highlightActiveNav() {
    var currentPath = window.location.pathname;
    document.querySelectorAll('.nav-link').forEach(function (link) {
        var linkPath = link.getAttribute('href');
        if (linkPath && currentPath.startsWith(linkPath) && linkPath !== '/') {
            link.classList.add('active');
        }
    });
}

// Показать уведомление
function showToast(message, type = 'info') {
    var toastHtml = `
        <div class="toast align-items-center text-bg-${type} border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    `;

    var toastContainer = document.querySelector('.toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(toastContainer);
    }

    toastContainer.innerHTML += toastHtml;

    var toastEl = toastContainer.lastElementChild;
    var toast = new bootstrap.Toast(toastEl, { delay: 3000 });
    toast.show();

    toastEl.addEventListener('hidden.bs.toast', function () {
        this.remove();
    });
}

// Анимации загрузки
function initLoadingAnimations() {
    // Анимация появления элементов
    var observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
            }
        });
    }, { threshold: 0.1 });

    document.querySelectorAll('.card, .table, .chart-container').forEach(function (el) {
        observer.observe(el);
    });
}

// Функции для работы с расчетами
function formatNumber(num, decimals = 2) {
    return parseFloat(num).toFixed(decimals);
}

function calculateTemperatureDifference(materialTemp, gasTemp) {
    return materialTemp - gasTemp;
}

// Экспорт данных в CSV
function exportToCsv(data, filename) {
    var csv = 'Параметр,Значение\n';
    data.forEach(function (row) {
        csv += row.join(',') + '\n';
    });

    var blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    var link = document.createElement('a');
    var url = URL.createObjectURL(blob);

    link.setAttribute('href', url);
    link.setAttribute('download', filename + '.csv');
    link.style.visibility = 'hidden';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Загрузка файлов
function handleFileUpload(input, callback) {
    var file = input.files[0];
    if (!file) return;

    var reader = new FileReader();
    reader.onload = function (e) {
        callback(e.target.result);
    };
    reader.readAsText(file);
}

// Сохранение настроек в localStorage
function saveSettings(key, value) {
    try {
        localStorage.setItem('heat_exchange_' + key, JSON.stringify(value));
    } catch (e) {
        console.warn('Не удалось сохранить настройки:', e);
    }
}

// Загрузка настроек из localStorage
function loadSettings(key, defaultValue) {
    try {
        var value = localStorage.getItem('heat_exchange_' + key);
        return value ? JSON.parse(value) : defaultValue;
    } catch (e) {
        console.warn('Не удалось загрузить настройки:', e);
        return defaultValue;
    }
}

// Копирование текста в буфер обмена
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(function () {
        showToast('Текст скопирован в буфер обмена', 'success');
    }).catch(function (err) {
        console.error('Ошибка копирования: ', err);
        showToast('Не удалось скопировать текст', 'danger');
    });
}