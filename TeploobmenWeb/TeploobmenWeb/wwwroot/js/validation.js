

// Функция валидации числа в реальном времени
function validateNumber(input, min, max, errorMessage) {
    const value = input.value.trim();
    const errorElement = document.getElementById(input.name + '-error');

    // Очищаем предыдущие ошибки
    input.classList.remove('is-invalid', 'is-valid');
    if (errorElement) errorElement.textContent = '';

    // Проверяем, пустое ли поле
    if (!value) {
        input.classList.add('is-invalid');
        if (errorElement) errorElement.textContent = 'Это поле обязательно для заполнения';
        return false;
    }

    // Проверяем, является ли числом
    const numValue = parseFloat(value.replace(',', '.'));
    if (isNaN(numValue)) {
        input.classList.add('is-invalid');
        if (errorElement) errorElement.textContent = 'Введите числовое значение';
        return false;
    }

    // Проверяем диапазон
    if (numValue < min || numValue > max) {
        input.classList.add('is-invalid');
        if (errorElement) errorElement.textContent = errorMessage;
        return false;
    }

    // Если все ок
    input.classList.add('is-valid');
    return true;
}

// Предотвращение ввода букв в числовые поля
function preventNonNumericInput(event) {
    const allowedKeys = [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        '.', ',', 'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight',
        'Tab', 'Home', 'End'
    ];

    if (!allowedKeys.includes(event.key) && !event.ctrlKey && !event.metaKey) {
        event.preventDefault();

        // Показываем подсказку
        const input = event.target;
        const errorElement = document.getElementById(input.name + '-error');
        if (errorElement) {
            errorElement.textContent = 'Можно вводить только цифры, точку или запятую';
            errorElement.style.display = 'block';
        }

        // Скрываем подсказку через 3 секунды
        setTimeout(() => {
            if (errorElement) {
                errorElement.textContent = '';
                errorElement.style.display = 'none';
            }
        }, 3000);
    }
}

// Функция проверки всей формы перед отправкой
function validateForm() {
    const inputs = document.querySelectorAll('input[type="number"][required]');
    let isValid = true;

    // Границы для каждого поля
    const boundaries = {
        'H0': { min: 0.1, max: 50, message: 'Высота слоя должна быть от 0.1 до 50 м' },
        'TPrime': { min: -273, max: 3000, message: 'Температура должна быть от -273 до 3000°C' },
        'TDoublePrime': { min: -273, max: 2000, message: 'Температура должна быть от -273 до 2000°C' },
        'Wg': { min: 0.01, max: 20, message: 'Скорость должна быть от 0.01 до 20 м/с' },
        'Cg': { min: 0.1, max: 5, message: 'Теплоемкость должна быть от 0.1 до 5 кДж/(кг·К)' },
        'Gm': { min: 0.01, max: 50, message: 'Расход должен быть от 0.01 до 50 кг/с' },
        'Cm': { min: 0.1, max: 5, message: 'Теплоемкость должна быть от 0.1 до 5 кДж/(кг·К)' },
        'Av': { min: 10, max: 20000, message: 'Коэффициент должен быть от 10 до 20000 Вт/(м³·К)' },
        'D': { min: 0.1, max: 20, message: 'Диаметр должен быть от 0.1 до 20 м' },
        'PointsCount': { min: 2, max: 200, message: 'Количество точек должно быть от 2 до 200' }
    };

    // Проверяем каждое поле
    inputs.forEach(input => {
        const boundary = boundaries[input.name];
        if (boundary) {
            if (!validateNumber(input, boundary.min, boundary.max, boundary.message)) {
                isValid = false;
            }
        }
    });

    return isValid;
}

// Инициализация валидации для формы
function initValidation() {
    // Добавляем обработчики для предотвращения ввода букв
    document.querySelectorAll('input[type="number"]').forEach(input => {
        input.addEventListener('keydown', preventNonNumericInput);

        // Валидация при потере фокуса
        input.addEventListener('blur', function () {
            const boundary = {
                'H0': { min: 0.1, max: 50, message: 'Высота слоя должна быть от 0.1 до 50 м' },
                'TPrime': { min: -273, max: 3000, message: 'Температура должна быть от -273 до 3000°C' },
                'TDoublePrime': { min: -273, max: 2000, message: 'Температура должна быть от -273 до 2000°C' },
                'Wg': { min: 0.01, max: 20, message: 'Скорость должна быть от 0.01 до 20 м/с' },
                'Cg': { min: 0.1, max: 5, message: 'Теплоемкость должна быть от 0.1 до 5 кДж/(кг·К)' },
                'Gm': { min: 0.01, max: 50, message: 'Расход должен быть от 0.01 до 50 кг/с' },
                'Cm': { min: 0.1, max: 5, message: 'Теплоемкость должна быть от 0.1 до 5 кДж/(кг·К)' },
                'Av': { min: 10, max: 20000, message: 'Коэффициент должен быть от 10 до 20000 Вт/(м³·К)' },
                'D': { min: 0.1, max: 20, message: 'Диаметр должен быть от 0.1 до 20 м' },
                'PointsCount': { min: 2, max: 200, message: 'Количество точек должно быть от 2 до 200' }
            }[input.name];

            if (boundary) {
                validateNumber(input, boundary.min, boundary.max, boundary.message);
            }
        });
    });

    // Валидация при отправке формы
    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', function (e) {
            if (!validateForm()) {
                e.preventDefault();
                showAlert('Исправьте ошибки в форме перед отправкой', 'danger');
            }
        });
    }
}

// Экспорт функций для использования в других файлах
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        validateNumber,
        preventNonNumericInput,
        validateForm,
        initValidation
    };
}