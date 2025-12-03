// ================================
// GESTIÓN DEL CARRITO DE COMPRAS
// Usa sessionStorage para mantener los datos temporalmente
// ================================

// Inicializar carrito al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    updateCartDisplay();
});

// Obtener carrito desde sessionStorage
function getCart() {
    const cart = sessionStorage.getItem('restaurantCart');
    return cart ? JSON.parse(cart) : [];
}

// Guardar carrito en sessionStorage
function saveCart(cart) {
    sessionStorage.setItem('restaurantCart', JSON.stringify(cart));
}

// Aumentar cantidad
function increaseQuantity(btn) {
    const input = btn.parentElement.querySelector('.quantity-input');
    let value = parseInt(input.value);
    if (value < 99) {
        input.value = value + 1;
    }
}

// Disminuir cantidad
function decreaseQuantity(btn) {
    const input = btn.parentElement.querySelector('.quantity-input');
    let value = parseInt(input.value);
    if (value > 1) {
        input.value = value - 1;
    }
}

// Agregar al carrito
function addToCart(id, name, price, btn) {
    const card = btn.closest('.menu-card');
    const quantity = parseInt(card.querySelector('.quantity-input').value);

    // Obtener carrito actual
    let cart = getCart();

    // Buscar si el producto ya existe
    const existingItemIndex = cart.findIndex(item => item.id === id);

    if (existingItemIndex !== -1) {
        // Si existe, aumentar cantidad
        cart[existingItemIndex].quantity += quantity;
    } else {
        // Si no existe, agregar nuevo
        cart.push({
            id: id,
            name: name,
            price: price,
            quantity: quantity
        });
    }

    // Guardar carrito
    saveCart(cart);

    // Actualizar display
    updateCartDisplay();

    // Animación visual
    btn.classList.add('added');
    const originalHTML = btn.innerHTML;
    btn.innerHTML = '✓ Agregado';
    btn.style.background = '#198754';

    setTimeout(() => {
        btn.innerHTML = originalHTML;
        btn.style.background = '#E53935';
        btn.classList.remove('added');
    }, 1500);

    // Resetear cantidad a 1
    card.querySelector('.quantity-input').value = 1;
}

// Actualizar display del carrito flotante
function updateCartDisplay() {
    const cart = getCart();

    // Calcular totales
    let totalItems = 0;
    let totalPrice = 0;

    cart.forEach(item => {
        totalItems += item.quantity;
        totalPrice += item.price * item.quantity;
    });

    // Actualizar UI
    document.getElementById('cartBadge').textContent = totalItems;
    document.getElementById('cartTotal').textContent = '$' + totalPrice.toFixed(2);
}

// Ir a la página del carrito
function goToCart() {
    const cart = getCart();
    if (cart.length > 0) {
        // Redirigir a la página del carrito - AJUSTA ESTA RUTA
        window.location.href = '/menus/Carrito.js';
    } else {
        // Mostrar mensaje si el carrito está vacío
        showNotification('El carrito está vacío', 'warning');
    }
}

// Mostrar notificación temporal
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} notification-toast`;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        min-width: 250px;
        animation: slideIn 0.3s ease;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    `;
    notification.textContent = message;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => notification.remove(), 300);
    }, 3000);
}

// Filtros de categoría
document.querySelectorAll('.filter-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
        this.classList.add('active');

        const filter = this.dataset.filter;

        document.querySelectorAll('.menu-item').forEach(item => {
            if (filter === 'all' || item.dataset.category === filter) {
                item.style.display = 'block';
            } else {
                item.style.display = 'none';
            }
        });
    });
});

// Animaciones CSS
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);